using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catalog that maps AprilTag names/IDs to specific 3D model prefabs.
/// Use this to associate each detected AprilTag with a unique model.
/// </summary>
[CreateAssetMenu(fileName = "AprilTagModelCatalog", menuName = "AR/AprilTag Model Catalog", order = 1)]
public class AprilTagModelCatalog : ScriptableObject
{
    [Header("Model Mappings")]
    [Tooltip("List of AprilTag to Model mappings. Add one entry per AprilTag.")]
    public List<AprilTagModelMapping> mappings = new List<AprilTagModelMapping>();

    [Header("Fallback")]
    [Tooltip("Optional: Default model if tag not found in catalog.")]
    public GameObject defaultModel;

    /// <summary>
    /// Get the model prefab for a specific AprilTag name.
    /// </summary>
    /// <param name="tagName">The name of the detected AprilTag (e.g., "0", "1", "2")</param>
    /// <returns>The associated model prefab, or null if not found</returns>
    public GameObject GetModelForTag(string tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            Debug.LogWarning("[Catalog] GetModelForTag called with null/empty tag name");
            return defaultModel;
        }

        // Find mapping for this tag
        var mapping = mappings.Find(m => m.aprilTagName == tagName);
        
        if (mapping != null && mapping.modelPrefab != null)
        {
            if (mapping.enabled)
            {
                return mapping.modelPrefab;
            }
            else
            {
                Debug.LogWarning($"[Catalog] Mapping for tag '{tagName}' is disabled");
                return null;
            }
        }

        // Not found - use default if available
        if (defaultModel != null)
        {
            Debug.LogWarning($"[Catalog] No mapping found for tag '{tagName}', using default model");
            return defaultModel;
        }

        Debug.LogWarning($"[Catalog] No mapping found for tag '{tagName}' and no default model set");
        return null;
    }

    /// <summary>
    /// Get the descriptive name for a specific AprilTag.
    /// </summary>
    public string GetDescriptiveNameForTag(string tagName)
    {
        var mapping = mappings.Find(m => m.aprilTagName == tagName);
        return mapping?.descriptiveName ?? tagName;
    }

    /// <summary>
    /// Check if a tag has a mapping in the catalog.
    /// </summary>
    public bool HasMapping(string tagName)
    {
        return mappings.Exists(m => m.aprilTagName == tagName && m.enabled && m.modelPrefab != null);
    }

    /// <summary>
    /// Get total number of enabled mappings.
    /// </summary>
    public int GetEnabledMappingCount()
    {
        return mappings.FindAll(m => m.enabled && m.modelPrefab != null).Count;
    }

    /// <summary>
    /// Validate the catalog and log any issues.
    /// </summary>
    public void ValidateCatalog()
    {
        Debug.Log($"[Catalog] Validating {mappings.Count} mappings...");

        int enabled = 0;
        int missingModels = 0;
        int duplicates = 0;
        HashSet<string> seenTags = new HashSet<string>();

        foreach (var mapping in mappings)
        {
            if (!mapping.enabled) continue;
            enabled++;

            // Check for missing models
            if (mapping.modelPrefab == null)
            {
                Debug.LogWarning($"[Catalog] Tag '{mapping.aprilTagName}' ({mapping.descriptiveName}) has no model assigned!");
                missingModels++;
            }

            // Check for duplicates
            if (!string.IsNullOrEmpty(mapping.aprilTagName))
            {
                if (seenTags.Contains(mapping.aprilTagName))
                {
                    Debug.LogWarning($"[Catalog] Duplicate tag name: '{mapping.aprilTagName}'");
                    duplicates++;
                }
                else
                {
                    seenTags.Add(mapping.aprilTagName);
                }
            }
        }

        Debug.Log($"[Catalog] Validation complete:");
        Debug.Log($"  ✅ Enabled mappings: {enabled}");
        Debug.Log($"  ⚠️ Missing models: {missingModels}");
        Debug.Log($"  ⚠️ Duplicate tags: {duplicates}");
        Debug.Log($"  📊 Total mappings: {mappings.Count}");
    }
}

/// <summary>
/// Single mapping entry: AprilTag name → Model prefab
/// </summary>
[System.Serializable]
public class AprilTagModelMapping
{
    [Header("AprilTag Info")]
    [Tooltip("The AprilTag name from Reference Library (e.g., '0', '1', '2')")]
    public string aprilTagName;

    [Tooltip("Descriptive name for this tag (e.g., 'Leaping Coyote', 'Running Fox')")]
    public string descriptiveName;

    [Header("Model")]
    [Tooltip("The 3D model prefab to spawn when this tag is detected")]
    public GameObject modelPrefab;

    [Header("Optional Settings")]
    [Tooltip("Enable/disable this mapping without deleting it")]
    public bool enabled = true;

    [Tooltip("Optional: Custom offset for this specific model (default: 0.1m above tag)")]
    public Vector3 customOffset = new Vector3(0, 0.1f, 0);

    [Tooltip("Optional: Custom rotation for this specific model")]
    public Vector3 customRotation = Vector3.zero;

    [Tooltip("Optional: Custom scale for this specific model (1 = normal size)")]
    public float customScale = 1f;

    [Header("Metadata (Optional)")]
    [Tooltip("Optional: Notes or description for this mapping")]
    [TextArea(2, 4)]
    public string notes;

    /// <summary>
    /// Check if this mapping has custom transform settings (different from defaults).
    /// </summary>
    public bool HasCustomTransform()
    {
        Vector3 defaultOffset = new Vector3(0, 0.1f, 0);
        return customOffset != defaultOffset || 
               customRotation != Vector3.zero || 
               customScale != 1f;
    }
}

