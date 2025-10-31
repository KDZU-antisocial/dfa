using UnityEngine;
using AprilTag;

/// <summary>
/// Handles the visualization and positioning of a detected AprilTag.
/// Updates the GameObject's transform based on the AprilTag's pose in the camera view.
/// </summary>
public class AprilTagVisualization : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField]
    [Tooltip("Scale factor for the AprilTag visualization.")]
    float m_Scale = 1f;

    /// <summary>
    /// Scale factor for the AprilTag visualization.
    /// </summary>
    public float scale
    {
        get => m_Scale;
        set => m_Scale = value;
    }

    [SerializeField]
    [Tooltip("Whether to show the AprilTag ID as text.")]
    bool m_ShowTagId = false;

    /// <summary>
    /// Whether to show the AprilTag ID as text.
    /// </summary>
    public bool showTagId
    {
        get => m_ShowTagId;
        set => m_ShowTagId = value;
    }

    [SerializeField]
    [Tooltip("Text component to display the AprilTag ID.")]
    UnityEngine.UI.Text m_TagIdText;

    /// <summary>
    /// Text component to display the AprilTag ID.
    /// </summary>
    public UnityEngine.UI.Text tagIdText
    {
        get => m_TagIdText;
        set => m_TagIdText = value;
    }

    [SerializeField]
    [Tooltip("Canvas for displaying tag information.")]
    Canvas m_TagCanvas;

    /// <summary>
    /// Canvas for displaying tag information.
    /// </summary>
    public Canvas tagCanvas
    {
        get => m_TagCanvas;
        set => m_TagCanvas = value;
    }

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Enable detailed logging for debugging.")]
    bool m_EnableLogging = true;

    /// <summary>
    /// Enable detailed logging for debugging.
    /// </summary>
    public bool enableLogging
    {
        get => m_EnableLogging;
        set => m_EnableLogging = value;
    }

    // Private fields
    private TagPose m_CurrentTagPose;
    private Camera m_ARCamera;
    private int m_TagId;
    private bool m_IsInitialized = false;
    private Vector3 m_LastPosition;
    private Quaternion m_LastRotation;
    
    // Fixed world positioning (for anchored tags)
    private bool m_UseFixedWorldPosition = false;
    private Vector3 m_FixedWorldPosition;
    private Quaternion m_FixedWorldRotation;

    /// <summary>
    /// Whether to use a fixed world position (tag doesn't update with new detections).
    /// </summary>
    public bool useFixedWorldPosition
    {
        get => m_UseFixedWorldPosition;
        set => m_UseFixedWorldPosition = value;
    }

    /// <summary>
    /// The fixed world position for this tag (when useFixedWorldPosition is true).
    /// </summary>
    public Vector3 fixedWorldPosition
    {
        get => m_FixedWorldPosition;
        set => m_FixedWorldPosition = value;
    }

    /// <summary>
    /// The fixed world rotation for this tag (when useFixedWorldPosition is true).
    /// </summary>
    public Quaternion fixedWorldRotation
    {
        get => m_FixedWorldRotation;
        set => m_FixedWorldRotation = value;
    }

    /// <summary>
    /// The current AprilTag pose data.
    /// </summary>
    public TagPose currentTagPose => m_CurrentTagPose;

    /// <summary>
    /// The AprilTag ID.
    /// </summary>
    public int tagId => m_TagId;

    /// <summary>
    /// Whether this visualization is initialized.
    /// </summary>
    public bool isInitialized => m_IsInitialized;

    void Start()
    {
        // Set up the tag ID text if not already assigned
        if (m_TagIdText == null && m_ShowTagId)
        {
            SetupTagIdText();
        }
    }

    void Update()
    {
        // Don't update in Update() - only update when AprilTagManager calls UpdatePosition
        // This prevents duplicate updates and ensures we use the latest detection data
    }

    /// <summary>
    /// Initialize the AprilTag visualization with pose data.
    /// </summary>
    /// <param name="tagPose">The AprilTag pose data.</param>
    /// <param name="arCamera">The AR camera used for detection.</param>
    public void Initialize(TagPose tagPose, Camera arCamera)
    {
        m_CurrentTagPose = tagPose;
        m_ARCamera = arCamera;
        m_TagId = tagPose.ID;
        m_IsInitialized = true;

        if (m_EnableLogging)
        {
            Debug.Log($"[AprilTagViz] Tag {m_TagId} INITIALIZED");
            Debug.Log($"[AprilTagViz] Camera-Relative Position: {tagPose.Position}");
            Debug.Log($"[AprilTagViz] Camera Position: {arCamera.transform.position}");
            Debug.Log($"[AprilTagViz] Camera Rotation: {arCamera.transform.rotation.eulerAngles}");
            
            if (m_UseFixedWorldPosition)
            {
                Debug.Log($"[AprilTagViz] Using FIXED world position: {m_FixedWorldPosition}");
            }
        }

        // Set the initial position
        UpdatePosition(tagPose);

        if (m_EnableLogging)
        {
            Debug.Log($"[AprilTagViz] World Position: {transform.position}");
            Debug.Log($"[AprilTagViz] World Rotation: {transform.rotation.eulerAngles}");
        }

        // Update the tag ID text
        if (m_ShowTagId && m_TagIdText != null)
        {
            m_TagIdText.text = m_TagId.ToString();
        }
    }

    /// <summary>
    /// Update the position and rotation of the AprilTag visualization.
    /// </summary>
    /// <param name="tagPose">The current AprilTag pose data.</param>
    public void UpdatePosition(TagPose tagPose)
    {
        if (m_ARCamera == null)
        {
            if (m_EnableLogging)
                Debug.LogWarning($"[AprilTagViz] UpdatePosition skipped - Camera is null");
            return;
        }

        m_CurrentTagPose = tagPose;

        // If using fixed world position, don't update position based on new detections
        if (m_UseFixedWorldPosition)
        {
            if (m_EnableLogging)
            {
                Debug.Log($"[AprilTagViz] Tag {m_TagId} FIXED - staying at world position {m_FixedWorldPosition}");
                Debug.Log($"  Distance from current camera: {Vector3.Distance(m_FixedWorldPosition, m_ARCamera.transform.position):F4}m");
            }
            
            // Keep the fixed world position
            transform.position = m_FixedWorldPosition;
            transform.rotation = m_FixedWorldRotation;
            
            // Apply scale
            transform.localScale = Vector3.one * m_Scale;
            
            // Update canvas to face the camera
            if (m_TagCanvas != null)
            {
                m_TagCanvas.transform.LookAt(m_ARCamera.transform);
                m_TagCanvas.transform.Rotate(0, 180, 0); // Face the camera
            }
            
            return;
        }

        // Store old values for comparison
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        if (m_EnableLogging)
        {
            Debug.Log($"[AprilTagViz] Tag {m_TagId} UPDATE BEFORE:");
            Debug.Log($"  Camera-Relative Position: {tagPose.Position}");
            Debug.Log($"  Camera World Position: {m_ARCamera.transform.position}");
            Debug.Log($"  Camera World Rotation: {m_ARCamera.transform.rotation.eulerAngles}");
        }

        // Transform position from camera-relative to world space
        // TagPose.Position and TagPose.Rotation are in camera-local coordinates
        Vector3 newWorldPosition = m_ARCamera.transform.TransformPoint(tagPose.Position);
        Quaternion newWorldRotation = m_ARCamera.transform.rotation * tagPose.Rotation;
        
        if (m_EnableLogging)
        {
            Debug.Log($"[AprilTagViz] Tag {m_TagId} UPDATE AFTER TransformPoint:");
            Debug.Log($"  New World Position: {newWorldPosition}");
            Debug.Log($"  Distance from Camera: {Vector3.Distance(newWorldPosition, m_ARCamera.transform.position):F4}m");
            Debug.Log($"  Expected distance (magnitude of camera-relative): {tagPose.Position.magnitude:F4}m");
        }
        
        transform.position = newWorldPosition;
        transform.rotation = newWorldRotation;
        
        // Apply scale
        transform.localScale = Vector3.one * m_Scale;

        // Log if position or rotation changed significantly
        if (m_EnableLogging)
        {
            float positionDelta = Vector3.Distance(oldPosition, transform.position);
            float rotationDelta = Quaternion.Angle(oldRotation, transform.rotation);
            
            if (positionDelta > 0.01f || rotationDelta > 1f)
            {
                Debug.Log($"[AprilTagViz] Tag {m_TagId} FINAL:");
                Debug.Log($"  World Position: {transform.position}");
                Debug.Log($"  Position Delta from last: {positionDelta:F4}m");
                Debug.Log($"  Rotation Delta from last: {rotationDelta:F2}°)");
            }
        }

        // Update canvas to face the camera
        if (m_TagCanvas != null)
        {
            m_TagCanvas.transform.LookAt(m_ARCamera.transform);
            m_TagCanvas.transform.Rotate(0, 180, 0); // Face the camera
        }
        
        // Store for next comparison
        m_LastPosition = transform.position;
        m_LastRotation = transform.rotation;
    }

    void SetupTagIdText()
    {
        // Create a canvas for the tag ID text
        GameObject canvasObject = new GameObject("TagCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.transform.localPosition = Vector3.zero;
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = Vector3.one;

        m_TagCanvas = canvasObject.AddComponent<Canvas>();
        m_TagCanvas.renderMode = RenderMode.WorldSpace;
        m_TagCanvas.worldCamera = m_ARCamera;

        // Add a canvas scaler
        var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        // Create the background object FIRST (Image needs to be on a separate object from Text)
        GameObject bgObject = new GameObject("Background");
        bgObject.transform.SetParent(canvasObject.transform);
        bgObject.transform.localPosition = new Vector3(0, 0.1f, 0);
        bgObject.transform.localRotation = Quaternion.identity;
        bgObject.transform.localScale = Vector3.one * 0.01f;
        
        var image = bgObject.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        
        var bgRect = bgObject.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(100, 40);

        // Create the text object as a child of background
        GameObject textObject = new GameObject("TagIdText");
        textObject.transform.SetParent(bgObject.transform);
        textObject.transform.localPosition = Vector3.zero;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = Vector3.one;

        m_TagIdText = textObject.AddComponent<UnityEngine.UI.Text>();
        m_TagIdText.text = m_TagId.ToString();
        m_TagIdText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        m_TagIdText.fontSize = 24;
        m_TagIdText.color = Color.white;
        m_TagIdText.alignment = TextAnchor.MiddleCenter;
        
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
    }

    /// <summary>
    /// Set the visibility of the AprilTag visualization.
    /// </summary>
    /// <param name="visible">True to make visible, false to hide.</param>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    /// <summary>
    /// Get the world position of the AprilTag.
    /// </summary>
    /// <returns>The world position of the AprilTag.</returns>
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Get the world rotation of the AprilTag.
    /// </summary>
    /// <returns>The world rotation of the AprilTag.</returns>
    public Quaternion GetWorldRotation()
    {
        return transform.rotation;
    }

    /// <summary>
    /// Get the distance from the AR camera to this AprilTag.
    /// </summary>
    /// <returns>The distance in meters.</returns>
    public float GetDistanceFromCamera()
    {
        if (m_ARCamera == null)
            return 0f;

        return Vector3.Distance(transform.position, m_ARCamera.transform.position);
    }
}