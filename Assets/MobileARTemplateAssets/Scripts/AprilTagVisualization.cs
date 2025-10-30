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
            Debug.Log($"[AprilTagViz] Initial Position: {tagPose.Position}");
            Debug.Log($"[AprilTagViz] Initial Rotation: {tagPose.Rotation.eulerAngles}");
        }

        // Set the initial position
        UpdatePosition(tagPose);

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

        // Store old values for comparison
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        // Transform from camera-local space to world space
        // TagPose.Position and TagPose.Rotation are in camera-local coordinates
        transform.position = m_ARCamera.transform.TransformPoint(tagPose.Position);
        transform.rotation = m_ARCamera.transform.rotation * tagPose.Rotation;
        
        // Apply scale
        transform.localScale = Vector3.one * m_Scale;

        // Log if position or rotation changed significantly
        if (m_EnableLogging)
        {
            float positionDelta = Vector3.Distance(oldPosition, transform.position);
            float rotationDelta = Quaternion.Angle(oldRotation, transform.rotation);
            
            if (positionDelta > 0.01f || rotationDelta > 1f)
            {
                Debug.Log($"[AprilTagViz] Tag {m_TagId} UPDATED:");
                Debug.Log($"  Position: {transform.position} (delta: {positionDelta:F4}m)");
                Debug.Log($"  Rotation: {transform.rotation.eulerAngles} (delta: {rotationDelta:F2}°)");
                Debug.Log($"  TagPose.Position: {tagPose.Position}");
                Debug.Log($"  TagPose.Rotation: {tagPose.Rotation.eulerAngles}");
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