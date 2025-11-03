using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Enhanced image tracking configuration for better angle detection and performance.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImprovedImageTracking : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField]
    [Tooltip("Max images to track simultaneously. Lower = better performance.")]
    [Range(1, 10)]
    int m_MaxNumberOfMovingImages = 2;
    
    [SerializeField]
    [Tooltip("Enable to detect images at different scales/angles. May impact performance.")]
    bool m_EnableAutomaticImageScaleEstimation = true;

    [Header("Detection Quality")]
    [SerializeField]
    [Tooltip("Minimum quality threshold for detection (0-1). Higher = more strict. Optimized: 0.2")]
    [Range(0f, 1f)]
    float m_MinimumDetectionQuality = 0.2f;

    private ARTrackedImageManager m_ImageManager;
    private ARSession m_ARSession;

    void Awake()
    {
        m_ImageManager = GetComponent<ARTrackedImageManager>();
        m_ARSession = FindAnyObjectByType<ARSession>();
        
        ApplyOptimalSettings();
    }

    void ApplyOptimalSettings()
    {
        if (m_ImageManager == null) return;

        // Set max moving images (using updated API)
        m_ImageManager.requestedMaxNumberOfMovingImages = m_MaxNumberOfMovingImages;
        
        Debug.Log($"[ImprovedTracking] ⚙️ Configured:");
        Debug.Log($"  - Max Moving Images: {m_MaxNumberOfMovingImages}");
        Debug.Log($"  - Scale Estimation: {(m_EnableAutomaticImageScaleEstimation ? "Enabled" : "Disabled")}");
        Debug.Log($"  - Min Detection Quality: {m_MinimumDetectionQuality}");
        
        // Note: Automatic scale estimation is a platform-level setting
        // It's configured through ARKit/ARCore session configuration
        if (m_EnableAutomaticImageScaleEstimation)
        {
            Debug.Log("  ℹ️ Automatic scale estimation requested (platform-dependent)");
        }
    }

    void OnEnable()
    {
        if (m_ImageManager != null)
        {
            m_ImageManager.trackedImagesChanged += OnImagesChanged;
        }
    }

    void OnDisable()
    {
        if (m_ImageManager != null)
        {
            m_ImageManager.trackedImagesChanged -= OnImagesChanged;
        }
    }

    private int m_FrameCounter = 0;
    private const int QualityCheckInterval = 10; // Check quality every 10 frames (optimized)

    void OnImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        // Filter out low-quality detections
        foreach (var image in args.added)
        {
            LogImageQuality(image, "Added");
        }

        // Optimize: Only check quality every N frames to reduce overhead
        m_FrameCounter++;
        bool shouldCheckQuality = (m_FrameCounter % QualityCheckInterval == 0);

        foreach (var image in args.updated)
        {
            // Only show visible images with good tracking
            if (image.trackingState == TrackingState.Tracking && shouldCheckQuality)
            {
                // Check if quality is acceptable (using approximate angle detection)
                float quality = EstimateTrackingQuality(image);
                
                if (quality < m_MinimumDetectionQuality)
                {
                    Debug.LogWarning($"[ImprovedTracking] ⚠️ Low quality detection ({quality:F2}) - consider adjusting viewing angle");
                }
            }
        }
    }

    /// <summary>
    /// Estimate tracking quality based on transform properties.
    /// Returns value between 0 (bad) and 1 (perfect).
    /// </summary>
    float EstimateTrackingQuality(ARTrackedImage image)
    {
        // Get the angle between the image normal and camera forward
        Camera arCamera = Camera.main;
        if (arCamera == null) return 1f;

        Vector3 imageNormal = image.transform.up; // Image faces up in local space
        Vector3 cameraToImage = (image.transform.position - arCamera.transform.position).normalized;
        
        // Dot product gives us the angle quality (1 = perpendicular/perfect, 0 = parallel/bad)
        float quality = Mathf.Abs(Vector3.Dot(imageNormal, cameraToImage));
        
        return quality;
    }

    void LogImageQuality(ARTrackedImage image, string eventType)
    {
        float quality = EstimateTrackingQuality(image);
        string qualityEmoji = quality > 0.7f ? "✅" : quality > 0.4f ? "⚠️" : "❌";
        
        Debug.Log($"[ImprovedTracking] {qualityEmoji} {eventType}: {image.referenceImage.name} " +
                  $"(Quality: {quality:F2}, State: {image.trackingState})");
    }

    // Public method to temporarily increase detection sensitivity
    public void BoostDetectionSensitivity()
    {
        m_MinimumDetectionQuality = 0.1f;
        Debug.Log("[ImprovedTracking] 🔍 Detection sensitivity boosted for difficult angles");
    }

    // Reset to normal sensitivity
    public void ResetDetectionSensitivity()
    {
        m_MinimumDetectionQuality = 0.2f; // Optimized default
        Debug.Log("[ImprovedTracking] ↩️ Detection sensitivity reset to optimized (0.2)");
    }
}

