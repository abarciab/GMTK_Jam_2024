using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class FogSceneViewControl
{
    static FogSceneViewControl()
    {
        // Called when Unity loads or recompiles scripts
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        // Check if fog is enabled and the scene view is set to show it
        bool fogEnabled = RenderSettings.fog && sceneView.sceneViewState.showFog;

        // Set a global float shader property
        Shader.SetGlobalFloat("_ApplyFog", fogEnabled ? 1 : 0);
    }
}
