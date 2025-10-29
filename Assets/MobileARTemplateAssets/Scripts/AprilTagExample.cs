using UnityEngine;
using AprilTag;

/// <summary>
/// Example script demonstrating how to use the AprilTag system.
/// This script shows how to respond to AprilTag detection events and interact with detected tags.
/// </summary>
public class AprilTagExample : MonoBehaviour
{
    [Header("AprilTag Integration")]
    [SerializeField]
    [Tooltip("The AprilTag Manager in the scene.")]
    AprilTagManager m_AprilTagManager;

    [SerializeField]
    [Tooltip("Prefab to spawn when a specific tag is detected.")]
    GameObject m_ObjectToSpawn;

    [SerializeField]
    [Tooltip("The ID of the AprilTag that triggers object spawning.")]
    int m_TriggerTagId = 0;

    [SerializeField]
    [Tooltip("Whether to spawn objects on all detected tags.")]
    bool m_SpawnOnAllTags = false;

    [Header("Visual Feedback")]
    [SerializeField]
    [Tooltip("Color to change when a tag is detected.")]
    Color m_DetectionColor = Color.green;

    [SerializeField]
    [Tooltip("Color to change when a tag is lost.")]
    Color m_LossColor = Color.red;

    [SerializeField]
    [Tooltip("Duration to show the color change.")]
    float m_ColorChangeDuration = 1f;

    private Camera m_ARCamera;
    private Color m_OriginalColor;
    private float m_ColorChangeTimer;

    void Start()
    {
        // Find the AprilTag Manager if not assigned
        if (m_AprilTagManager == null)
        {
            m_AprilTagManager = FindObjectOfType<AprilTagManager>();
        }

        // Find the AR Camera
        m_ARCamera = Camera.main;
        if (m_ARCamera == null)
        {
            m_ARCamera = FindObjectOfType<Camera>();
        }

        // Store original color
        if (m_ARCamera != null)
        {
            m_OriginalColor = m_ARCamera.backgroundColor;
        }

        // Subscribe to AprilTag events
        if (m_AprilTagManager != null)
        {
            m_AprilTagManager.OnAprilTagDetected += OnAprilTagDetected;
            m_AprilTagManager.OnAprilTagLost += OnAprilTagLost;
            m_AprilTagManager.OnAprilTagUpdated += OnAprilTagUpdated;
        }
        else
        {
            Debug.LogWarning("AprilTagExample: No AprilTag Manager found! Please assign one in the inspector.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (m_AprilTagManager != null)
        {
            m_AprilTagManager.OnAprilTagDetected -= OnAprilTagDetected;
            m_AprilTagManager.OnAprilTagLost -= OnAprilTagLost;
            m_AprilTagManager.OnAprilTagUpdated -= OnAprilTagUpdated;
        }
    }

    void Update()
    {
        // Handle color change timer
        if (m_ColorChangeTimer > 0)
        {
            m_ColorChangeTimer -= Time.deltaTime;
            if (m_ColorChangeTimer <= 0 && m_ARCamera != null)
            {
                m_ARCamera.backgroundColor = m_OriginalColor;
            }
        }
    }

    void OnAprilTagDetected(int tagId, TagPose tagPose)
    {
        Debug.Log($"AprilTag {tagId} detected at position {tagPose.Position}");

        // Change camera background color
        ChangeCameraColor(m_DetectionColor);

        // Spawn object if conditions are met
        if (m_ObjectToSpawn != null && (m_SpawnOnAllTags || tagId == m_TriggerTagId))
        {
            SpawnObjectOnTag(tagId, tagPose);
        }

        // Log tag information
        LogTagInformation(tagId, tagPose);
    }

    void OnAprilTagLost(int tagId)
    {
        Debug.Log($"AprilTag {tagId} lost");

        // Change camera background color
        ChangeCameraColor(m_LossColor);
    }

    void OnAprilTagUpdated(int tagId, TagPose tagPose)
    {
        // This is called every frame while a tag is being tracked
        // Use this for continuous updates or animations
    }

    void ChangeCameraColor(Color color)
    {
        if (m_ARCamera != null)
        {
            m_ARCamera.backgroundColor = color;
            m_ColorChangeTimer = m_ColorChangeDuration;
        }
    }

    void SpawnObjectOnTag(int tagId, TagPose tagPose)
    {
        // Get the tag visualization
        var visualization = m_AprilTagManager.GetAprilTagVisualization(tagId);
        if (visualization == null)
        {
            Debug.LogWarning($"AprilTagExample: Could not find visualization for tag {tagId}");
            return;
        }

        // Spawn object at tag position
        Vector3 spawnPosition = visualization.GetWorldPosition();
        Quaternion spawnRotation = visualization.GetWorldRotation();

        GameObject spawnedObject = Instantiate(m_ObjectToSpawn, spawnPosition, spawnRotation);
        spawnedObject.name = $"SpawnedObject_Tag{tagId}";

        Debug.Log($"Spawned object for AprilTag {tagId} at position {spawnPosition}");
    }

    void LogTagInformation(int tagId, TagPose tagPose)
    {
        var visualization = m_AprilTagManager.GetAprilTagVisualization(tagId);
        if (visualization != null)
        {
            float distance = visualization.GetDistanceFromCamera();
            Debug.Log($"Tag {tagId} - Position: {visualization.GetWorldPosition()}, Distance: {distance:F2}m");
        }
    }

    /// <summary>
    /// Get the number of currently tracked AprilTags.
    /// </summary>
    /// <returns>Number of tracked tags.</returns>
    public int GetTrackedTagCount()
    {
        if (m_AprilTagManager != null)
        {
            return m_AprilTagManager.GetTrackedTagIds().Length;
        }
        return 0;
    }

    /// <summary>
    /// Check if a specific tag is currently being tracked.
    /// </summary>
    /// <param name="tagId">The tag ID to check.</param>
    /// <returns>True if the tag is being tracked.</returns>
    public bool IsTagTracked(int tagId)
    {
        if (m_AprilTagManager != null)
        {
            var visualization = m_AprilTagManager.GetAprilTagVisualization(tagId);
            return visualization != null;
        }
        return false;
    }

    /// <summary>
    /// Get the position of a specific tracked tag.
    /// </summary>
    /// <param name="tagId">The tag ID.</param>
    /// <returns>The world position of the tag, or Vector3.zero if not found.</returns>
    public Vector3 GetTagPosition(int tagId)
    {
        if (m_AprilTagManager != null)
        {
            var visualization = m_AprilTagManager.GetAprilTagVisualization(tagId);
            if (visualization != null)
            {
                return visualization.GetWorldPosition();
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Clear all spawned objects.
    /// </summary>
    [ContextMenu("Clear All Spawned Objects")]
    public void ClearAllSpawnedObjects()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("SpawnedObject");
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        Debug.Log("Cleared all spawned objects");
    }
}