using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    [SerializeField] private Vector2Int _outResolution = new Vector2Int(256, 256);
    [SerializeField] private int _outDepth = 32;

    [SerializeField] private List<AnimationClip> _targetClips;

    #region Unity Callbacks
    private void Awake()
    {
        // make a new rendertexture
        RenderTexture sideCamOutput = new RenderTexture(_outResolution.x, _outResolution.y, _outDepth);
        sideCamOutput.name = "SideCamOutput";
        sideCamOutput.enableRandomWrite = true;
        sideCamOutput.Create();

        // assign the rendertexture to the camera
        _sideRenderCamera.targetTexture = sideCamOutput;

    }

    private void Start()
    {
        _sideRenderCamera.gameObject.SetActive(true);
    }
    #endregion

    #region Public-facing methods
    public void RenderToSprite()
    {
        string dateTimeStr = System.DateTime.Now.ToString("MM_dd_HH_mm");
        string fileNameStr = "Assets/Output/TestRenderTexture_" + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        // if there are any clips, do the thing
        if (_targetClips.Count > 0)
        {
            // todo: go through all clips. this can be replaced with a single for loop instead of an if.
            CycleThroughKeyframes(_targetClips[0]);
        }

        // todo: comment back in
        // RenderTextureToFileUtil.SaveRenderTextureToFile(rt, fileNameStr, RenderTextureToFileUtil.SaveTextureFileFormat.PNG);

        // Debug.LogError("Wrote texture to file " + fileNameStr);
    }
    #endregion

    #region Private Helper Methods
    private void CycleThroughKeyframes(AnimationClip clip)
    {
        // animation events
        Debug.LogError("Animation events: " + clip.events.Length + " for clip " + clip.name + " at " + clip.length + " seconds.");

        var keyframes = clip.events;
        foreach (var frame in keyframes)
        {
            Debug.LogError("Keyframe: " + frame.functionName + " at " + frame.time + " seconds. " + frame.intParameter + " " + frame.floatParameter + " " + frame.objectReferenceParameter);
        }
    }
    #endregion
}

