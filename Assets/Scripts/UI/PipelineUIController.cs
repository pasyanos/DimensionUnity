using UnityEngine;

public class PipelineUIController : MonoBehaviour
{
    [SerializeField] private SpritePipelineController controllerRef;
    
    public void OnPressRenderButton()
    {
        controllerRef.RenderToSprite();
    }
}
