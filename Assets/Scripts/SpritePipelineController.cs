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

    public void RenderToSprite()
    {
        string dateTimeStr = System.DateTime.Now.ToString("MM_dd_HH_mm");
        string fileNameStr = "Assets/Output/TestRenderTexture_" + dateTimeStr;

        RenderTexture rt = _sideRenderCamera.targetTexture;

        RenderTextureToFileUtil.SaveRenderTextureToFile(rt, fileNameStr, RenderTextureToFileUtil.SaveTextureFileFormat.PNG);

        Debug.LogError("Wrote texture to file " + fileNameStr);
    }
}

