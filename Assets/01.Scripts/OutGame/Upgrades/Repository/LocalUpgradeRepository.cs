using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.Upgrades.Repository
{
    public class LocalUpgradeRepository : IUpgradeRepository
    {
        private const string SaveKey = "upgrade_save";

        public UniTask<UpgradeSaveData> Load()
        {
            string json = PlayerPrefs.GetString(SaveKey, "");
            if (string.IsNullOrEmpty(json))
                return UniTask.FromResult(UpgradeSaveData.Default);
            return UniTask.FromResult(JsonUtility.FromJson<UpgradeSaveData>(json));
        }

        public UniTaskVoid Save(UpgradeSaveData data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            return default;
        }
    }
}
