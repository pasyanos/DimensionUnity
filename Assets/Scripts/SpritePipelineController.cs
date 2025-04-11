using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static RenderTextureToFileUtil;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    [Header("2D Render Settings")]
    [SerializeField] private Vector2Int _outResolution = new Vector2Int(256, 256);
    [SerializeField] private int _outDepth = 32;

    [Header("Sprite Sheet Output Settings")]
    [SerializeField] private int maxSpriteSheetWidth = 10;
    [SerializeField] private string _spriteName;
    [SerializeField] private bool _appendDateTime = true;

    [Header("3D Animation Assets")]
    [SerializeField] private Animator _animatorComponent;
    // [SerializeField] private List<AnimationClip> _targetClips;
    [SerializeField] private AnimationClip _targetClip;

    // public facing getters, mostly for UI
    public RenderTexture currentRenderTexture { get { return _sideRenderCamera.targetTexture; } }

    // runtime variables
    // private List<float[]> keyframeTimes = null;
    float[] cachedKeyframeTimes = null;
    private int _currentFrameIndex;
    private int _numFrames;

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
        _currentFrameIndex = 0;
        _animatorComponent.speed = 0f;
        SetAnimationAndKeyframe(_targetClip, cachedKeyframeTimes[_currentFrameIndex]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // RenderAllKeyframes();
            StartCoroutine(RenderKeyframesCoroutine());
        }
    }
    #endregion

    #region Public-facing methods
    public void RenderAllKeyframes()
    {
        string dateTimeStr = _appendDateTime ? 
            "_" + System.DateTime.Now.ToString("MM_dd_HH_mm") : "";
        string fileNameStr = "Assets/Output/" + _spriteName + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        List<Texture2D> capturedFrames = new List<Texture2D>();
        
        for (int i = 0; i < _numFrames; ++i)
        {
            capturedFrames.Add(WriteRenderTextureToTex2D(rt, SaveTextureFileFormat.PNG));
            // AdvanceKeyframe(_targetClips[0]);
            AdvanceKeyframe(_targetClip);
        }

        RenderToSingleImage(capturedFrames, fileNameStr);
    }

    public IEnumerator RenderKeyframesCoroutine()
    {
        string dateTimeStr = _appendDateTime ?
            "_" + System.DateTime.Now.ToString("MM_dd_HH_mm") : "";
        string fileNameStr = "Assets/Output/" + _spriteName + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        List<Texture2D> capturedFrames = new List<Texture2D>();

        for (int i = 0; i < _numFrames; ++i)
        {
            _sideRenderCamera.Render();
            capturedFrames.Add(WriteRenderTextureToTex2D(rt, SaveTextureFileFormat.PNG));

            yield return new WaitForEndOfFrame();

            AdvanceKeyframe(_targetClip);
        }

        RenderToSingleImage(capturedFrames, fileNameStr);
    }
    #endregion

    #region Private Helper Methods
    private void CacheKeyframesForAllClips()
    {
        // keyframeTimes = new List<float[]>();
        //foreach (AnimationClip clip in _targetClips)
        //{
        //    keyframeTimes.Add(KeyframeUtils.CacheAnimationKeyframes(clip));
        //}

        cachedKeyframeTimes = KeyframeUtils.CacheAnimationKeyframes(_targetClip);
        _numFrames = cachedKeyframeTimes.Length;
    }

    //private void SetTargetClip(int index)
    //{
    //    if (index < 0 || index >= _targetClips.Count) 
    //    {
    //        Debug.LogError("invalid index");
    //        return; 
    //    }

    //    // reset all clip and keyframe counters
    //    _currentClipIndex = index;
    //    _currentClipKeyframes = keyframeTimes[_currentClipIndex].Length;
    //    _currentFrameIndex = 0;
    //    float keyframeTime = keyframeTimes[_currentClipIndex][_currentFrameIndex];

    //    SetAnimationAndKeyframe(_targetClips[_currentClipIndex], keyframeTime);
    //}

    private void AdvanceKeyframe(AnimationClip clip)
    {
        _currentFrameIndex = Mathf.Min(_currentFrameIndex + 1, _numFrames - 1);

        float keyframeTime = cachedKeyframeTimes[_currentFrameIndex];
        SetAnimationAndKeyframe(clip, keyframeTime);
    }

    private void SetAnimationAndKeyframe(AnimationClip clip, float keyframeTime)
    {
        _animatorComponent.Play(clip.name, 0, keyframeTime / clip.length);
        _animatorComponent.Update(0);
    }

    private void RenderToSingleImage(List<Texture2D> capturedFrames, string fileNameStr)
    {
        // combine all textures into a grid
        int frames = capturedFrames.Count;

        int sheetColumns = Mathf.Min(frames, maxSpriteSheetWidth);
        int sheetRows = Mathf.CeilToInt((float)frames / sheetColumns);

        // this may be greater than the actual number of frames
        int numSprites = sheetColumns * sheetRows;

        Texture2D combinedTexture = new Texture2D(_outResolution.x * sheetColumns, _outResolution.y * sheetRows);
        int spriteSheetHeight = combinedTexture.height;

        Color[] emptyPixels = new Color[_outResolution.x * _outResolution.y];

        for (int i = 0; i < emptyPixels.Length; ++i)
            emptyPixels[i] = new Color(0, 0, 0, 0);

        for (int i = 0; i < numSprites; ++i)
        {
            int xPos = (i % sheetColumns) * _outResolution.x;
            int yPos = (i / sheetColumns) * _outResolution.y;

            Color[] pixels;

            if (i < frames)
            {
                //int xPos = (i % sheetColumns) * _outResolution.x;
                //int yPos = (i / sheetColumns) * _outResolution.y;

                pixels = capturedFrames[i].GetPixels();
            }
            else
            {
                pixels = emptyPixels;
            }

            combinedTexture.SetPixels(xPos, spriteSheetHeight - yPos - _outResolution.y, _outResolution.x, _outResolution.y, pixels);
        }

        combinedTexture.Apply();

        RenderTextureToFileUtil.SaveTexture2DToFile(combinedTexture, fileNameStr, SaveTextureFileFormat.PNG);
        Debug.LogErrorFormat("Rendered sprite sheet to {0}.png", fileNameStr);
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

        RenderTexture.active = oldRt;

        return tex;
    }
    #endregion
}
