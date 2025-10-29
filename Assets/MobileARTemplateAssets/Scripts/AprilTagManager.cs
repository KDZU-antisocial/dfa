using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using AprilTag;

/// <summary>
/// Manages AprilTag detection and tracking in the AR scene.
/// Handles detection events and manages AprilTag visualizations.
/// </summary>
public class AprilTagManager : MonoBehaviour
{
    [Header("AprilTag Detection")]
    [SerializeField]
    [Tooltip("The AR Camera that will be used for AprilTag detection.")]
    Camera m_ARCamera;

    /// <summary>
    /// The AR Camera that will be used for AprilTag detection.
    /// </summary>
    public Camera arCamera
    {
        get => m_ARCamera;
        set => m_ARCamera = value;
    }

    [Header("AprilTag Visualization")]
    [SerializeField]
    [Tooltip("Prefab to instantiate when an AprilTag is detected.")]
    GameObject m_AprilTagPrefab;

    /// <summary>
    /// Prefab to instantiate when an AprilTag is detected.
    /// </summary>
    public GameObject aprilTagPrefab
    {
        get => m_AprilTagPrefab;
        set => m_AprilTagPrefab = value;
    }

    [SerializeField]
    [Tooltip("Parent transform for all AprilTag visualizations.")]
    Transform m_AprilTagParent;

    /// <summary>
    /// Parent transform for all AprilTag visualizations.
    /// </summary>
    public Transform aprilTagParent
    {
        get => m_AprilTagParent;
        set => m_AprilTagParent = value;
    }

    [Header("Detection Settings")]
    [SerializeField]
    [Tooltip("Field of view for pose estimation.")]
    float m_FieldOfView = 60f;

    /// <summary>
    /// Field of view for pose estimation.
    /// </summary>
    public float fieldOfView
    {
        get => m_FieldOfView;
        set => m_FieldOfView = value;
    }

    [SerializeField]
    [Tooltip("Physical size of the AprilTag in meters.")]
    float m_TagSize = 0.1f;

    /// <summary>
    /// Physical size of the AprilTag in meters.
    /// </summary>
    public float tagSize
    {
        get => m_TagSize;
        set => m_TagSize = value;
    }

    [SerializeField]
    [Tooltip("Visual scale multiplier for the prefab (1 = normal size).")]
    float m_VisualizationScale = 1f;

    /// <summary>
    /// Visual scale multiplier for the prefab.
    /// </summary>
    public float visualizationScale
    {
        get => m_VisualizationScale;
        set => m_VisualizationScale = value;
    }

    [SerializeField]
    [Tooltip("Maximum number of AprilTags to track simultaneously.")]
    int m_MaxTrackedTags = 10;

    /// <summary>
    /// Maximum number of AprilTags to track simultaneously.
    /// </summary>
    public int maxTrackedTags
    {
        get => m_MaxTrackedTags;
        set => m_MaxTrackedTags = Mathf.Max(1, value);
    }

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Show debug information in the console.")]
    bool m_ShowDebugInfo = true;

    /// <summary>
    /// Show debug information in the console.
    /// </summary>
    public bool showDebugInfo
    {
        get => m_ShowDebugInfo;
        set => m_ShowDebugInfo = value;
    }

    // Private fields
    private TagDetector m_Detector;
    private Dictionary<int, AprilTagVisualization> m_TrackedTags = new Dictionary<int, AprilTagVisualization>();
    private List<TagPose> m_CurrentDetections = new List<TagPose>();
    private bool m_IsInitialized = false;

    /// <summary>
    /// Event fired when an AprilTag is detected for the first time.
    /// </summary>
    public System.Action<int, TagPose> OnAprilTagDetected;

    /// <summary>
    /// Event fired when an AprilTag is lost (no longer detected).
    /// </summary>
    public System.Action<int> OnAprilTagLost;

    /// <summary>
    /// Event fired when an AprilTag is updated (still detected but position changed).
    /// </summary>
    public System.Action<int, TagPose> OnAprilTagUpdated;

    void Start()
    {
        InitializeDetector();
    }

    void Update()
    {
        if (m_IsInitialized && m_ARCamera != null)
        {
            ProcessFrame();
        }
    }

    void OnDestroy()
    {
        m_Detector?.Dispose();
    }

    void InitializeDetector()
    {
        // Initialize the AR Camera if not assigned
        if (m_ARCamera == null)
        {
            m_ARCamera = Camera.main;
        }

        // Create parent for AprilTag visualizations if not assigned
        if (m_AprilTagParent == null)
        {
            GameObject parent = new GameObject("AprilTag Visualizations");
            m_AprilTagParent = parent.transform;
        }

        // Initialize the detector
        if (m_ARCamera != null)
        {
            int width = Screen.width;
            int height = Screen.height;
            m_Detector = new TagDetector(width, height);
            m_IsInitialized = true;

            if (m_ShowDebugInfo)
            {
                Debug.Log($"AprilTagManager: Initialized detector for {width}x{height} resolution");
            }
        }
        else
        {
            Debug.LogError("AprilTagManager: No AR Camera found! Please assign one in the inspector.");
        }
    }

