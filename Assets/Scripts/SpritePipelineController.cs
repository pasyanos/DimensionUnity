using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using static RenderTextureToFileUtil;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    [Header("Sprite Settings")]
    [SerializeField] private Vector2Int _outResolution = new Vector2Int(256, 256);
    [SerializeField] private int _outDepth = 32;
    [SerializeField] private string _spriteName;
    [SerializeField] private bool _appendDateTime = true;

    [Header("3D Animation Assets")]
    [SerializeField] private Animator _animatorComponent;
    [SerializeField] private List<AnimationClip> _targetClips;

    // public facing getters, mostly for UI
    public RenderTexture currentRenderTexture { get { return _sideRenderCamera.targetTexture; } }

    // runtime variables
    private List<float[]> keyframeTimes = null;
    private int _currentClipIndex;
    private int _currentFrameIndex;
    private int _currentClipKeyframes;

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
        // cache keyframe information 
        CacheKeyframesForAllClips();
        
        _sideRenderCamera.gameObject.SetActive(true);

        // reset clip and frame indices
        SetTargetClip(0);
        _currentFrameIndex = 0;
        _animatorComponent.speed = 0f;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    AdvanceKeyframe(_targetClips[0]);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RenderToSprite();
        }
    }
    #endregion

    #region Public-facing methods
    public void RenderToSprite()
    {
        string dateTimeStr = _appendDateTime ? 
            "_" + System.DateTime.Now.ToString("MM_dd_HH_mm") : "";
        string fileNameStr = "Assets/Output/" + _spriteName + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        //// if there are any clips, do the thing
        //if (_targetClips.Count > 0)
        //{
        //    var keyframes = KeyframeUtils.CacheAnimationKeyframes(_targetClips[0]);

        //    foreach (var keyframe in keyframes)
        //    {
        //        Debug.LogErrorFormat("Keyframe for clip {0}: {1}", _targetClips[0].name, keyframe);
        //    }
            
        //    // todo: go through all clips. this can be replaced with a single for loop instead of an if.
        //}

        // todo: comment back in
        // RenderTextureToFileUtil.SaveRenderTextureToFile(rt, fileNameStr, RenderTextureToFileUtil.SaveTextureFileFormat.PNG);
        List<Texture2D> capturedFrames = new List<Texture2D>();
        
        for (int i = 0; i < _currentClipKeyframes; ++i)
        {
            capturedFrames.Add(WriteRenderTextureToTex2D(rt, SaveTextureFileFormat.PNG));
            // Debug.LogFormat("Keframe index {0}", i);
            AdvanceKeyframe(_targetClips[0]);
        }

        int counter = 0;
        foreach(var tex in capturedFrames)
        {
            RenderTextureToFileUtil.SaveTexture2DToFile(tex, fileNameStr + string.Format("_{0}", counter), SaveTextureFileFormat.PNG);
            counter++;
        }
        // Debug.Log("Wrote texture to file " + fileNameStr);
    }
    #endregion

    #region Private Helper Methods
    private void CacheKeyframesForAllClips()
    {
        keyframeTimes = new List<float[]>();

        foreach (AnimationClip clip in _targetClips)
        {
            keyframeTimes.Add(KeyframeUtils.CacheAnimationKeyframes(clip));
        }
    }

    private void SetTargetClip(int index)
    {
        if (index < 0 || index >= _targetClips.Count) 
        {
            Debug.LogError("invalid index");
            return; 
        }

        // reset all clip and keyframe counters
        _currentClipIndex = index;
        _currentClipKeyframes = keyframeTimes[_currentClipIndex].Length;
        _currentFrameIndex = 0;
        float keyframeTime = keyframeTimes[_currentClipIndex][_currentFrameIndex];

        SetAnimationAndKeyframe(_targetClips[_currentClipIndex], keyframeTime);
    }

    private void AdvanceKeyframe(AnimationClip clip)
    {
        _currentFrameIndex = Mathf.Min(_currentFrameIndex + 1, _currentClipKeyframes - 1);

        float keyframeTime = keyframeTimes[_currentClipIndex][_currentFrameIndex];
        //_animatorComponent.Play(clip.name, 0, keyframeTime / clip.length);
        //_animatorComponent.Update(0);
        SetAnimationAndKeyframe(clip, keyframeTime);
    }

    private void SetAnimationAndKeyframe(AnimationClip clip, float keyframeTime)
    {
        _animatorComponent.Play(clip.name, 0, keyframeTime / clip.length);
        _animatorComponent.Update(0);
    }

    static private Texture2D WriteRenderTextureToTex2D(RenderTexture rt,
        SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG)
    {
        Texture2D tex;

        if (fileFormat != SaveTextureFileFormat.EXR)
            tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, false);
        else
            tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);

        var oldRt = RenderTexture.active;

        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        //if (Application.isPlaying)
        //    Object.Destroy(tex);
        //else
        //    Object.DestroyImmediate(tex);
        RenderTexture.active = oldRt;

        return tex;
    }
    #endregion
}

