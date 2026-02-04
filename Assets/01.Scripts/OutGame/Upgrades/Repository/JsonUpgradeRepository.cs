using System.Collections.Generic;
using System.IO;
using Upgrade.Domain;
using UnityEngine;

namespace Upgrade.Repository
{
    public class JsonUpgradeRepository : IUpgradeRepository
    {
        private readonly string filePath;

        private readonly string _userId;
    
        public JsonUpgradeRepository(string userId)
        {
            _userId = userId;
        
            filePath = Path.Combine(Application.persistentDataPath, $"{userId}_upgrade_save.json");
        }

        public void Save(UpgradeSaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }

        public UpgradeSaveData Load()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("File not found: " + filePath);
                return UpgradeSaveData.Default;
            }

            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<UpgradeSaveData>(json);
        }

        public int LoadLevel(EUpgradeType type)
        {
            throw new System.NotImplementedException();
        }

        public void SaveLevel(EUpgradeType type, int level)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<EUpgradeType, int> LoadAll(IEnumerable<EUpgradeType> types)
        {
            throw new System.NotImplementedException();
        }
    }
}