using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to create a proper AprilTag prefab.
/// Run this in the Unity Editor to generate the prefab.
/// </summary>
public class CreateAprilTagPrefab : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/AprilTag/Create AprilTag Prefab")]
    static void CreatePrefab()
    {
        // Create the main object
        GameObject aprilTagObject = new GameObject("AprilTagPrefab");
        
        // Add a cube as the main visual
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "TagPlane";
        cube.transform.SetParent(aprilTagObject.transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f); // Thin flat square
        
        // Create material for the cube
        Material cubeMaterial = new Material(Shader.Find("Standard"));
        cubeMaterial.color = Color.white;
        cube.GetComponent<Renderer>().sharedMaterial = cubeMaterial;
        
        // Create a frame border
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Border";
        frame.transform.SetParent(aprilTagObject.transform);
        frame.transform.localPosition = Vector3.zero;
        frame.transform.localScale = new Vector3(0.12f, 0.12f, 0.005f); // Slightly larger, thinner
        
        // Create material for the frame
        Material frameMaterial = new Material(Shader.Find("Standard"));
        frameMaterial.color = new Color(1f, 0.3f, 0f); // Orange border
        frame.GetComponent<Renderer>().sharedMaterial = frameMaterial;
        
        // Add the AprilTagVisualization component
        aprilTagObject.AddComponent<AprilTagVisualization>();
        
        // Ensure the Prefabs directory exists
        string prefabPath = "Assets/MobileARTemplateAssets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/MobileARTemplateAssets", "Prefabs");
        }
        
        // Save as prefab
        string fullPath = prefabPath + "/AprilTagPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(aprilTagObject, fullPath);
        
        // Clean up the scene object
        DestroyImmediate(aprilTagObject);
        
        Debug.Log($"AprilTag prefab created successfully at {fullPath}");
        
        // Select the prefab in the project view
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
#endif
}
