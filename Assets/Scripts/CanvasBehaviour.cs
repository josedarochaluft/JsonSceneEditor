using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBehaviour : MonoBehaviour
{
    [SerializeField]
    List<RawImage> previewImages;

    [SerializeField]
    List<Texture2D> thumbnails;

    [SerializeField]
    GameObject loadingImage;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void AddThumbnail(Texture2D texture)
    {
        if(texture)
        {
            thumbnails.Add(texture);
        }
        else
        {
            Debug.Log("Trying to insert null value as texture!");
        }
    }

    public void DisplayThumbnails()
    {
        for(int i = 0; i < previewImages.Count; ++i)
        {
            previewImages[i].texture = thumbnails[i];
            previewImages[i].enabled = true;
        }
    }

    public void DeactivateLoadingDisplay()
    {
        loadingImage.SetActive(false);
    }
}
