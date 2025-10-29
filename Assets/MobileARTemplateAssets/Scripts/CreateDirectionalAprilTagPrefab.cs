using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Creates an AprilTag prefab with clear directional indicators.
/// Shows up/down direction and rotation clearly.
/// </summary>
public class CreateDirectionalAprilTagPrefab : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/AprilTag/Create Directional AprilTag Prefab")]
    static void CreateDirectionalPrefab()
    {
        // Create the main object
        GameObject aprilTagObject = new GameObject("AprilTagPrefab");
        
        // 1. Base plane (white)
        GameObject basePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        basePlane.name = "Base";
        basePlane.transform.SetParent(aprilTagObject.transform);
        basePlane.transform.localPosition = Vector3.zero;
        basePlane.transform.localScale = new Vector3(0.1f, 0.1f, 0.005f);
        Material baseMaterial = new Material(Shader.Find("Standard"));
        baseMaterial.color = Color.white;
        basePlane.GetComponent<Renderer>().sharedMaterial = baseMaterial;
        
        // 2. Border frame (black)
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = "Border";
        border.transform.SetParent(aprilTagObject.transform);
        border.transform.localPosition = new Vector3(0, 0, -0.003f);
        border.transform.localScale = new Vector3(0.11f, 0.11f, 0.002f);
        Material borderMaterial = new Material(Shader.Find("Standard"));
        borderMaterial.color = Color.black;
        border.GetComponent<Renderer>().sharedMaterial = borderMaterial;
        
        // 3. UP ARROW (Green) - Top of the tag
        GameObject upArrow = CreateArrow(aprilTagObject.transform, "UP Arrow", 
            new Vector3(0, 0.035f, 0.005f), Color.green, Vector3.zero);
        
        // 4. FRONT indicator (Blue cylinder) - Shows which side is front
        GameObject frontIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frontIndicator.name = "FRONT";
        frontIndicator.transform.SetParent(aprilTagObject.transform);
        frontIndicator.transform.localPosition = new Vector3(0, 0, 0.01f);
        frontIndicator.transform.localRotation = Quaternion.Euler(90, 0, 0);
        frontIndicator.transform.localScale = new Vector3(0.02f, 0.005f, 0.02f);
        Material frontMaterial = new Material(Shader.Find("Standard"));
        frontMaterial.color = Color.blue;
        frontIndicator.GetComponent<Renderer>().sharedMaterial = frontMaterial;
        
        // 5. ROTATION indicator (Red arrow pointing right) - Shows rotation direction
        GameObject rotationArrow = CreateArrow(aprilTagObject.transform, "ROTATION Arrow", 
            new Vector3(0.04f, 0, 0.005f), Color.red, new Vector3(0, 0, -90));
        
        // 6. Corner marker (Yellow) - Reference point
        GameObject cornerMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cornerMarker.name = "Corner Marker";
        cornerMarker.transform.SetParent(aprilTagObject.transform);
        cornerMarker.transform.localPosition = new Vector3(-0.045f, -0.045f, 0.008f);
        cornerMarker.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        Material cornerMaterial = new Material(Shader.Find("Standard"));
        cornerMaterial.color = Color.yellow;
        cornerMarker.GetComponent<Renderer>().sharedMaterial = cornerMaterial;
        
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
        
        Debug.Log($"Directional AprilTag prefab created successfully at {fullPath}");
        Debug.Log("Visual Guide: GREEN arrow = UP, BLUE dot = FRONT, RED arrow = ROTATION, YELLOW sphere = CORNER");
        
        // Select the prefab in the project view
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    
    static GameObject CreateArrow(Transform parent, string name, Vector3 position, Color color, Vector3 rotation)
    {
        GameObject arrow = new GameObject(name);
        arrow.transform.SetParent(parent);
        arrow.transform.localPosition = position;
        arrow.transform.localRotation = Quaternion.Euler(rotation);
        
        // Arrow shaft (thin cylinder)
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.name = "Shaft";
        shaft.transform.SetParent(arrow.transform);
        shaft.transform.localPosition = new Vector3(0, -0.008f, 0);
        shaft.transform.localScale = new Vector3(0.005f, 0.008f, 0.005f);
        Material shaftMaterial = new Material(Shader.Find("Standard"));
        shaftMaterial.color = color;
        shaft.GetComponent<Renderer>().sharedMaterial = shaftMaterial;
        
        // Arrow head (cone)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(arrow.transform);
        head.transform.localPosition = Vector3.zero;
        head.transform.localScale = new Vector3(0.015f, 0.01f, 0.003f);
        Material headMaterial = new Material(Shader.Find("Standard"));
        headMaterial.color = color;
        head.GetComponent<Renderer>().sharedMaterial = headMaterial;
        
        return arrow;
    }
#endif
}
