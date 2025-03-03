using UnityEngine;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    [SerializeField] private Vector2Int _outResolution = new Vector2Int(256, 256);
    [SerializeField] private int _outDepth = 32;




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

    public void RenderToSprite()
    {
        string dateTimeStr = System.DateTime.Now.ToString("MM_dd_HH_mm");
        string fileNameStr = "Assets/Output/TestRenderTexture_" + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        RenderTextureToFileUtil.SaveRenderTextureToFile(rt, fileNameStr, RenderTextureToFileUtil.SaveTextureFileFormat.PNG);

        Debug.LogError("Wrote texture to file " + fileNameStr);
    }
}