    void ProcessFrame()
    {
        if (m_Detector == null) return;

        // Get camera render texture
        var renderTexture = m_ARCamera.targetTexture;
        if (renderTexture == null)
        {
            // Use screen capture for main camera
            var screenTexture = ScreenCapture.CaptureScreenshotAsTexture();
            if (screenTexture != null)
            {
                ProcessImage(screenTexture);
                DestroyImmediate(screenTexture);
            }
        }
        else
        {
            // Use render texture
            ProcessRenderTexture(renderTexture);
        }
    }

    void ProcessImage(Texture2D texture)
    {
        // Convert texture to Color32 array
        var pixels = texture.GetPixels32();
        m_Detector.ProcessImage(pixels, m_FieldOfView, m_TagSize);

        // Process detected tags
        ProcessDetectedTags();
    }

    void ProcessRenderTexture(RenderTexture renderTexture)
    {
        // Convert render texture to texture2D
        var texture = new Texture2D(renderTexture.width, renderTexture.height);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        ProcessImage(texture);
        DestroyImmediate(texture);
    }

    void ProcessDetectedTags()
    {
        m_CurrentDetections.Clear();
        m_CurrentDetections.AddRange(m_Detector.DetectedTags);

        // Create a set of currently detected tag IDs
        HashSet<int> currentTagIds = new HashSet<int>();
        foreach (var tagPose in m_CurrentDetections)
        {
            currentTagIds.Add(tagPose.ID);
        }

        // Check for lost tags
        List<int> lostTags = new List<int>();
        foreach (var kvp in m_TrackedTags)
        {
            if (!currentTagIds.Contains(kvp.Key))
            {
                lostTags.Add(kvp.Key);
            }
        }

        // Remove lost tags
        foreach (int tagId in lostTags)
        {
            if (m_TrackedTags.TryGetValue(tagId, out var visualization))
            {
                if (visualization != null)
                {
                    Destroy(visualization.gameObject);
                }
                m_TrackedTags.Remove(tagId);
                OnAprilTagLost?.Invoke(tagId);

                if (m_ShowDebugInfo)
                {
                    Debug.Log($"AprilTag {tagId} lost");
                }
            }
        }

        // Process current detections
        foreach (var tagPose in m_CurrentDetections)
        {
            int tagId = tagPose.ID;

            if (m_TrackedTags.ContainsKey(tagId))
            {
                // Update existing tag
                var visualization = m_TrackedTags[tagId];
                if (visualization != null)
                {
                    visualization.UpdatePosition(tagPose);
                    OnAprilTagUpdated?.Invoke(tagId, tagPose);
                }
            }
            else
            {
                // Create new tag visualization
                if (m_TrackedTags.Count < m_MaxTrackedTags)
                {
                    CreateAprilTagVisualization(tagPose);
                    OnAprilTagDetected?.Invoke(tagId, tagPose);
                }
                else if (m_ShowDebugInfo)
                {
                    Debug.LogWarning($"Maximum number of tracked AprilTags ({m_MaxTrackedTags}) reached. Ignoring new detection.");
                }
            }
        }
    }

    void CreateAprilTagVisualization(TagPose tagPose)
    {
        if (m_AprilTagPrefab == null)
        {
            Debug.LogError("AprilTagManager: No AprilTag prefab assigned!");
            return;
        }

        // Create the visualization GameObject
        GameObject tagObject = Instantiate(m_AprilTagPrefab, m_AprilTagParent);
        tagObject.name = $"AprilTag_{tagPose.ID}";

        // Get or add the visualization component
        var visualization = tagObject.GetComponent<AprilTagVisualization>();
        if (visualization == null)
        {
            visualization = tagObject.AddComponent<AprilTagVisualization>();
        }

        // Initialize the visualization
        visualization.Initialize(tagPose, m_ARCamera);
        
        // Apply visualization scale
        visualization.scale = m_VisualizationScale;

        // Add to tracked tags
        m_TrackedTags[tagPose.ID] = visualization;

        if (m_ShowDebugInfo)
        {
            Debug.Log($"AprilTag {tagPose.ID} detected at position {tagPose.Position}");
        }
    }

    /// <summary>
    /// Get all currently tracked AprilTag IDs.
    /// </summary>
    /// <returns>Array of tracked AprilTag IDs.</returns>
    public int[] GetTrackedTagIds()
    {
        int[] ids = new int[m_TrackedTags.Count];
        m_TrackedTags.Keys.CopyTo(ids, 0);
        return ids;
    }

    /// <summary>
    /// Get the visualization for a specific AprilTag ID.
    /// </summary>
    /// <param name="tagId">The AprilTag ID to look for.</param>
    /// <returns>The AprilTagVisualization component, or null if not found.</returns>
    public AprilTagVisualization GetAprilTagVisualization(int tagId)
    {
        m_TrackedTags.TryGetValue(tagId, out var visualization);
        return visualization;
    }

    /// <summary>
    /// Clear all tracked AprilTags.
    /// </summary>
    public void ClearAllAprilTags()
    {
        foreach (var kvp in m_TrackedTags)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        m_TrackedTags.Clear();
    }

    /// <summary>
    /// Enable or disable AprilTag detection.
    /// </summary>
    /// <param name="enabled">True to enable detection, false to disable.</param>
    public void SetDetectionEnabled(bool enabled)
    {
        m_IsInitialized = enabled;
    }
}