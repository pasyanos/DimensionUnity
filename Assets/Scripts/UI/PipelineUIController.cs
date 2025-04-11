using UnityEngine;
using UnityEngine.UI;

public class PipelineUIController : MonoBehaviour
{
    [SerializeField] private SpritePipelineController controllerRef;
    [SerializeField] private RawImage rawImg;
    
    public void OnPressRenderButton()
    {
        controllerRef.RenderAllKeyframes();
    }

    private void Start()
    {
        rawImg.texture = controllerRef.currentRenderTexture;
    }
}
