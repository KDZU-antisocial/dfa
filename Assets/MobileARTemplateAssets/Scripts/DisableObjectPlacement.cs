using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Simple script to disable the tap-to-place object spawning functionality.
/// Attach this to any GameObject in your scene.
/// </summary>
public class DisableObjectPlacement : MonoBehaviour
{
    [Header("What to Disable")]
    [SerializeField]
    [Tooltip("Disable the object spawner (tap-to-place).")]
    bool m_DisableObjectSpawner = true;
    
    [SerializeField]
    [Tooltip("Disable the create button UI.")]
    bool m_DisableCreateButton = true;
    
    [SerializeField]
    [Tooltip("Disable the entire menu manager.")]
    bool m_DisableMenuManager = false;
    
    [SerializeField]
    [Tooltip("Disable the goal/tutorial system.")]
    bool m_DisableGoalManager = true;
    
    [SerializeField]
    [Tooltip("Disable AR plane visualization (dotted planes).")]
    bool m_DisablePlaneVisualization = true;

    void Start()
    {
        if (m_DisableObjectSpawner)
        {
            var objectSpawner = FindObjectOfType<ObjectSpawner>();
            if (objectSpawner != null)
            {
                objectSpawner.enabled = false;
                Debug.Log("✅ ObjectSpawner disabled - tap-to-place is OFF");
            }
        }
        
        if (m_DisableCreateButton)
        {
            var menuManager = FindObjectOfType<ARTemplateMenuManager>();
            if (menuManager != null && menuManager.createButton != null)
            {
                menuManager.createButton.gameObject.SetActive(false);
                Debug.Log("✅ Create button hidden");
            }
        }
        
        if (m_DisableMenuManager)
        {
            var menuManager = FindObjectOfType<ARTemplateMenuManager>();
            if (menuManager != null)
            {
                menuManager.enabled = false;
                Debug.Log("✅ AR Template Menu Manager disabled");
            }
        }
        
        if (m_DisableGoalManager)
        {
            var goalManager = FindObjectOfType<GoalManager>();
            if (goalManager != null)
            {
                goalManager.enabled = false;
                Debug.Log("✅ Goal/Tutorial Manager disabled");
            }
        }
        
        if (m_DisablePlaneVisualization)
        {
            // Disable ARPlaneManager to stop detecting new planes
            var planeManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARPlaneManager>();
            if (planeManager != null)
            {
                // Hide all existing planes
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
                
                // Disable plane detection
                planeManager.enabled = false;
                Debug.Log("✅ AR Plane visualization disabled");
            }
        }
    }
}
