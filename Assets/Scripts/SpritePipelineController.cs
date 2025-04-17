using UnityEngine;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
// using static RenderTextureToFileUtil;

public class SpritePipelineController : MonoBehaviour
{
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
    [SerializeField] private List<AnimationClip> _targetClips;

    // public facing getters, mostly for UI
    public RenderTexture currentRenderTexture { get { return _sideRenderCamera.targetTexture; } }

    // runtime variables
    private int currentClipIndex = 0;
    private List<float[]> keyframeTimes = new List<float[]>();
    private int currentFrameIndex;

    #region Unity Callbacks
    private void Awake()
    {
        // make a new rendertexture and assign it to the side camera
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
        SetClipIndex(0);
        _animatorComponent.speed = 0f;
        SetAnimationAndKeyframe(_targetClips[currentClipIndex], keyframeTimes[currentClipIndex][currentFrameIndex]);
    }
    #endregion

    #region Public-facing methods
    public void OnGUIButton()
    {
        // Debug.LogError(":)");
        StartCoroutine(RenderKeyframesCoroutine());
    }
    #endregion

    #region Private Helper Methods
    private IEnumerator RenderKeyframesCoroutine()
    {
        string dateTimeStr = _appendDateTime ?
            "_" + System.DateTime.Now.ToString("MM_dd_HH_mm") : "";
        string fileNameStr = "Assets/Output/" + _spriteName + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        int numClips = _targetClips.Count;

        for (int i = 0; i < numClips; ++i)
        {
            SetClipIndex(i);
            AnimationClip clip = _targetClips[i];

            // reset animation
            SetAnimationAndKeyframe(clip, keyframeTimes[i][currentFrameIndex]);

            List<Texture2D> framesForClip = new List<Texture2D>();
            yield return RenderFramesForSingleClip(framesForClip, clip, i, rt, _sideRenderCamera);
            RenderToSingleImage(framesForClip, fileNameStr + string.Format("_{0}", clip.name));
        }
    }

    private IEnumerator RenderFramesForSingleClip(List<Texture2D> outputFrames, 
        AnimationClip clip, int clipIndex, RenderTexture rt, Camera renderCam)
    {
        int numFrames = keyframeTimes[clipIndex].Length;

        for (int i = 0; i < numFrames; ++i)
        {
            // todo: may not be necessary
            renderCam.Render();
            outputFrames.Add(WriteRenderTextureToTex2D(rt, Utils.SaveTextureFileFormat.PNG));

            yield return new WaitForEndOfFrame();

            AdvanceKeyframe(clip, numFrames);
        }

        Debug.LogFormat("Finished rendering for animation clip {0}", clip.name);
    }

    private void CacheKeyframesForAllClips()
    {
        keyframeTimes.Clear();
        // keyframeTimes = new List<float[]>();
        foreach (AnimationClip clip in _targetClips)
        {
            keyframeTimes.Add(Utils.CacheAnimationKeyframes(clip));
        }
    }

    private void SetClipIndex(int index)
    {
        if (index >= 0 && index < _targetClips.Count)
        {
            currentClipIndex = index;

            // begin at frame 0
            currentFrameIndex = 0;
        }
    }

    private void AdvanceKeyframe(AnimationClip clip, int numFrames)
    {
        currentFrameIndex = Mathf.Min(currentFrameIndex + 1, numFrames - 1);

        float keyframeTime = keyframeTimes[0][currentFrameIndex];
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
                pixels = capturedFrames[i].GetPixels();
            }
            else
            {
                pixels = emptyPixels;
            }

            combinedTexture.SetPixels(xPos, spriteSheetHeight - yPos - _outResolution.y, _outResolution.x, _outResolution.y, pixels);
        }

        combinedTexture.Apply();

        Utils.SaveTexture2DToFile(combinedTexture, fileNameStr, Utils.SaveTextureFileFormat.PNG);
        Debug.LogFormat("Rendered sprite sheet to {0}.png", fileNameStr);
    }

    static private Texture2D WriteRenderTextureToTex2D(RenderTexture rt,
        Utils.SaveTextureFileFormat fileFormat = Utils.SaveTextureFileFormat.PNG)
    {
        Texture2D tex;

        if (fileFormat != Utils.SaveTextureFileFormat.EXR)
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
