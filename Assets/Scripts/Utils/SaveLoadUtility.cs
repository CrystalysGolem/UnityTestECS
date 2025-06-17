using UnityEngine;
using Data;

namespace Utils
{
    public static class SaveLoadUtility
    {
        private const string Key = "SaveData";

        public static SaveData Load()
        {
            if (!PlayerPrefs.HasKey(Key)) return null;
            var json = PlayerPrefs.GetString(Key);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public static void Save(SaveData data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
