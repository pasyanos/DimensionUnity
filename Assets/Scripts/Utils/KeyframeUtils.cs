using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeyframeUtils
{
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
}
