using UnityEngine;

public class SpritePipelineController : MonoBehaviour
{
    // todo: make this a list of cameras, corresponding to the number of perspective types
    [SerializeField] private Camera _sideRenderCamera;
    // [SerializeField] private Texture2D _testTexture;


    private void Start() {}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogError("Hi");

            // RenderTextureToFileUtil.SaveTextureToFile(_testTexture, "Assets/TestRender2");
            RenderTexture rt = _sideRenderCamera.targetTexture;

            RenderTextureToFileUtil.SaveRenderTextureToFile(rt, "Assets/TestRenderTexture", RenderTextureToFileUtil.SaveTextureFileFormat.PNG);

            Debug.LogError("Wrote texture");
        }
    }
}
