using UnityEngine;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        RenderTexture rt = _sideRenderCamera.targetTexture;

    //        RenderTextureToFileUtil.SaveRenderTextureToFile(rt, "Assets/TestRenderTexture", RenderTextureToFileUtil.SaveTextureFileFormat.PNG);

    //        Debug.LogError("Wrote texture");
    //    }
    //}

    private void Awake()
    {
        // make a new rendertexture
        RenderTexture sideCamOutput = new RenderTexture(64, 64, 32);
        sideCamOutput.name = "SideCamOutput";
        sideCamOutput.enableRandomWrite = true;
        sideCamOutput.Create();

        // assign the rendertexture to the camera
        _sideRenderCamera.targetTexture = sideCamOutput;

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

