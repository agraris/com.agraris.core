using UnityEditor;
using System.Linq;
using System.IO;

namespace Agraris.Tools.Core
{
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
}
