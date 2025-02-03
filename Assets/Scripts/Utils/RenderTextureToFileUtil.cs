using UnityEngine;

public class RenderTextureToFileUtil
{
    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    };

    public static void SaveTextureToFile(Texture2D tex, string filePath, 
        SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
    {
        switch (fileFormat)
        {
            case SaveTextureFileFormat.EXR:
                System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
                break;
            case SaveTextureFileFormat.JPG:
                System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
                break;
            case SaveTextureFileFormat.PNG:
                System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
                break;
            case SaveTextureFileFormat.TGA:
                System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
                break;
        }
    }

    public static void SaveRenderTextureToFile(RenderTexture rt, string filePath,
        SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
    {
        Texture2D tex;

        if (fileFormat != SaveTextureFileFormat.EXR)
            tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
        else
            tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);

        var oldRT = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = oldRT;
        SaveTextureToFile(tex, filePath, fileFormat, jpgQuality);

        if (Application.isPlaying)
            Object.Destroy(tex);
        else
            Object.DestroyImmediate(tex);
    }
}
