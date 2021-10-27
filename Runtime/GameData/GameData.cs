using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

public static class GameData
{
    public enum GameDataType { Binary, Text }

    static private string _fileName = "game_data";
    static private string _fileFullPath = "";
    static private GameDataType _type = GameDataType.Text;

    static private Dictionary<string, string> prefs = new Dictionary<string, string>();

    static public DateTime CreatedAt => GetDateTime("CreatedAt");
    static public DateTime UpdatedAt => GetDateTime("UpdatedAt");
    static public TimeSpan TotalPlayTime => GetTimeSpan("TotalPlayTime");

    static public string Extension
    {
        get
        {
            switch (_type)
            {
                case GameDataType.Binary:
                    return ".bin";
                case GameDataType.Text:
                    return ".json";
                default:
                    return ".json";
            }
        }
    }
    static public string SaveDirectory
    {
        get
        {
            string savePath = Path.Combine(Application.persistentDataPath);
#if UNITY_EDITOR
            savePath = Path.Combine(Application.dataPath, "../GameData");
#endif
            return savePath;
        }
    }

    static public void Init(String fileName = "game_data", GameDataType fileType = GameDataType.Text)
    {
        _fileName = fileName;
        _type = fileType;

        Init();
    }

    static void Init()
    {
        string _fileDir = Directory.CreateDirectory(SaveDirectory).FullName;
        _fileFullPath = Path.Combine(_fileDir, _fileName + Extension);

        if (File.Exists(_fileFullPath))
            Load();
        else
        {
            DateTime now = DateTime.UtcNow;
            SetDateTime("CreatedAt", now);
            SetDateTime("UpdatedAt", now);
            SetTimeSpan("TotalPlayTime", TimeSpan.Zero);
        }
    }

    public static void DeleteSave()
    {
        string _fileFullPath = Path.Combine(SaveDirectory, _fileName + Extension);

        if (File.Exists(_fileFullPath))
            File.Delete(_fileFullPath);
    }

    public static void Save()
    {
        DateTime now = DateTime.UtcNow;
        SetDateTime("UpdatedAt", now);
        SetTimeSpan("TotalPlayTime", TotalPlayTime.Add(now.Subtract(UpdatedAt)));

        switch (_type)
        {
            case GameDataType.Binary:
                SaveBinary();
                break;
            case GameDataType.Text:
                SaveText();
                break;
            default:
                SaveText();
                break;
        }
    }

    static void SaveBinary()
    {
        BinaryWriter w = new BinaryWriter(new FileStream(_fileFullPath, FileMode.OpenOrCreate));

        w.Write(prefs.Count);
        foreach (KeyValuePair<string, string> item in prefs)
        {
            w.Write(item.Key);
            w.Write(item.Value);
        }

        w.Close();
    }

    static void SaveText()
    {
        string jsonString = JsonConvert.SerializeObject(prefs);

        if (!Debug.isDebugBuild)
            jsonString = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

        File.WriteAllText(_fileFullPath, jsonString);
    }

    public static void Load()
    {
        switch (_type)
        {
            case GameDataType.Binary:
                LoadBinary();
                break;
            case GameDataType.Text:
                LoadText();
                break;
            default:
                LoadText();
                break;
        }
    }

    static void LoadBinary()
    {
        BinaryReader r = new BinaryReader(new FileStream(_fileFullPath, FileMode.Open));

        prefs.Clear();
        int charCount = r.ReadInt32();
        for (int i = 0; i < charCount; i++)
        {
            prefs.Add(r.ReadString(), r.ReadString());
        }

        r.Close();
    }

    static void LoadText()
    {
        string base64String = File.ReadAllText(_fileFullPath);

        if (!Debug.isDebugBuild)
            base64String = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

        prefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(base64String);
    }

    #region Boolean
    public static void SetBool(String key, bool value)
    {
        prefs[key] = Convert.ToString(value);
    }

    public static bool GetBool(String key)
    {
        string value;
        prefs.TryGetValue(key, out value);
        return Convert.ToBoolean(value);
    }

    public static bool GetBool(String key, bool defaultValue)
    {
        bool value = GetBool(key);
        return value == default(bool) ? defaultValue : value;
    }
    #endregion

    #region Integer32
    public static void SetInt(String key, int value)
    {
        prefs[key] = Convert.ToString(value);
    }

    public static int GetInt(String key)
    {
        string value;
        prefs.TryGetValue(key, out value);
        return Convert.ToInt32(value);
    }

    public static int GetInt(String key, int defaultValue)
    {
        int value = GetInt(key);
        return value == default(int) ? defaultValue : value;
    }
    #endregion

    #region Single
    public static void SetFloat(String key, float value)
    {
        prefs[key] = Convert.ToString(value);
    }

    public static float GetFloat(String key)
    {
        string value;
        prefs.TryGetValue(key, out value);
        return Convert.ToSingle(value);
    }

    public static float GetFloat(String key, float defaultValue)
    {
        float value = GetFloat(key);
        return value == default(float) ? defaultValue : value;
    }
    #endregion

    #region String
    public static void SetString(String key, String value)
    {
        prefs[key] = value;
    }

    public static String GetString(String key)
    {
        String value;
        prefs.TryGetValue(key, out value);
        return value;
    }

    public static String GetString(String key, String defaultValue)
    {
        String value = GetString(key);
        return value == default(String) ? defaultValue : value;
    }
    #endregion

    #region DateTime
    public static void SetDateTime(String key, DateTime value)
    {
        prefs[key] = Convert.ToString(value);
    }

    public static DateTime GetDateTime(String key)
    {
        string value;
        prefs.TryGetValue(key, out value);
        return Convert.ToDateTime(value);
    }

    public static DateTime GetDateTime(String key, DateTime defaultValue)
    {
        DateTime value = GetDateTime(key);
        return value == default(DateTime) ? defaultValue : value;
    }
    #endregion

    #region TimeSpan
    public static void SetTimeSpan(String key, TimeSpan value)
    {
        prefs[key] = Convert.ToString(value);
    }

    public static TimeSpan GetTimeSpan(String key)
    {
        string value;
        prefs.TryGetValue(key, out value);
        TimeSpan ts;
        if (TimeSpan.TryParse(value, out ts)) return ts;
        else return TimeSpan.Zero;
    }

    public static TimeSpan GetTimeSpan(String key, TimeSpan defaultValue)
    {
        TimeSpan value = GetTimeSpan(key);
        return value == TimeSpan.Zero ? defaultValue : value;
    }
    #endregion
}

#if UNITY_EDITOR
public class GameDataEditor : Editor
{
    // [MenuItem("Agraris/Game Data/Save", false, 50)]
    static void SaveGameData()
    {
        GameData.Init();
        GameData.Save();
    }

    [MenuItem("Agraris/Game Data/Clear", false, 51)]
    static void ClearGameData()
    {
        GameData.DeleteSave();
    }

    [MenuItem("Agraris/Game Data/Open in Explorer", false, 52)]
    static void OpenGameDataInExplorer()
    {
        string path = Directory.CreateDirectory(GameData.SaveDirectory).FullName;
        string file = Directory.EnumerateFiles(path).FirstOrDefault();

        if (!string.IsNullOrEmpty(file))
            EditorUtility.RevealInFinder(Path.Combine(path, file));
        else
            EditorUtility.RevealInFinder(path);
    }
}
#endif
