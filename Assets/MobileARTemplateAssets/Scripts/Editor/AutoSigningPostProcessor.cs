using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class AutoSigningPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            // Get the main target GUID
            string targetGuid = project.GetUnityMainTargetGuid();

            // Enable automatic signing for all build configurations
            project.SetBuildProperty(targetGuid, "CODE_SIGN_STYLE", "Automatic");
            
            // Optional: Set development team if you have one
            // Uncomment and add your Team ID (10-character code) if you know it:
            // project.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", "YOUR_TEAM_ID");
            
            // Also set for the UnityFramework target
            string frameworkGuid = project.GetUnityFrameworkTargetGuid();
            project.SetBuildProperty(frameworkGuid, "CODE_SIGN_STYLE", "Automatic");
            // project.SetBuildProperty(frameworkGuid, "DEVELOPMENT_TEAM", "YOUR_TEAM_ID");

            // Write changes back to project
            project.WriteToFile(projectPath);
            
            Debug.Log("✅ Automatic signing enabled in Xcode project!");
        }
#endif
    }
}
