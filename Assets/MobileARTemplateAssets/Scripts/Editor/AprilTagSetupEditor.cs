using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AprilTagSetup))]
public class AprilTagSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AprilTag Setup", EditorStyles.boldLabel);
        
        AprilTagSetup setup = (AprilTagSetup)target;
        
        if (GUILayout.Button("Setup AprilTag Detection", GUILayout.Height(30)))
        {
            setup.SetupAprilTagDetection();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Default AprilTag Prefab"))
        {
            setup.CreateDefaultAprilTagPrefab();
        }
        
        if (GUILayout.Button("Test AprilTag Detection"))
        {
            setup.TestAprilTagDetection();
        }
    }
}