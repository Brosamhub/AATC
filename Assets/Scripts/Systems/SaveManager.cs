using System.IO;
using UnityEngine;

namespace AroundTheCorner
{
    public class SaveManager
    {
        private readonly string savePath;

        public SaveManager()
        {
            savePath = Path.Combine(Application.persistentDataPath, "and-around-the-corner-save.json");
        }

        public bool HasSave()
        {
            return File.Exists(savePath);
        }

        public SaveData Load()
        {
            if (!HasSave())
            {
                return GameBalance.CreateDefaultSave();
            }

            try
            {
                string json = File.ReadAllText(savePath);
                SaveData loaded = JsonUtility.FromJson<SaveData>(json);
                if (loaded == null)
                {
                    return GameBalance.CreateDefaultSave();
                }

                return loaded;
            }
            catch
            {
                return GameBalance.CreateDefaultSave();
            }
        }

        public void Save(SaveData saveData)
        {
            string directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
        }

        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }

        public string GetSavePath()
        {
            return savePath;
        }
    }
}
