using System.IO;
using Cysharp.Threading.Tasks;
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

        public UniTask<UpgradeSaveData> Load()
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("File not found: " + _filePath);
                return UniTask.FromResult(UpgradeSaveData.Default);
            }

            string json = File.ReadAllText(_filePath);
            return UniTask.FromResult(JsonUtility.FromJson<UpgradeSaveData>(json));
        }

        public UniTaskVoid Save(UpgradeSaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_filePath, json);
            return default;
        }
    }
}
