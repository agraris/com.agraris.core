#if UNITY_ANDROID
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Reflection;
using System.IO;

[InitializeOnLoad]
sealed class AndroidBuildPipeline : IPostprocessBuildWithReport
{
    public int callbackOrder => 100;

    public enum BuildType { Production, Development }

    static void AndroidBuild(BuildType buildType, bool includeARM64 = false)
    {
        PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

        switch (buildType)
        {
            case BuildType.Production:
                EditorUserBuildSettings.buildAppBundle = true;
                EditorUserBuildSettings.development = false;
                break;
            default:
                EditorUserBuildSettings.buildAppBundle = false;
                if (!includeARM64)
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                break;
        }

        EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, GetBuildPath(buildType));

        BuildPlayerOptions options = GetBuildPlayerOptions();
        options.options |= BuildOptions.DetailedBuildReport;

        if (buildType == BuildType.Production)
        {
            options.options |= BuildOptions.CompressWithLz4HC;
        }
        else
        {
            options.options |= BuildOptions.Development;
            options.options |= BuildOptions.CompressWithLz4;
        }

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        switch (summary.result)
        {
            case BuildResult.Cancelled:
                break;
            case BuildResult.Failed:
                Debug.Log("Build failed.");
                break;
            case BuildResult.Succeeded:
                Debug.Log("Build succeeded. " + summary.outputPath);
                EditorUtility.RevealInFinder(summary.outputPath);
                break;
            case BuildResult.Unknown:
                break;
            default:
                break;
        }
    }

    static string GetBuildPath(BuildType buildType)
    {
        string defaultFolder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds");
        // Based on semantic versioning
        string defaultName = $"{Application.productName} {PlayerSettings.bundleVersion}{(buildType == BuildType.Development ? "-dev" : "")}+{PlayerSettings.Android.bundleVersionCode}";
        string extension = (EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk");
        return Path.Combine(defaultFolder, defaultName + extension).Replace("\\", "/");
    }

    static BuildPlayerOptions GetBuildPlayerOptions(bool askForLocation = false, BuildPlayerOptions defaultOptions = new BuildPlayerOptions())
    {
        // Get static internal "GetBuildPlayerOptionsInternal" method
        MethodInfo method = typeof(BuildPlayerWindow.DefaultBuildMethods).GetMethod(
          "GetBuildPlayerOptionsInternal",
          BindingFlags.NonPublic | BindingFlags.Static);

        // invoke internal method
        return (BuildPlayerOptions)method.Invoke(
            null,
            new object[] { askForLocation, defaultOptions });
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
    }

    #region MENU
    [MenuItem("Agraris/Build/Android - Development", false, 1)]
    public static void DevelopmentAndroidBuild()
    {
        AndroidBuild(BuildType.Development);
    }

    [MenuItem("Agraris/Build/Android - Development + ARM64 (Slow)", false, 2)]
    public static void Development64AndroidBuild()
    {
        AndroidBuild(BuildType.Development, true);
    }

    [MenuItem("Agraris/Build/Android - Production", false, 3)]
    public static void ProductionAndroidBuild()
    {
        AndroidBuild(BuildType.Production);
    }
    #endregion
}
#endif
