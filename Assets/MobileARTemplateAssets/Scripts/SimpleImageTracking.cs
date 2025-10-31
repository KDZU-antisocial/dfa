using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Simple and reliable image tracking using AR Foundation only.
/// Perfect for tracking AprilTags or any other printed markers.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class SimpleImageTracking : MonoBehaviour
{
    [Header("AR Foundation Components")]
    [SerializeField]
    [Tooltip("AR Tracked Image Manager component (auto-assigned).")]
    ARTrackedImageManager m_TrackedImageManager;

    [Header("Visualization")]
    [SerializeField]
    [Tooltip("Prefab to spawn on tracked images.")]
    GameObject m_ModelPrefab;

    [SerializeField]
    [Tooltip("Offset from the image (in local space). Y = height above image.")]
    Vector3 m_ModelOffset = new Vector3(0, 0.3f, 0);

    [SerializeField]
    [Tooltip("Rotation offset (euler angles). Use (0, 0, 0) for default.")]
    Vector3 m_RotationOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Scale multiplier for the spawned model.")]
    float m_Scale = 1f;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Show debug messages in console.")]
    bool m_ShowDebug = true;

    // Track spawned objects
    private Dictionary<TrackableId, GameObject> m_SpawnedObjects = new Dictionary<TrackableId, GameObject>();

    void Awake()
    {
        Debug.Log("[ImageTracking] ⚙️ Awake() called");
        
        // Get ARTrackedImageManager if not assigned
        if (m_TrackedImageManager == null)
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
            Debug.Log($"[ImageTracking] Auto-assigned ARTrackedImageManager: {m_TrackedImageManager != null}");
        }
        
        if (m_TrackedImageManager == null)
        {
            Debug.LogError("[ImageTracking] ❌ ARTrackedImageManager is NULL! Component not found.");
        }
        else
        {
            Debug.Log($"[ImageTracking] ✅ ARTrackedImageManager found: {m_TrackedImageManager.name}");
        }
    }

    void OnEnable()
    {
        Debug.Log("[ImageTracking] ▶️ OnEnable() called");
        
        if (m_TrackedImageManager != null)
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("[ImageTracking] ✅ Subscribed to trackedImagesChanged event");
            
            // Log library info
            if (m_TrackedImageManager.referenceLibrary != null)
            {
                Debug.Log($"[ImageTracking] 📚 Reference Library has {m_TrackedImageManager.referenceLibrary.count} images");
                
                // Log each image in the library
                for (int i = 0; i < m_TrackedImageManager.referenceLibrary.count; i++)
                {
                    var refImage = m_TrackedImageManager.referenceLibrary[i];
                    Debug.Log($"[ImageTracking]   #{i}: Name='{refImage.name}', Size={refImage.size}m");
                }
            }
            else
            {
                Debug.LogWarning("[ImageTracking] ⚠️ Reference Library is NULL!");
            }
        }
        else
        {
            Debug.LogError("[ImageTracking] ❌ Cannot subscribe - ARTrackedImageManager is NULL!");
        }
    }
    
    void Update()
    {
        // Periodically log tracking status
        if (m_TrackedImageManager != null && Time.frameCount % 120 == 0) // Every 2 seconds at 60fps
        {
            int trackingCount = 0;
            foreach (var trackedImage in m_TrackedImageManager.trackables)
            {
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    trackingCount++;
                }
            }
            
            if (trackingCount > 0)
            {
                Debug.Log($"[ImageTracking] 👁️ Currently tracking {trackingCount} image(s)");
            }
            else
            {
                Debug.Log("[ImageTracking] 🔍 Scanning for images... (Point camera at AprilTag)");
            }
        }
    }

    void OnDisable()
    {
        if (m_TrackedImageManager != null)
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle newly detected images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            OnImageAdded(trackedImage);
        }

        // Handle updated images (position/rotation changed)
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            OnImageUpdated(trackedImage);
        }

        // Handle removed images
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            OnImageRemoved(trackedImage);
        }
    }

    void OnImageAdded(ARTrackedImage trackedImage)
    {
        if (m_ShowDebug)
        {
            Debug.Log($"[ImageTracking] ✅ Detected: {trackedImage.referenceImage.name}");
            Debug.Log($"  Position: {trackedImage.transform.position}");
            Debug.Log($"  Rotation: {trackedImage.transform.rotation.eulerAngles}");
        }

        // Spawn the model if prefab is assigned
        if (m_ModelPrefab != null && !m_SpawnedObjects.ContainsKey(trackedImage.trackableId))
        {
            // Create the model as a child of the tracked image
            GameObject spawnedObject = Instantiate(m_ModelPrefab, trackedImage.transform);
            
            // Apply offset and rotation
            spawnedObject.transform.localPosition = m_ModelOffset;
            spawnedObject.transform.localRotation = Quaternion.Euler(m_RotationOffset);
            spawnedObject.transform.localScale = Vector3.one * m_Scale;

            // Store reference
            m_SpawnedObjects[trackedImage.trackableId] = spawnedObject;

            if (m_ShowDebug)
            {
                Debug.Log($"[ImageTracking] 📦 Spawned model at offset: {m_ModelOffset}");
            }
        }
    }

    void OnImageUpdated(ARTrackedImage trackedImage)
    {
        // The GameObject is a child of the tracked image, so it automatically follows!
        // We just need to show/hide based on tracking quality
        if (m_SpawnedObjects.TryGetValue(trackedImage.trackableId, out GameObject spawnedObject))
        {
            bool shouldBeVisible = trackedImage.trackingState == TrackingState.Tracking;
            
            if (spawnedObject.activeSelf != shouldBeVisible)
            {
                spawnedObject.SetActive(shouldBeVisible);

                if (m_ShowDebug)
                {
                    string status = shouldBeVisible ? "👁️ VISIBLE" : "🙈 HIDDEN";
                    Debug.Log($"[ImageTracking] {status}: {trackedImage.referenceImage.name} (State: {trackedImage.trackingState})");
                }
            }
        }
    }

    void OnImageRemoved(ARTrackedImage trackedImage)
    {
        if (m_SpawnedObjects.TryGetValue(trackedImage.trackableId, out GameObject spawnedObject))
        {
            if (m_ShowDebug)
            {
                Debug.Log($"[ImageTracking] ❌ Removed: {trackedImage.referenceImage.name}");
            }

            Destroy(spawnedObject);
            m_SpawnedObjects.Remove(trackedImage.trackableId);
        }
    }

    /// <summary>
    /// Get the spawned object for a specific tracked image.
    /// </summary>
    public GameObject GetSpawnedObject(ARTrackedImage trackedImage)
    {
        m_SpawnedObjects.TryGetValue(trackedImage.trackableId, out GameObject obj);
        return obj;
    }

    /// <summary>
    /// Get all currently tracked images.
    /// </summary>
    public List<ARTrackedImage> GetActiveTrackedImages()
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
}

