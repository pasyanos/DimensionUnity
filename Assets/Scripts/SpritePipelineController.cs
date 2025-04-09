using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    [SerializeField] private Vector2Int _outResolution = new Vector2Int(256, 256);
    [SerializeField] private int _outDepth = 32;

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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AdvanceKeyframe(_targetClips[0]);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RenderToSprite();
        }
    }
    #endregion

    #region Public-facing methods
    public void RenderToSprite()
    {
        string dateTimeStr = System.DateTime.Now.ToString("MM_dd_HH_mm");
        string fileNameStr = "Assets/Output/TestRenderTexture_" + dateTimeStr;

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
        RenderTextureToFileUtil.SaveRenderTextureToFile(rt, fileNameStr, RenderTextureToFileUtil.SaveTextureFileFormat.PNG);
        Debug.LogError("Wrote texture to file " + fileNameStr);
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
    #endregion
}

