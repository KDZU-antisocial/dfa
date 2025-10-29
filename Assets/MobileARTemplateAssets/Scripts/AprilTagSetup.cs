using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using AprilTag;

/// <summary>
/// Helper script to set up AprilTag detection in the AR scene.
/// This script can be used to automatically configure the AprilTag system.
/// </summary>
public class AprilTagSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField]
    [Tooltip("The AR Camera in the scene.")]
    Camera m_ARCamera;

    [SerializeField]
    [Tooltip("The XR Origin in the scene.")]
    XROrigin m_XROrigin;

    [SerializeField]
    [Tooltip("Whether to automatically set up AprilTag detection on Start.")]
    bool m_AutoSetup = true;

    [Header("Prefab References")]
    [SerializeField]
    [Tooltip("The AprilTag prefab to use for visualization.")]
    GameObject m_AprilTagPrefab;

    void Start()
    {
        if (m_AutoSetup)
        {
            SetupAprilTagDetection();
        }
    }

    /// <summary>
    /// Set up AprilTag detection in the scene.
    /// </summary>
    [ContextMenu("Setup AprilTag Detection")]
    public void SetupAprilTagDetection()
    {
        // Find AR Camera if not assigned
        if (m_ARCamera == null)
        {
            m_ARCamera = Camera.main;
            if (m_ARCamera == null)
            {
                m_ARCamera = FindObjectOfType<Camera>();
            }
        }

        // Find XR Origin if not assigned
        if (m_XROrigin == null)
        {
            m_XROrigin = FindObjectOfType<XROrigin>();
        }

        if (m_ARCamera == null)
        {
            Debug.LogError("AprilTagSetup: No AR Camera found! Please assign one in the inspector.");
            return;
        }

        // Create AprilTag manager if it doesn't exist
        var manager = FindObjectOfType<AprilTagManager>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject("AprilTag Manager");
            manager = managerObject.AddComponent<AprilTagManager>();
            Debug.Log("AprilTagSetup: Created AprilTag Manager.");
        }

        // Configure the manager
        manager.arCamera = m_ARCamera;
        manager.aprilTagPrefab = m_AprilTagPrefab;

        // Create parent for AprilTag visualizations
        if (manager.aprilTagParent == null)
        {
            GameObject parentObject = new GameObject("AprilTag Visualizations");
            manager.aprilTagParent = parentObject.transform;
        }

        Debug.Log("AprilTagSetup: AprilTag detection setup complete!");
    }

    /// <summary>
    /// Create a default AprilTag prefab if none is assigned.
    /// </summary>
    [ContextMenu("Create Default AprilTag Prefab")]
    public void CreateDefaultAprilTagPrefab()
    {
        if (m_AprilTagPrefab != null)
        {
            Debug.LogWarning("AprilTagSetup: AprilTag prefab already assigned.");
            return;
        }

        // Create a simple cube prefab
        GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefab.name = "AprilTagPrefab";
        
        // Add the visualization component
        var visualization = prefab.AddComponent<AprilTagVisualization>();
        
        // Create a frame
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Frame";
        frame.transform.SetParent(prefab.transform);
        frame.transform.localPosition = Vector3.zero;
        frame.transform.localScale = new Vector3(0.95f, 0.95f, 0.1f);
        
        // Make the frame red
        var frameRenderer = frame.GetComponent<Renderer>();
        var frameMaterial = new Material(Shader.Find("Standard"));
        frameMaterial.color = Color.red;
        frameRenderer.sharedMaterial = frameMaterial;
        
        // Make the main cube white
        var mainRenderer = prefab.GetComponent<Renderer>();
        var mainMaterial = new Material(Shader.Find("Standard"));
        mainMaterial.color = Color.white;
        mainRenderer.sharedMaterial = mainMaterial;

        // Save as prefab (this would need to be done manually in the editor)
        m_AprilTagPrefab = prefab;
        
        Debug.Log("AprilTagSetup: Created default AprilTag prefab. Please save it as a prefab in the Prefabs folder.");
    }

    /// <summary>
    /// Test AprilTag detection by creating a test tag.
    /// </summary>
    [ContextMenu("Test AprilTag Detection")]
    public void TestAprilTagDetection()
    {
        var manager = FindObjectOfType<AprilTagManager>();
        if (manager == null)
        {
            Debug.LogError("AprilTagSetup: No AprilTag Manager found! Run setup first.");
            return;
        }

        // Enable detection
        manager.SetDetectionEnabled(true);
        
        Debug.Log("AprilTagSetup: AprilTag detection enabled. Point the camera at an AprilTag to test detection.");
    }
}