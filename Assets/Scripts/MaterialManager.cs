using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField]
    List<InteractiveObject> objects;

    Color[] colors = { Color.white, Color.blue, Color.red };

    int objectIndex;

    public void ChooseObject(int index)
    {
        objectIndex = index;
    }

    public void ChooseColor(int colorIndex)
    {
        objects[objectIndex].ChooseColor(colorIndex);
    }

    public void ChooseTexture(int textureIndex)
    {
        objects[objectIndex].ChooseTexture(textureIndex);
    }

    public void AddObject(InteractiveObject o)
    {
        objects.Add(o);
    }
}
