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

    [Header("Model Catalog")]
    [SerializeField]
    [Tooltip("Catalog mapping AprilTag names to specific models. If set, overrides Model Prefab.")]
    AprilTagModelCatalog m_ModelCatalog;

    [Header("Visualization")]
    [SerializeField]
    [Tooltip("Default prefab to spawn on tracked images (used if no catalog or tag not found).")]
    GameObject m_ModelPrefab;

    [SerializeField]
    [Tooltip("Default offset from the image (in local space). Y = height above image.")]
    Vector3 m_ModelOffset = new Vector3(0, 0.3f, 0);

    [SerializeField]
    [Tooltip("Default rotation offset (euler angles). Use (0, 0, 0) for default.")]
    Vector3 m_RotationOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("Default scale multiplier for the spawned model.")]
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
                
                // Log catalog info
                if (m_ModelCatalog != null)
                {
                    int enabledCount = m_ModelCatalog.GetEnabledMappingCount();
                    Debug.Log($"[ImageTracking] 📖 Model Catalog loaded with {enabledCount} enabled mappings");
                }
                else
                {
                    Debug.Log("[ImageTracking] ℹ️ No Model Catalog assigned - using default model for all tags");
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
        // Periodically log tracking status (optimized: every 2 seconds)
        if (m_ShowDebug && m_TrackedImageManager != null && Time.frameCount % 120 == 0)
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
        string tagName = trackedImage.referenceImage.name;
        
        if (m_ShowDebug)
        {
            Debug.Log($"[ImageTracking] ✅ Detected: {tagName}");
            
            // Show descriptive name if catalog is available
            if (m_ModelCatalog != null)
            {
                string descriptiveName = m_ModelCatalog.GetDescriptiveNameForTag(tagName);
                if (descriptiveName != tagName)
                {
                    Debug.Log($"  📝 Model: '{descriptiveName}'");
                }
            }
            
            Debug.Log($"  Position: {trackedImage.transform.position}");
            Debug.Log($"  Rotation: {trackedImage.transform.rotation.eulerAngles}");
        }

        // Don't spawn if already exists
        if (m_SpawnedObjects.ContainsKey(trackedImage.trackableId))
            return;

        // Get the appropriate model for this tag
        GameObject modelToSpawn = GetModelForTag(tagName);
        
        if (modelToSpawn == null)
        {
            if (m_ShowDebug)
            {
                Debug.LogWarning($"[ImageTracking] ⚠️ No model available for tag '{tagName}'");
            }
            return;
        }

        // Get transform settings (catalog-specific or defaults)
        Vector3 offset = m_ModelOffset;
        Vector3 rotation = m_RotationOffset;
        float scale = m_Scale;

        // Check if catalog has custom settings for this tag
        if (m_ModelCatalog != null)
        {
            var mapping = m_ModelCatalog.mappings.Find(m => m.aprilTagName == tagName);
            if (mapping != null)
            {
                // Always use catalog values (they have sensible defaults: 0.1m up, scale 1)
                offset = mapping.customOffset;
                rotation = mapping.customRotation;
                scale = mapping.customScale;
                
                if (m_ShowDebug && mapping.HasCustomTransform())
                {
                    Debug.Log($"[ImageTracking] 🎨 Using custom transform for '{mapping.descriptiveName}'");
                }
            }
        }

        // Create the model as a child of the tracked image
        GameObject spawnedObject = Instantiate(modelToSpawn, trackedImage.transform);
        
        // Apply offset and rotation
        spawnedObject.transform.localPosition = offset;
        spawnedObject.transform.localRotation = Quaternion.Euler(rotation);
        spawnedObject.transform.localScale = Vector3.one * scale;

        // Store reference
        m_SpawnedObjects[trackedImage.trackableId] = spawnedObject;

        if (m_ShowDebug)
        {
            Debug.Log($"[ImageTracking] 📦 Spawned model at offset: {offset}");
            Debug.Log($"[ImageTracking] 🔍 Diagnostic Info:");
            Debug.Log($"  - Prefab Name: {modelToSpawn.name}");
            Debug.Log($"  - Spawned Object: {spawnedObject.name}");
            Debug.Log($"  - Is Active: {spawnedObject.activeSelf}");
            Debug.Log($"  - Scale: {scale} (Local Scale: {spawnedObject.transform.localScale})");
            Debug.Log($"  - Position (local): {spawnedObject.transform.localPosition}");
            Debug.Log($"  - Position (world): {spawnedObject.transform.position}");
            Debug.Log($"  - Rotation (local): {spawnedObject.transform.localRotation.eulerAngles}");
            
            // Check for renderers
            Renderer[] renderers = spawnedObject.GetComponentsInChildren<Renderer>(true);
            Debug.Log($"  - Renderers Found: {renderers.Length}");
            foreach (var renderer in renderers)
            {
                Debug.Log($"    * {renderer.name}: Enabled={renderer.enabled}, Visible={renderer.isVisible}, GameObj Active={renderer.gameObject.activeSelf}");
            }
            
            // Check if it has MeshRenderer or SpriteRenderer
            MeshRenderer meshRenderer = spawnedObject.GetComponentInChildren<MeshRenderer>();
            SpriteRenderer spriteRenderer = spawnedObject.GetComponentInChildren<SpriteRenderer>();
            Debug.Log($"  - MeshRenderer: {(meshRenderer != null ? "Found" : "None")}");
            Debug.Log($"  - SpriteRenderer: {(spriteRenderer != null ? "Found" : "None")}");
        }
    }

    /// <summary>
    /// Get the appropriate model prefab for a given AprilTag name.
    /// Checks catalog first, falls back to default model.
    /// </summary>
    GameObject GetModelForTag(string tagName)
    {
        // Try catalog first
        if (m_ModelCatalog != null)
        {
            GameObject catalogModel = m_ModelCatalog.GetModelForTag(tagName);
            if (catalogModel != null)
            {
                return catalogModel;
            }
        }

        // Fall back to default model
        return m_ModelPrefab;
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

