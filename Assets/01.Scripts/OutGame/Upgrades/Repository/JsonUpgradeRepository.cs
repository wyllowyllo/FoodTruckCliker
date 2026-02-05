using System.IO;
using UnityEngine;

namespace OutGame.Upgrades.Repository
{
    public class JsonUpgradeRepository : IUpgradeRepository
    {
        private readonly string _filePath;

        public JsonUpgradeRepository(string userId)
        {
            _filePath = Path.Combine(Application.persistentDataPath, $"{userId}_upgrade_save.json");
        }

        public UpgradeSaveData Load()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("File not found: " + _filePath);
                return UpgradeSaveData.Default;
            }

            string json = File.ReadAllText(_filePath);
            return JsonUtility.FromJson<UpgradeSaveData>(json);
        }

        public void Save(UpgradeSaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_filePath, json);
        }
    }
}
