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
    float m_VisualizationScale = 2f;

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

    [SerializeField]
    [Tooltip("Number of frames to wait before removing a lost AprilTag (prevents flickering).")]
    int m_LostTagPersistenceFrames = 30;

    /// <summary>
    /// Number of frames to wait before removing a lost AprilTag.
    /// </summary>
    public int lostTagPersistenceFrames
    {
        get => m_LostTagPersistenceFrames;
        set => m_LostTagPersistenceFrames = Mathf.Max(0, value);
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

    [SerializeField]
    [Tooltip("Show detailed frame-by-frame logging.")]
    bool m_ShowDetailedLogging = false;

    /// <summary>
    /// Show detailed frame-by-frame logging.
    /// </summary>
    public bool showDetailedLogging
    {
        get => m_ShowDetailedLogging;
        set => m_ShowDetailedLogging = value;
    }

    [SerializeField]
    [Tooltip("Anchor the world origin to the first detected AprilTag (tag stays at fixed world position).")]
    bool m_AnchorWorldToFirstTag = true;

    /// <summary>
    /// Anchor the world origin to the first detected AprilTag.
    /// </summary>
    public bool anchorWorldToFirstTag
    {
        get => m_AnchorWorldToFirstTag;
        set => m_AnchorWorldToFirstTag = value;
    }

    [SerializeField]
    [Tooltip("Spawn a debug marker at world origin (0,0,0).")]
    bool m_ShowOriginMarker = false;

    /// <summary>
    /// Spawn a debug marker at world origin (0,0,0).
    /// </summary>
    public bool showOriginMarker
    {
        get => m_ShowOriginMarker;
        set
        {
            m_ShowOriginMarker = value;
            UpdateOriginMarker();
        }
    }

    [SerializeField]
    [Tooltip("Height offset for the origin marker (in meters).")]
    float m_OriginMarkerHeight = 0.1f;

    /// <summary>
    /// Height offset for the origin marker (in meters).
    /// </summary>
    public float originMarkerHeight
    {
        get => m_OriginMarkerHeight;
        set
        {
            m_OriginMarkerHeight = value;
            UpdateOriginMarkerPosition();
        }
    }

    [SerializeField]
    [Tooltip("Show a debug marker at the camera's position.")]
    bool m_ShowCameraMarker = false;

    /// <summary>
    /// Show a debug marker at the camera's position.
    /// </summary>
    public bool showCameraMarker
    {
        get => m_ShowCameraMarker;
        set
        {
            m_ShowCameraMarker = value;
            UpdateCameraMarker();
        }
    }

    // Private fields
    private TagDetector m_Detector;
    private Dictionary<int, AprilTagVisualization> m_TrackedTags = new Dictionary<int, AprilTagVisualization>();
    private Dictionary<int, int> m_LostTagFrameCount = new Dictionary<int, int>(); // Tracks frames since tag was lost
    private Dictionary<int, Vector3> m_TagWorldPositions = new Dictionary<int, Vector3>(); // Stores fixed world positions for anchored tags
    private Dictionary<int, Quaternion> m_TagWorldRotations = new Dictionary<int, Quaternion>(); // Stores fixed world rotations for anchored tags
    private List<TagPose> m_CurrentDetections = new List<TagPose>();
    private bool m_IsInitialized = false;
    private GameObject m_OriginMarkerObject;
    private GameObject m_CameraMarkerObject;

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
        UpdateOriginMarker();
        UpdateCameraMarker();
    }

    void Update()
    {
        if (m_IsInitialized && m_ARCamera != null)
        {
            ProcessFrame();
            
            // Update camera marker position every frame (1m in front of camera so it's visible)
            if (m_ShowCameraMarker && m_CameraMarkerObject != null && m_ARCamera != null)
            {
                m_CameraMarkerObject.transform.position = m_ARCamera.transform.position + m_ARCamera.transform.forward * 1.0f;
                m_CameraMarkerObject.transform.rotation = m_ARCamera.transform.rotation;
            }
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

        if (m_ShowDetailedLogging && m_CurrentDetections.Count > 0)
        {
            Debug.Log($"[AprilTagMgr] Processing {m_CurrentDetections.Count} detected tags");
        }

        // Create a set of currently detected tag IDs
        HashSet<int> currentTagIds = new HashSet<int>();
        foreach (var tagPose in m_CurrentDetections)
        {
            currentTagIds.Add(tagPose.ID);
            
            if (m_ShowDetailedLogging)
            {
                Debug.Log($"[AprilTagMgr] Detected Tag {tagPose.ID}:");
                Debug.Log($"  Position: {tagPose.Position}");
                Debug.Log($"  Rotation: {tagPose.Rotation.eulerAngles}");
            }
        }

        // Check for lost tags and update persistence counters
        List<int> tagsToRemove = new List<int>();
        foreach (var kvp in m_TrackedTags)
        {
            int tagId = kvp.Key;
            
            if (!currentTagIds.Contains(tagId))
            {
                // Tag was not detected this frame
                if (!m_LostTagFrameCount.ContainsKey(tagId))
                {
                    m_LostTagFrameCount[tagId] = 0;
                    if (m_ShowDebugInfo)
                    {
                        Debug.Log($"AprilTag {tagId} lost - will persist for {m_LostTagPersistenceFrames} frames");
                    }
                }
                
                m_LostTagFrameCount[tagId]++;
                
                // Only remove after persistence timeout
                if (m_LostTagFrameCount[tagId] >= m_LostTagPersistenceFrames)
                {
                    tagsToRemove.Add(tagId);
                }
            }
            else
            {
                // Tag was detected - reset lost counter if it exists
                if (m_LostTagFrameCount.ContainsKey(tagId))
                {
                    m_LostTagFrameCount.Remove(tagId);
                }
            }
        }

        // Remove tags that have been lost for too long
        foreach (int tagId in tagsToRemove)
        {
            if (m_TrackedTags.TryGetValue(tagId, out var visualization))
            {
                if (visualization != null)
                {
                    Destroy(visualization.gameObject);
                }
                m_TrackedTags.Remove(tagId);
                m_LostTagFrameCount.Remove(tagId);
                OnAprilTagLost?.Invoke(tagId);

                if (m_ShowDebugInfo)
                {
                    Debug.Log($"AprilTag {tagId} removed after {m_LostTagPersistenceFrames} frames");
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
                    if (m_ShowDetailedLogging)
                    {
                        Debug.Log($"[AprilTagMgr] Updating existing tag {tagId}");
                    }
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
        GameObject tagObject;
        
        // If no prefab is assigned or prefab doesn't have renderers, create a fallback visualization
        if (m_AprilTagPrefab == null || !HasVisibleRenderers(m_AprilTagPrefab))
        {
            if (m_ShowDebugInfo && m_AprilTagPrefab == null)
            {
                Debug.LogWarning("AprilTagManager: No AprilTag prefab assigned! Creating fallback visualization.");
            }
            
            tagObject = CreateFallbackVisualization();
            tagObject.transform.SetParent(m_AprilTagParent);
        }
        else
        {
            // Create the visualization GameObject from prefab
            tagObject = Instantiate(m_AprilTagPrefab, m_AprilTagParent);
            
            // Make sure all materials on the prefab are bright and visible
            MakeVisualizationVisible(tagObject);
        }
        
        tagObject.name = $"AprilTag_{tagPose.ID}";

        // Get or add the visualization component
        var visualization = tagObject.GetComponent<AprilTagVisualization>();
        if (visualization == null)
        {
            visualization = tagObject.AddComponent<AprilTagVisualization>();
        }

        // Calculate initial world position
        Vector3 initialWorldPosition = m_ARCamera.transform.TransformPoint(tagPose.Position);
        Quaternion initialWorldRotation = m_ARCamera.transform.rotation * tagPose.Rotation;
        
        // If anchoring is enabled, use fixed world positioning
        if (m_AnchorWorldToFirstTag)
        {
            // Check if we already have an anchored position for this tag ID
            if (m_TagWorldPositions.ContainsKey(tagPose.ID))
            {
                // Reuse the existing anchored position
                initialWorldPosition = m_TagWorldPositions[tagPose.ID];
                initialWorldRotation = m_TagWorldRotations[tagPose.ID];
                
                if (m_ShowDebugInfo)
                {
                    Debug.Log($"[AprilTagMgr] Tag {tagPose.ID} RE-USING EXISTING anchor at: {initialWorldPosition}");
                }
            }
            else
            {
                // First time seeing this tag - store the anchor position
                m_TagWorldPositions[tagPose.ID] = initialWorldPosition;
                m_TagWorldRotations[tagPose.ID] = initialWorldRotation;
                
                if (m_ShowDebugInfo)
                {
                    Debug.Log($"[AprilTagMgr] Tag {tagPose.ID} FIRST ANCHOR at fixed world position: {initialWorldPosition}");
                    Debug.Log($"[AprilTagMgr] Camera was at: {m_ARCamera.transform.position}");
                    Debug.Log($"[AprilTagMgr] Tag is {Vector3.Distance(initialWorldPosition, m_ARCamera.transform.position):F4}m from camera");
                }
            }
            
            // Tell the visualization to use fixed world position (no updates)
            visualization.useFixedWorldPosition = true;
            visualization.fixedWorldPosition = initialWorldPosition;
            visualization.fixedWorldRotation = initialWorldRotation;
        }

        // Initialize the visualization (this will set the initial position)
        visualization.Initialize(tagPose, m_ARCamera);
        
        // Apply visualization scale (enforce minimum to ensure visibility)
        float safeScale = Mathf.Max(m_VisualizationScale, 0.5f);
        if (safeScale != m_VisualizationScale && m_ShowDebugInfo)
        {
            Debug.LogWarning($"AprilTagManager: Visualization scale was too small ({m_VisualizationScale}), using minimum safe scale ({safeScale})");
        }
        visualization.scale = safeScale;

        // Add to tracked tags
        m_TrackedTags[tagPose.ID] = visualization;

        if (m_ShowDebugInfo)
        {
            Debug.Log($"AprilTag {tagPose.ID} detected at position {tagPose.Position}");
        }
    }
    
    /// <summary>
    /// Make all renderers on a GameObject bright and visible using unlit shaders.
    /// </summary>
    void MakeVisualizationVisible(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer.material != null)
            {
                // Try to find a suitable unlit shader with fallbacks
                Shader shader = Shader.Find("Unlit/Color") ?? 
                               Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                               Shader.Find("Sprites/Default") ??
                               Shader.Find("UI/Default");
                
                if (shader != null)
                {
                    // Get the current color
                    Color currentColor = renderer.material.color;
                    
                    // Make it brighter if it's too dark
                    if (currentColor.r + currentColor.g + currentColor.b < 0.5f)
                    {
                        currentColor = new Color(
                            Mathf.Max(currentColor.r, 0.5f),
                            Mathf.Max(currentColor.g, 0.5f),
                            Mathf.Max(currentColor.b, 0.5f),
                            1f
                        );
                    }
                    
                    // Create a new material with the unlit shader
                    Material mat = new Material(shader);
                    mat.color = currentColor;
                    renderer.material = mat;
                }
            }
        }
        
        if (m_ShowDebugInfo)
        {
            Debug.Log($"Made {renderers.Length} renderers visible with unlit shaders");
        }
    }
    
    /// <summary>
    /// Check if a GameObject has visible renderers.
    /// </summary>
    bool HasVisibleRenderers(GameObject obj)
    {
        if (obj == null) return false;
        var renderers = obj.GetComponentsInChildren<Renderer>();
        return renderers != null && renderers.Length > 0;
    }
    
    /// <summary>
    /// Create a fallback visualization when no prefab is assigned.
    /// </summary>
    GameObject CreateFallbackVisualization()
    {
        // Create a parent object
        GameObject tagObject = new GameObject("AprilTagVisualization");
        
        // Create a bright green cube as the main tag
        GameObject mainCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainCube.name = "TagPlane";
        mainCube.transform.SetParent(tagObject.transform);
        mainCube.transform.localPosition = Vector3.zero;
        mainCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f); // 10cm flat square
        
        // Remove collider
        var collider1 = mainCube.GetComponent<Collider>();
        if (collider1 != null) Destroy(collider1);
        
        // Make it bright green with unlit shader
        var mainRenderer = mainCube.GetComponent<Renderer>();
        if (mainRenderer != null)
        {
            Shader shader = Shader.Find("Unlit/Color") ?? 
                           Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                           Shader.Find("Sprites/Default") ??
                           Shader.Find("UI/Default");
            
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.color = new Color(0f, 1f, 0f, 1f); // Bright green
                mainRenderer.material = mat;
            }
            else
            {
                mainRenderer.material.color = new Color(0f, 1f, 0f, 1f); // Bright green fallback
            }
        }
        
        // Create a bright magenta border frame
        GameObject borderFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        borderFrame.name = "Border";
        borderFrame.transform.SetParent(tagObject.transform);
        borderFrame.transform.localPosition = new Vector3(0, 0, -0.005f); // Slightly behind
        borderFrame.transform.localScale = new Vector3(0.12f, 0.12f, 0.005f); // 12cm border
        
        // Remove collider
        var collider2 = borderFrame.GetComponent<Collider>();
        if (collider2 != null) Destroy(collider2);
        
        // Make it bright magenta with unlit shader
        var borderRenderer = borderFrame.GetComponent<Renderer>();
        if (borderRenderer != null)
        {
            Shader shader = Shader.Find("Unlit/Color") ?? 
                           Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                           Shader.Find("Sprites/Default") ??
                           Shader.Find("UI/Default");
            
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.color = new Color(1f, 0f, 1f, 1f); // Bright magenta
                borderRenderer.material = mat;
            }
            else
            {
                borderRenderer.material.color = new Color(1f, 0f, 1f, 1f); // Bright magenta fallback
            }
        }
        
        if (m_ShowDebugInfo)
        {
            Debug.Log("Created fallback AprilTag visualization (bright green with magenta border)");
        }
        
        return tagObject;
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

    /// <summary>
    /// Create or destroy the origin marker based on the showOriginMarker flag.
    /// </summary>
    void UpdateOriginMarker()
    {
        if (m_ShowOriginMarker && m_OriginMarkerObject == null)
        {
            CreateOriginMarker();
        }
        else if (!m_ShowOriginMarker && m_OriginMarkerObject != null)
        {
            Destroy(m_OriginMarkerObject);
            m_OriginMarkerObject = null;
        }
    }

    /// <summary>
    /// Update the position of the origin marker.
    /// </summary>
    void UpdateOriginMarkerPosition()
    {
        if (m_OriginMarkerObject != null)
        {
            m_OriginMarkerObject.transform.position = new Vector3(0, m_OriginMarkerHeight, 0);
        }
    }

    /// <summary>
    /// Create a debug marker at the world origin.
    /// </summary>
    void CreateOriginMarker()
    {
        // Create a small cube at the origin
        m_OriginMarkerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_OriginMarkerObject.name = "Debug_OriginMarker";
        m_OriginMarkerObject.transform.position = new Vector3(0, m_OriginMarkerHeight, 0);
        m_OriginMarkerObject.transform.localScale = Vector3.one * 0.1f; // 10cm cube - larger for visibility
        
        // Remove collider - we don't need physics for debug markers
        var collider = m_OriginMarkerObject.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        
        // Make it bright red and visible
        var renderer = m_OriginMarkerObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Try to find a suitable unlit shader with fallbacks
            Shader shader = Shader.Find("Unlit/Color") ?? 
                           Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                           Shader.Find("Sprites/Default") ??
                           Shader.Find("UI/Default");
            
            if (shader != null)
            {
                // Create a new material with the found shader
                Material mat = new Material(shader);
                mat.color = new Color(1f, 0f, 0f, 1f); // Bright red
                renderer.material = mat;
            }
            else
            {
                // Fallback: just modify the existing material's color
                renderer.material.color = new Color(1f, 0f, 0f, 1f); // Bright red
            }
        }

        if (m_ShowDebugInfo)
        {
            Debug.Log($"Origin marker created at (0, {m_OriginMarkerHeight}, 0)");
        }
    }

    /// <summary>
    /// Create or destroy the camera marker based on the showCameraMarker flag.
    /// </summary>
    void UpdateCameraMarker()
    {
        if (m_ShowCameraMarker && m_CameraMarkerObject == null)
        {
            CreateCameraMarker();
        }
        else if (!m_ShowCameraMarker && m_CameraMarkerObject != null)
        {
            Destroy(m_CameraMarkerObject);
            m_CameraMarkerObject = null;
        }
    }

    /// <summary>
    /// Create a debug marker at the camera position.
    /// </summary>
    void CreateCameraMarker()
    {
        if (m_ARCamera == null)
        {
            Debug.LogWarning("Cannot create camera marker - AR Camera is null");
            return;
        }

        // Create a sphere offset from the camera so it's visible
        m_CameraMarkerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_CameraMarkerObject.name = "Debug_CameraMarker";
        
        // Position it 1 meter in front of the camera so you can actually see it
        m_CameraMarkerObject.transform.position = m_ARCamera.transform.position + m_ARCamera.transform.forward * 1.0f;
        m_CameraMarkerObject.transform.localScale = Vector3.one * 0.15f; // 15cm sphere - larger for visibility
        
        // Remove collider - we don't need physics for debug markers
        var collider = m_CameraMarkerObject.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        
        // Make it bright cyan and visible
        var renderer = m_CameraMarkerObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Try to find a suitable unlit shader with fallbacks
            Shader shader = Shader.Find("Unlit/Color") ?? 
                           Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                           Shader.Find("Sprites/Default") ??
                           Shader.Find("UI/Default");
            
            if (shader != null)
            {
                // Create a new material with the found shader
                Material mat = new Material(shader);
                mat.color = new Color(0f, 1f, 1f, 1f); // Bright cyan
                renderer.material = mat;
            }
            else
            {
                // Fallback: just modify the existing material's color
                renderer.material.color = new Color(0f, 1f, 1f, 1f); // Bright cyan
            }
        }

        // Add a direction indicator to show camera forward
        CreateCameraDirectionIndicator();

        if (m_ShowDebugInfo)
        {
            Debug.Log($"Camera marker created at {m_CameraMarkerObject.transform.position} (1m in front of camera)");
        }
    }

    /// <summary>
    /// Create a visual indicator showing the camera's forward direction.
    /// </summary>
    void CreateCameraDirectionIndicator()
    {
        if (m_CameraMarkerObject == null) return;

        // Create an elongated cube pointing forward to show camera direction
        GameObject directionMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        directionMarker.name = "CameraDirection";
        directionMarker.transform.SetParent(m_CameraMarkerObject.transform);
        directionMarker.transform.localPosition = new Vector3(0, 0, 0.3f); // 30cm in front
        directionMarker.transform.localRotation = Quaternion.identity;
        directionMarker.transform.localScale = new Vector3(0.03f, 0.03f, 0.5f); // Thin elongated cube - more visible
        
        // Remove collider - we don't need physics for debug markers
        var collider = directionMarker.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        
        var renderer = directionMarker.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Try to find a suitable unlit shader with fallbacks
            Shader shader = Shader.Find("Unlit/Color") ?? 
                           Shader.Find("Mobile/Unlit (Supports Lightmap)") ?? 
                           Shader.Find("Sprites/Default") ??
                           Shader.Find("UI/Default");
            
            if (shader != null)
            {
                // Create a new material with the found shader
                Material mat = new Material(shader);
                mat.color = new Color(1f, 1f, 0f, 1f); // Bright yellow
                renderer.material = mat;
            }
            else
            {
                // Fallback: just modify the existing material's color
                renderer.material.color = new Color(1f, 1f, 0f, 1f); // Bright yellow
            }
        }
    }
}