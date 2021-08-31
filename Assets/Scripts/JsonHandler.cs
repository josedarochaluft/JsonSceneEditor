using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

public class JsonHandler : MonoBehaviour
{
    public ModelList modelList = new ModelList();

    List<Transform> sceneTransforms;

    [SerializeField]
    List<GameObject> loadedObjects;

    [SerializeField]
    CanvasBehaviour canvasBehaviour;

    [SerializeField]
    MaterialManager materialManager;

    // Start is called before the first frame update
    void Start()
    {
        sceneTransforms = new List<Transform>();
        loadedObjects = new List<GameObject>();
        StartCoroutine("SceneSetup");
    }

    IEnumerator SceneSetup()
    {
        LoadObject("MetalTable");
        LoadObject("3Seat");
        LoadObject("Fotel");
        LoadObject("Couch");
        LoadObject("chair");
        yield return new WaitForSeconds(0.5f);
        GetJsonData();
    }

    void LoadObject(string name)
    {
        loadedObjects.Add(Resources.Load($"Prefabs/{name}") as GameObject);
    }

    public async void GetJsonData()
    {
        var url = "https://s3-sa-east-1.amazonaws.com/static-files-prod/unity3d/models.json";

        using (var www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.downloadHandler.text);
                    //Get data from the webrequest in the form of a string, removing the first 3 bytes to allow deserialization
                    string jsonString = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data, 3, www.downloadHandler.data.Length - 3);

                    //Deserialize JSON string
                    modelList = JsonUtility.FromJson<ModelList>(jsonString);

                    //Populate Scene
                    PopulateScene();
                }
            }
        }
    }

    public void ClearScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        sceneTransforms.Clear();
    }

    void PopulateScene()
    {
        int i = 0;
        foreach (Model m in modelList.models)
        {
            GameObject newObject = Instantiate(loadedObjects[i], new Vector3(m.position[0], m.position[1], m.position[2]), Quaternion.Euler(m.rotation[0], m.rotation[1], m.rotation[2]), transform);
            newObject.transform.localScale = new Vector3(m.scale[0], m.scale[1], m.scale[2]);
            newObject.name = m.name;

            InteractiveObject objectScript = newObject.GetComponent<InteractiveObject>();
            if (objectScript)
            {
                objectScript.jsonHandler = this;
            }
            sceneTransforms.Add(newObject.transform);
            materialManager.AddObject(objectScript);

            //Use asset preview for thumbnails when in editor, use manually saved textures otherwise
            string loadingPath = "Textures/Thumbnail_" + loadedObjects[i].name;
            Texture2D thumbnail;
#if UNITY_EDITOR
            thumbnail = AssetPreview.GetAssetPreview(loadedObjects[i]);
#elif UNITY_STANDALONE
            thumbnail = Resources.Load(loadingPath) as Texture2D ;
#endif
            canvasBehaviour.AddThumbnail(thumbnail);

            //Increment i to change the models used in every instance
            i++;
            i = i % 5;
        }
        canvasBehaviour.DeactivateLoadingDisplay();
        canvasBehaviour.DisplayThumbnails();
    }

    public void SerializeJson()
    {
        UpdateListData();

        string path = Application.dataPath + "/SceneData.json";
        string contents = JsonUtility.ToJson(modelList, true);
        System.IO.File.WriteAllText(path, contents);

        Debug.Log($"Serialized Model List in filepath: {path}");
        Debug.Log(contents);
    }

    void UpdateListData()
    {
        for(int i = 0; i < modelList.models.Count; ++i)
        {
            modelList.models[i].UpdateModelData(sceneTransforms[i]);
        }
    }

    public void AddToList(Transform toAdd)
    {
        Model newModel = new Model(toAdd);
        newModel.name = $"modelo{(modelList.models.Count<9?"0":"")}{modelList.models.Count+1}";
        modelList.models.Add(newModel);
        toAdd.name = newModel.name;
        sceneTransforms.Add(toAdd);
        InteractiveObject newObjectScript = toAdd.gameObject.GetComponent<InteractiveObject>();
        if(newObjectScript)
        {
            newObjectScript.jsonHandler = this;
        }
        
    }

#if UNITY_EDITOR
    [ContextMenu("Create Thumbnails")]
    //Use this during the editor's play mode to generate thumbnails that we can use outside of the editor
    void CreateThumbnails()
    {
        foreach(GameObject g in loadedObjects)
        {
            string path = Application.dataPath + "/Resources/Textures/Thumbnail_" + g.name + ".png";
            System.IO.File.WriteAllBytes(path, AssetPreview.GetAssetPreview(g).EncodeToPNG());
            Debug.Log("Created file in path:" + path);
        }
    }
#endif
}