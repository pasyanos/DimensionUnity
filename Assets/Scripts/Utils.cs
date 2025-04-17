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

    #region Keyframe Reading Utils
    public static float[] CacheAnimationKeyframes(AnimationClip clip)
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);
        var timingsSet = new HashSet<float>();

        // collect all timings in the hashset. Duplicates are not allowed.
        foreach (var binding in bindings)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            foreach (var key in curve.keys)
                timingsSet.Add(key.time);
        }

        // sort hashset by ascending time
        var ret = timingsSet.OrderBy(t => t).ToArray();

        Debug.LogFormat("Animation Clip {0}: {1} keyframes", clip.name, ret.Length);

        return ret;
    }
    #endregion // Keyframe Reading Utils
}
