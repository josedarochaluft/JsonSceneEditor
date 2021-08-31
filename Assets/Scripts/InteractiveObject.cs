using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public JsonHandler jsonHandler;
    Camera mainCamera;
    float rotationSpeed = -90.0f;
    float scalingFactor = .1f;
    bool rotating = false;
    Vector3 oldMousePosition, mouseDelta = Vector3.zero;

    [SerializeField]
    Material material;

    public Texture2D[] textures;

    Color[] colors = { Color.white, Color.blue, Color.red };

    int chosenColor = 0, chosenTexture = 0;

    private void OnValidate()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        oldMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        mouseDelta = Input.mousePosition - oldMousePosition;
        oldMousePosition = Input.mousePosition;
        if (rotating)
        {
            if(Input.GetMouseButtonUp(1))
            {
                rotating = false;
            }
            else
            {
                transform.Rotate(0.0f, mouseDelta.x * Time.deltaTime * rotationSpeed, 0.0f);
            }
        }
    }

    private void OnMouseDrag()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            transform.position = cameraRay.GetPoint(rayLength);
        }
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            rotating = true;
        }
        if(Input.mouseScrollDelta.y != 0.0f)
        {
            transform.localScale += Vector3.one * Input.mouseScrollDelta.y * scalingFactor;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            jsonHandler.AddToList(Instantiate(gameObject, transform.parent).transform);
        }
    }

    public void ChooseColor(int colorIndex)
    {
        material.color = colors[colorIndex];
    }

    public void ChooseTexture(int textureIndex)
    {
        material.mainTexture = textures[textureIndex];
    }
}
