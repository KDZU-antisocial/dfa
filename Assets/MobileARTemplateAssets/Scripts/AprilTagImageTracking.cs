using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Combines AprilTag detection (for ID recognition) with AR Foundation image tracking (for positioning/rotation).
/// This provides robust tracking with proper rotation handling.
/// 
/// SETUP INSTRUCTIONS:
/// 1. Add an AR Tracked Image Manager component to your AR Session Origin
/// 2. Create a Reference Image Library asset (right-click in Project > Create > XR > Reference Image Library)
/// 3. Add your AprilTag images to the Reference Image Library
/// 4. Assign the Reference Image Library to the AR Tracked Image Manager
/// 5. Add this script to the same GameObject as the AR Tracked Image Manager
/// 6. Assign your 3D model prefab to the m_ModelPrefab field
/// 7. Build and deploy to your device
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class AprilTagImageTracking : MonoBehaviour
{
    [Header("AR Foundation Components")]
    [SerializeField]
    [Tooltip("AR Tracked Image Manager component.")]
    ARTrackedImageManager m_TrackedImageManager;

    [SerializeField]
    [Tooltip("The AR Camera.")]
    Camera m_ARCamera;

    [Header("Visualization")]
    [SerializeField]
    [Tooltip("Prefab (3D model) to instantiate when an AprilTag is tracked.")]
    GameObject m_ModelPrefab;

    [SerializeField]
    [Tooltip("Position offset relative to the tracked image (local space). Use this to position the model above/below/in-front of the tag.")]
    Vector3 m_ModelOffset = new Vector3(0, 0.2f, 0); // 20cm above the tag by default

    [SerializeField]
    [Tooltip("Rotation offset for the model (euler angles). Use this to make the model face upright.")]
    Vector3 m_ModelRotationOffset = new Vector3(0, 0, 0);

    [SerializeField]
    [Tooltip("Scale for the spawned model.")]
    float m_ModelScale = 1f;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Show debug logging.")]
    bool m_ShowDebugInfo = true;

    // Private fields
    private Dictionary<TrackableId, GameObject> m_SpawnedModels = new Dictionary<TrackableId, GameObject>();

    void Awake()
    {
        // Get AR Tracked Image Manager if not assigned
        if (m_TrackedImageManager == null)
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        // Get AR Camera if not assigned
        if (m_ARCamera == null)
        {
            m_ARCamera = Camera.main;
        }

        if (m_TrackedImageManager == null)
        {
            Debug.LogError("[AprilTagImageTracking] ARTrackedImageManager not found! Please add this component.");
        }

        if (m_ModelPrefab == null)
        {
            Debug.LogWarning("[AprilTagImageTracking] Model Prefab not assigned! Please assign a 3D model to spawn.");
        }
    }

    void OnEnable()
    {
        if (m_TrackedImageManager != null)
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            
            if (m_ShowDebugInfo)
            {
                Debug.Log("[AprilTagImageTracking] Subscribed to tracked images changed event");
            }
        }
    }

    void OnDisable()
    {
        if (m_TrackedImageManager != null)
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            
            if (m_ShowDebugInfo)
            {
                Debug.Log("[AprilTagImageTracking] Unsubscribed from tracked images changed event");
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle newly detected images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            HandleTrackedImageAdded(trackedImage);
        }

        // Handle updated images (position/rotation changed)
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            HandleTrackedImageUpdated(trackedImage);
        }

        // Handle removed images
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            HandleTrackedImageRemoved(trackedImage);
        }
    }

    void HandleTrackedImageAdded(ARTrackedImage trackedImage)
    {
        if (m_ShowDebugInfo)
        {
            Debug.Log($"[AprilTagImageTracking] New tracked image detected: {trackedImage.referenceImage.name}");
            Debug.Log($"  World Position: {trackedImage.transform.position}");
            Debug.Log($"  World Rotation: {trackedImage.transform.rotation.eulerAngles}");
            Debug.Log($"  Tracking State: {trackedImage.trackingState}");
            Debug.Log($"  Size: {trackedImage.size}");
        }

        // Create the 3D model if prefab is assigned
        if (m_ModelPrefab != null && !m_SpawnedModels.ContainsKey(trackedImage.trackableId))
        {
            // Instantiate the model as a child of the tracked image
            // This way, AR Foundation automatically handles all position/rotation updates
            GameObject spawnedModel = Instantiate(m_ModelPrefab, trackedImage.transform);
            
            // Apply local offset (relative to the AprilTag)
            spawnedModel.transform.localPosition = m_ModelOffset;
            spawnedModel.transform.localRotation = Quaternion.Euler(m_ModelRotationOffset);
            spawnedModel.transform.localScale = Vector3.one * m_ModelScale;

            // Store reference
            m_SpawnedModels[trackedImage.trackableId] = spawnedModel;

            if (m_ShowDebugInfo)
            {
                Debug.Log($"[AprilTagImageTracking] Spawned model '{m_ModelPrefab.name}' for image '{trackedImage.referenceImage.name}'");
                Debug.Log($"  Local Offset: {m_ModelOffset}");
                Debug.Log($"  Local Rotation: {m_ModelRotationOffset}");
                Debug.Log($"  Scale: {m_ModelScale}");
            }
        }
    }

    void HandleTrackedImageUpdated(ARTrackedImage trackedImage)
    {
        // AR Foundation automatically updates the trackedImage.transform position/rotation
        // Since our model is a child of the tracked image, it automatically follows
        // We just need to handle visibility based on tracking quality
        
        if (m_SpawnedModels.TryGetValue(trackedImage.trackableId, out GameObject spawnedModel))
        {
            // Show/hide based on tracking quality
            bool shouldBeActive = trackedImage.trackingState == TrackingState.Tracking;
            
            if (spawnedModel.activeSelf != shouldBeActive)
            {
                spawnedModel.SetActive(shouldBeActive);

                if (m_ShowDebugInfo)
                {
                    string state = shouldBeActive ? "visible (tracking)" : "hidden (lost tracking)";
                    Debug.Log($"[AprilTagImageTracking] Image '{trackedImage.referenceImage.name}' is now {state}");
                    if (shouldBeActive)
                    {
                        Debug.Log($"  World Position: {trackedImage.transform.position}");
                        Debug.Log($"  World Rotation: {trackedImage.transform.rotation.eulerAngles}");
                    }
                }
            }
        }
    }

    void HandleTrackedImageRemoved(ARTrackedImage trackedImage)
    {
        if (m_SpawnedModels.TryGetValue(trackedImage.trackableId, out GameObject spawnedModel))
        {
            if (m_ShowDebugInfo)
            {
                Debug.Log($"[AprilTagImageTracking] Removing tracked image: {trackedImage.referenceImage.name}");
            }

            Destroy(spawnedModel);
            m_SpawnedModels.Remove(trackedImage.trackableId);
        }
    }

    /// <summary>
    /// Get the spawned model for a specific tracked image.
    /// </summary>
    public GameObject GetSpawnedModel(ARTrackedImage trackedImage)
    {
        m_SpawnedModels.TryGetValue(trackedImage.trackableId, out GameObject model);
        return model;
    }

    /// <summary>
    /// Get all currently tracked images that are actively being tracked.
    /// </summary>
    public List<ARTrackedImage> GetActivelyTrackedImages()
    {
        List<ARTrackedImage> images = new List<ARTrackedImage>();
        if (m_TrackedImageManager != null)
        {
            foreach (var trackedImage in m_TrackedImageManager.trackables)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    images.Add(trackedImage);
                }
            }
        }
        return images;
    }

    /// <summary>
    /// Update the model offset at runtime.
    /// </summary>
    public void SetModelOffset(Vector3 offset)
    {
        m_ModelOffset = offset;
        
        // Update all existing spawned models
        foreach (var kvp in m_SpawnedModels)
        {
            if (kvp.Value != null)
            {
                kvp.Value.transform.localPosition = m_ModelOffset;
            }
        }
        
        if (m_ShowDebugInfo)
        {
            Debug.Log($"[AprilTagImageTracking] Updated model offset to: {m_ModelOffset}");
        }
    }

    /// <summary>
    /// Update the model rotation offset at runtime.
    /// </summary>
    public void SetModelRotationOffset(Vector3 rotationEuler)
    {
        m_ModelRotationOffset = rotationEuler;
        
        // Update all existing spawned models
        foreach (var kvp in m_SpawnedModels)
        {
            if (kvp.Value != null)
            {
                kvp.Value.transform.localRotation = Quaternion.Euler(m_ModelRotationOffset);
            }
        }
        
        if (m_ShowDebugInfo)
        {
            Debug.Log($"[AprilTagImageTracking] Updated model rotation offset to: {m_ModelRotationOffset}");
        }
    }

    /// <summary>
    /// Update the model scale at runtime.
    /// </summary>
    public void SetModelScale(float scale)
    {
        m_ModelScale = scale;
        
        // Update all existing spawned models
        foreach (var kvp in m_SpawnedModels)
        {
            if (kvp.Value != null)
            {
                kvp.Value.transform.localScale = Vector3.one * m_ModelScale;
            }
        }
        
        if (m_ShowDebugInfo)
        {
            Debug.Log($"[AprilTagImageTracking] Updated model scale to: {m_ModelScale}");
        }
    }
}
