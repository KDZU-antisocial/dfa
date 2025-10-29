using UnityEngine;
using UnityEditor;

public class CreateDirectionalPrefabEditor : EditorWindow
{
    [MenuItem("Tools/Create Directional AprilTag Prefab")]
    static void CreatePrefab()
    {
        // Create materials first
        Material whiteMaterial = CreateMaterial("AprilTag_White", Color.white);
        Material greenMaterial = CreateMaterial("AprilTag_Green", Color.green);
        Material blueMaterial = CreateMaterial("AprilTag_Blue", Color.blue);
        Material redMaterial = CreateMaterial("AprilTag_Red", Color.red);
        Material yellowMaterial = CreateMaterial("AprilTag_Yellow", Color.yellow);
        Material blackMaterial = CreateMaterial("AprilTag_Black", Color.black);
        
        // Create the main object
        GameObject aprilTagObject = new GameObject("AprilTagPrefab");
        
        // 1. Base plane (white)
        GameObject basePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        basePlane.name = "Base";
        basePlane.transform.SetParent(aprilTagObject.transform);
        basePlane.transform.localPosition = Vector3.zero;
        basePlane.transform.localScale = new Vector3(0.1f, 0.1f, 0.005f);
        DestroyImmediate(basePlane.GetComponent<Collider>());
        basePlane.GetComponent<Renderer>().sharedMaterial = whiteMaterial;
        
        // 2. Border frame (black)
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = "Border";
        border.transform.SetParent(aprilTagObject.transform);
        border.transform.localPosition = new Vector3(0, 0, -0.003f);
        border.transform.localScale = new Vector3(0.11f, 0.11f, 0.002f);
        DestroyImmediate(border.GetComponent<Collider>());
        border.GetComponent<Renderer>().sharedMaterial = blackMaterial;
        
        // 3. UP ARROW (Green)
        GameObject upArrow = CreateSimpleArrow(aprilTagObject.transform, "UP_Arrow", 
            new Vector3(0, 0.035f, 0.005f), greenMaterial, Vector3.zero);
        
        // 4. FRONT indicator (Blue)
        GameObject frontIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frontIndicator.name = "FRONT";
        frontIndicator.transform.SetParent(aprilTagObject.transform);
        frontIndicator.transform.localPosition = new Vector3(0, 0, 0.01f);
        frontIndicator.transform.localRotation = Quaternion.Euler(90, 0, 0);
        frontIndicator.transform.localScale = new Vector3(0.02f, 0.005f, 0.02f);
        DestroyImmediate(frontIndicator.GetComponent<Collider>());
        frontIndicator.GetComponent<Renderer>().sharedMaterial = blueMaterial;
        
        // 5. ROTATION indicator (Red arrow)
        GameObject rotationArrow = CreateSimpleArrow(aprilTagObject.transform, "ROTATION_Arrow", 
            new Vector3(0.04f, 0, 0.005f), redMaterial, new Vector3(0, 0, -90));
        
        // 6. Corner marker (Yellow)
        GameObject cornerMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cornerMarker.name = "Corner_Marker";
        cornerMarker.transform.SetParent(aprilTagObject.transform);
        cornerMarker.transform.localPosition = new Vector3(-0.045f, -0.045f, 0.008f);
        cornerMarker.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        DestroyImmediate(cornerMarker.GetComponent<Collider>());
        cornerMarker.GetComponent<Renderer>().sharedMaterial = yellowMaterial;
        
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
        
        Debug.Log($"✅ Directional AprilTag prefab created at {fullPath}");
        Debug.Log("🟩 GREEN arrow = UP | 🔵 BLUE dot = FRONT | 🔴 RED arrow = RIGHT | 🟡 YELLOW sphere = CORNER");
        
        // Select and highlight the prefab
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    
    static Material CreateMaterial(string name, Color color)
    {
        // Check if material already exists
        string materialPath = $"Assets/MobileARTemplateAssets/Materials/{name}.mat";
        Material existingMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existingMat != null)
        {
            return existingMat;
        }
        
        // Create materials folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/MobileARTemplateAssets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets/MobileARTemplateAssets", "Materials");
        }
        
        // Create new material
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        
        // Save the material
        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();
        
        return material;
    }
    
    static GameObject CreateSimpleArrow(Transform parent, string name, Vector3 position, Material material, Vector3 rotation)
    {
        GameObject arrow = new GameObject(name);
        arrow.transform.SetParent(parent);
        arrow.transform.localPosition = position;
        arrow.transform.localRotation = Quaternion.Euler(rotation);
        
        // Arrow shaft
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.name = "Shaft";
        shaft.transform.SetParent(arrow.transform);
        shaft.transform.localPosition = new Vector3(0, -0.008f, 0);
        shaft.transform.localScale = new Vector3(0.005f, 0.008f, 0.005f);
        DestroyImmediate(shaft.GetComponent<Collider>());
        shaft.GetComponent<Renderer>().sharedMaterial = material;
        
        // Arrow head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(arrow.transform);
        head.transform.localPosition = Vector3.zero;
        head.transform.localScale = new Vector3(0.015f, 0.01f, 0.003f);
        DestroyImmediate(head.GetComponent<Collider>());
        head.GetComponent<Renderer>().sharedMaterial = material;
        
        return arrow;
    }
}
