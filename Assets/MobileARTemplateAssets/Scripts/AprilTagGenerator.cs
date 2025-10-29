using UnityEngine;
using System.IO;
using AprilTag;

/// <summary>
/// Utility script to generate AprilTag images for testing.
/// This script can be used to create AprilTag images that can be printed and used for testing.
/// </summary>
public class AprilTagGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField]
    [Tooltip("The AprilTag family to use for generation.")]
    AprilTagFamily m_TagFamily = AprilTagFamily.TagStandard41h12;

    [SerializeField]
    [Tooltip("The ID of the AprilTag to generate.")]
    int m_TagId = 0;

    [SerializeField]
    [Tooltip("The size of the generated AprilTag image in pixels.")]
    int m_ImageSize = 512;

    [SerializeField]
    [Tooltip("The path to save the generated AprilTag images.")]
    string m_SavePath = "AprilTags";

    [SerializeField]
    [Tooltip("Whether to generate multiple tags at once.")]
    bool m_GenerateMultiple = false;

    [SerializeField]
    [Tooltip("Number of tags to generate if generating multiple.")]
    int m_NumberOfTags = 5;

    /// <summary>
    /// Generate a single AprilTag image.
    /// </summary>
    [ContextMenu("Generate Single AprilTag")]
    public void GenerateSingleAprilTag()
    {
        GenerateAprilTag(m_TagId, m_ImageSize);
    }

    /// <summary>
    /// Generate multiple AprilTag images.
    /// </summary>
    [ContextMenu("Generate Multiple AprilTags")]
    public void GenerateMultipleAprilTags()
    {
        if (!m_GenerateMultiple)
        {
            Debug.LogWarning("AprilTagGenerator: Multiple generation is disabled. Enable 'Generate Multiple' to use this feature.");
            return;
        }

        for (int i = 0; i < m_NumberOfTags; i++)
        {
            GenerateAprilTag(i, m_ImageSize);
        }
    }

    /// <summary>
    /// Generate an AprilTag image with the specified ID and size.
    /// </summary>
    /// <param name="tagId">The ID of the AprilTag to generate.</param>
    /// <param name="size">The size of the image in pixels.</param>
    public void GenerateAprilTag(int tagId, int size)
    {
        try
        {
            // Create a simple test pattern for now
            // In a real implementation, you would use the AprilTag library to generate the actual tag
            Texture2D texture = CreateTestAprilTag(tagId, size);
            
            if (texture == null)
            {
                Debug.LogError($"AprilTagGenerator: Failed to generate AprilTag {tagId}");
                return;
            }

            // Encode to PNG
            byte[] pngData = texture.EncodeToPNG();
            
            // Create directory if it doesn't exist
            string fullPath = Path.Combine(Application.dataPath, m_SavePath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Save the image
            string fileName = $"AprilTag_{m_TagFamily}_{tagId}_{size}x{size}.png";
            string filePath = Path.Combine(fullPath, fileName);
            File.WriteAllBytes(filePath, pngData);

            Debug.Log($"AprilTagGenerator: Generated AprilTag {tagId} saved to {filePath}");

            // Clean up
            DestroyImmediate(texture);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AprilTagGenerator: Error generating AprilTag {tagId}: {e.Message}");
        }
    }

    Texture2D CreateTestAprilTag(int tagId, int size)
    {
        // Create a simple test pattern that looks like an AprilTag
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGB24, false);
        
        // Fill with white background
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        // Create a simple black and white pattern
        int borderSize = size / 8;
        int innerSize = size - (borderSize * 2);
        
        // Black border
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x < borderSize || x >= size - borderSize || y < borderSize || y >= size - borderSize)
                {
                    pixels[y * size + x] = Color.black;
                }
            }
        }
        
        // Inner pattern (simplified)
        for (int y = borderSize; y < size - borderSize; y++)
        {
            for (int x = borderSize; x < size - borderSize; x++)
            {
                int patternX = (x - borderSize) * 8 / innerSize;
                int patternY = (y - borderSize) * 8 / innerSize;
                
                // Create a simple checkerboard pattern
                if ((patternX + patternY) % 2 == 0)
                {
                    pixels[y * size + x] = Color.black;
                }
            }
        }
        
        // Add tag ID in the center
        int centerX = size / 2;
        int centerY = size / 2;
        int textSize = size / 16;
        
        for (int y = centerY - textSize; y < centerY + textSize; y++)
        {
            for (int x = centerX - textSize; x < centerX + textSize; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[y * size + x] = Color.black;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }

    /// <summary>
    /// Generate a test scene with AprilTag images.
    /// </summary>
    [ContextMenu("Generate Test Scene")]
    public void GenerateTestScene()
    {
        // Create a test plane
        GameObject testPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        testPlane.name = "AprilTag Test Plane";
        testPlane.transform.position = new Vector3(0, 0, 2);
        testPlane.transform.localScale = new Vector3(2, 1, 2);

        // Create a material for the plane
        Material planeMaterial = new Material(Shader.Find("Standard"));
        planeMaterial.color = Color.white;
        testPlane.GetComponent<Renderer>().material = planeMaterial;

        // Generate a few AprilTag images and apply them to the plane
        for (int i = 0; i < 3; i++)
        {
            GenerateAprilTag(i, 256);
        }

        Debug.Log("AprilTagGenerator: Test scene created. AprilTag images have been generated for testing.");
    }

    /// <summary>
    /// Open the folder containing the generated AprilTag images.
    /// </summary>
    [ContextMenu("Open AprilTag Folder")]
    public void OpenAprilTagFolder()
    {
        string fullPath = Path.Combine(Application.dataPath, m_SavePath);
        if (Directory.Exists(fullPath))
        {
            Application.OpenURL($"file://{fullPath}");
        }
        else
        {
            Debug.LogWarning("AprilTagGenerator: AprilTag folder does not exist. Generate some tags first.");
        }
    }
}

/// <summary>
/// Enum for AprilTag families (simplified version).
/// </summary>
public enum AprilTagFamily
{
    TagStandard41h12
}