using UnityEngine;
using UnityEditor;

public class SimpleAprilTagPrefabCreator : EditorWindow
{
    [MenuItem("Tools/FIX Pink Prefab - Create Simple AprilTag")]
    static void CreateSimplePrefab()
    {
        // Try to find URP Lit shader first, fallback to Standard
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Diffuse");
        }
        
        Debug.Log($"Using shader: {shader.name}");
        
        // Create materials with the correct shader
        Material whiteMaterial = new Material(shader);
        whiteMaterial.color = Color.white;
        
        Material greenMaterial = new Material(shader);
        greenMaterial.color = Color.green;
        
        Material blueMaterial = new Material(shader);
        blueMaterial.color = Color.blue;
        
        Material redMaterial = new Material(shader);
        redMaterial.color = Color.red;
        
        Material yellowMaterial = new Material(shader);
        yellowMaterial.color = Color.yellow;
        
        // Save materials
        string matPath = "Assets/MobileARTemplateAssets/Materials";
        if (!AssetDatabase.IsValidFolder(matPath))
        {
            AssetDatabase.CreateFolder("Assets/MobileARTemplateAssets", "Materials");
        }
        
        AssetDatabase.CreateAsset(whiteMaterial, $"{matPath}/White_URP.mat");
        AssetDatabase.CreateAsset(greenMaterial, $"{matPath}/Green_URP.mat");
        AssetDatabase.CreateAsset(blueMaterial, $"{matPath}/Blue_URP.mat");
        AssetDatabase.CreateAsset(redMaterial, $"{matPath}/Red_URP.mat");
        AssetDatabase.CreateAsset(yellowMaterial, $"{matPath}/Yellow_URP.mat");
        AssetDatabase.SaveAssets();
        
        // Reload materials from disk to ensure they're valid
        whiteMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{matPath}/White_URP.mat");
        greenMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{matPath}/Green_URP.mat");
        blueMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{matPath}/Blue_URP.mat");
        redMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{matPath}/Red_URP.mat");
        yellowMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{matPath}/Yellow_URP.mat");
        
        // Create the prefab
        GameObject root = new GameObject("AprilTagPrefab");
        
        // Base (white square)
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObj.name = "Base";
        baseObj.transform.SetParent(root.transform);
        baseObj.transform.localPosition = Vector3.zero;
        baseObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.005f);
        DestroyImmediate(baseObj.GetComponent<Collider>());
        baseObj.GetComponent<Renderer>().sharedMaterial = whiteMaterial;
        
        // UP indicator (green cube)
        GameObject upObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upObj.name = "UP_Green";
        upObj.transform.SetParent(root.transform);
        upObj.transform.localPosition = new Vector3(0, 0.04f, 0.006f);
        upObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.004f);
        DestroyImmediate(upObj.GetComponent<Collider>());
        upObj.GetComponent<Renderer>().sharedMaterial = greenMaterial;
        
        // FRONT indicator (blue sphere)
        GameObject frontObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        frontObj.name = "FRONT_Blue";
        frontObj.transform.SetParent(root.transform);
        frontObj.transform.localPosition = new Vector3(0, 0, 0.01f);
        frontObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        DestroyImmediate(frontObj.GetComponent<Collider>());
        frontObj.GetComponent<Renderer>().sharedMaterial = blueMaterial;
        
        // RIGHT indicator (red cube)
        GameObject rightObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightObj.name = "RIGHT_Red";
        rightObj.transform.SetParent(root.transform);
        rightObj.transform.localPosition = new Vector3(0.04f, 0, 0.006f);
        rightObj.transform.localScale = new Vector3(0.02f, 0.02f, 0.004f);
        DestroyImmediate(rightObj.GetComponent<Collider>());
        rightObj.GetComponent<Renderer>().sharedMaterial = redMaterial;
        
        // CORNER indicator (yellow sphere)
        GameObject cornerObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cornerObj.name = "CORNER_Yellow";
        cornerObj.transform.SetParent(root.transform);
        cornerObj.transform.localPosition = new Vector3(-0.04f, -0.04f, 0.01f);
        cornerObj.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        DestroyImmediate(cornerObj.GetComponent<Collider>());
        cornerObj.GetComponent<Renderer>().sharedMaterial = yellowMaterial;
        
        // Add visualization component
        root.AddComponent<AprilTagVisualization>();
        
        // Save as prefab
        string prefabPath = "Assets/MobileARTemplateAssets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/MobileARTemplateAssets", "Prefabs");
        }
        
        string fullPath = $"{prefabPath}/AprilTagPrefab.prefab";
        
        // Delete old prefab if exists
        if (AssetDatabase.LoadAssetAtPath<GameObject>(fullPath) != null)
        {
            AssetDatabase.DeleteAsset(fullPath);
        }
        
        PrefabUtility.SaveAsPrefabAsset(root, fullPath);
        DestroyImmediate(root);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ PREFAB CREATED!");
        Debug.Log("🟩 Green cube = UP");
        Debug.Log("🔵 Blue sphere = FRONT");  
        Debug.Log("🔴 Red cube = RIGHT");
        Debug.Log("🟡 Yellow sphere = CORNER");
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
}
