using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Utils
{
    #region Write Texture Utils
    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    };

    /// <summary>
    /// Saves a Texture2D to disk with the specified filename and image format
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="filePath"></param>
    /// <param name="fileFormat"></param>
    /// <param name="jpgQuality"></param>
    static public void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat, int jpgQuality = 95)
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
    #endregion // Write texture utils

    static public int GetKeyframeCount(AnimationClip clip, float targetFPS)
    {
        float len = clip.length;
        float clipFPS = clip.frameRate;

        // deal with some edge cases where our target FPS doesn't equal the clip FPS
        // but it's fine as long as the target can be divided by the clip FPS
        if (targetFPS < clipFPS)
        {
            Debug.LogWarningFormat("Target FPS of {0} is less than clip's FPS of {1}; " +
                "some animation detail might be lost", targetFPS, clipFPS);
        }
        else if (targetFPS > clipFPS && targetFPS % clipFPS != 0)
        {
            Debug.LogWarningFormat("Target FPS of {0} is not divisible by clip FPS of {1};" +
                " May cause strange artifacts or skip keyframes.", targetFPS, clipFPS);
        }

        return Mathf.RoundToInt(targetFPS * len) + 1;
    }
}
