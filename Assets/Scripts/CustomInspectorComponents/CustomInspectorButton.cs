using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpritePipelineController))]
public class CustomInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpritePipelineController controller = (SpritePipelineController)target;

        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox("Multi object editing not supported", MessageType.Info);
            return;
        }

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Run Pipeline"))
        {
            controller.OnGUIButton();
        }

        GUI.enabled = true;
    }
}
