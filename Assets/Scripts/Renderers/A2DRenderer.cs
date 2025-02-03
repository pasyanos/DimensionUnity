using UnityEngine;

public abstract class A2DRenderer : MonoBehaviour, I2DRenderer
{
    public abstract void WriteToFile(string fileName);
}
