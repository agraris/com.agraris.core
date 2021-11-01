using UnityEditor;

public class Versioning
{
    [MenuItem("Agraris/Versioning/Add Major", false, 11)]
    public static void MajorVersioning()
    {
        IncrementVersion(1, 0, 0);
    }

    [MenuItem("Agraris/Versioning/Add Minor", false, 12)]
    public static void MinorVersioning()
    {
        IncrementVersion(0, 1, 0);
    }

    [MenuItem("Agraris/Versioning/Add Patch", false, 13)]
    public static void PatchVersioning()
    {
        IncrementVersion(0, 0, 1);
    }

    [MenuItem("Agraris/Versioning/Add Build", false, 14)]
    public static void BuildVersioning()
    {
        IncrementVersion(0, 0, 0);
    }

    static void IncrementVersion(int major, int minor, int build)
    {
#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode += 1;
#endif

        string[] bVersion = PlayerSettings.bundleVersion.Split('.');

        int majorV = int.Parse(bVersion[0]) + major;
        int minorV = int.Parse(bVersion[1]) + minor;
        int buildV = int.Parse(bVersion[2]) + build;

        // If increment major, reset minor and build
        if (major > 0)
        {
            minorV = 0;
            buildV = 0;
        }

        // If increment minor, reset build
        if (minor > 0)
            buildV = 0;

        PlayerSettings.bundleVersion = $"{majorV}.{minorV}.{buildV}";
    }
}
