#if UNITY_EDITOR_WIN

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Text;
using System.Reflection;

public class AndroidKeystoreAuthenticatorWindows : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var keystorePath = PlayerSettings.applicationIdentifier + "#Keystore";
        if (string.IsNullOrEmpty(keystorePath) || !File.Exists(PlayerSettings.Android.keystoreName) || string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName))
        {
            // Unity will warn the user that the Keystore hasn't been configured yet
            return;
        }

        // Used to only warn once if both Keystore and Key Alias passwords are missing
        var hasWarned = false;

        var pwd = ManagePassword("Keystore", keystorePath, PlayerSettings.Android.keystorePass, ref hasWarned);
        if (pwd != null)
        {
            PlayerSettings.Android.keystorePass = pwd;
        }

        var keyName = PlayerSettings.applicationIdentifier + "#Keyalias";
        pwd = ManagePassword("Key Alias", keyName, PlayerSettings.Android.keyaliasPass, ref hasWarned);
        if (pwd != null)
        {
            PlayerSettings.Android.keyaliasPass = pwd;
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        // Remove passwords after build
        // Note that this is not reliable as it won't be called if the build is cancelled
        // In this case Unity will clear the password when it exits
        PlayerSettings.Android.keystorePass = null;
        PlayerSettings.Android.keyaliasPass = null;
    }

    static string ManagePassword(string name, string keyName, string currentPassword, ref bool hasWarned)
    {
        string output;
        var code = Execute(keyName, "", "-load", out output);
        Debug.Log("Output " + output + " Code " + code);
        if (code != 0)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                if (!hasWarned)
                {
                    // Password not saved or set, prompt the user to do so
                    EditorUtility.DisplayDialog(
                        "Android Keystore Authenticator",
                        "The " + name + " password could not found in the EditorPrefs.\n\n"
                        + "Set it once in Unity's Player Settings and it will "
                        + "be remembered in the EditorPrefs for successive builds.",
                        "Ok"
                    );
                    hasWarned = true;
                }
                return null;
            }
            else
            {
                // Password set but not yet saved, store it in EditorPrefs
                Debug.Log("Adding " + name + " password to EditorPrefs.");
                AddPassword(keyName, currentPassword);
                return null;
            }
        }
        else if (currentPassword != output)
        {
            if (!string.IsNullOrEmpty(currentPassword))
            {
                // Password in EditorPrefs differs, update it
                Debug.Log("Updating " + name + " password in EditorPrefs.");
                AddPassword(keyName, currentPassword);
                return null;
            }
            else
            {
                // Set password from EditorPrefs
                return output;
            }
        }

        // EditorPrefs password and Player Settins password match
        return null;
    }

    static void AddPassword(string keyName, string password)
    {
        // We use the interactive mode of security here that allows us to
        // pipe the command to stdin and thus avoid having the password
        // exposed in the process table.
        string output;
        var code = Execute(keyName, password, "-store", out output);
        if (code != 0)
        {
            Debug.LogError("Failed to store password in EditorPrefs");
        }
    }

    static int Execute(string key, string value, string arguments, out string output)
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;

        if (arguments == "-store")
        {
            EditorPrefs.SetString(key, Encode(value));
        }

        try
        {
            output = Decode(EditorPrefs.GetString(key, ""));
            return 0;
        }
        catch (System.Exception)
        {
            output = "";
            return 1;
        }
    }

    static string Encode(string inputText)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(inputText);
        return Convert.ToBase64String(bytesToEncode);
    }

    static string Decode(string encodedText)
    {
        byte[] decodedBytes = Convert.FromBase64String(encodedText);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}
#endif
