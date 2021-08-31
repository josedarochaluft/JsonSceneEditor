using UnityEngine;

[System.Serializable]
public class Model
{
    public string name;
    public float[] position;
    public float[] rotation;
    public float[] scale;

    public Model(Transform t)
    {
        UpdateModelData(t);
    }

    public void UpdateModelData(Transform t)
    {
        position = Vector3ToArray(t.localPosition);
        rotation = QuaternionToArray(t.localRotation);
        scale = Vector3ToArray(t.localScale);
    }

    float[] Vector3ToArray(Vector3 vector)
    {
        return new float[] { vector.x, vector.y, vector.z };
    }

    float[] QuaternionToArray(Quaternion quaternion)
    {
        Vector3 rotationVector = quaternion.eulerAngles;
        return Vector3ToArray(rotationVector);
    }
}