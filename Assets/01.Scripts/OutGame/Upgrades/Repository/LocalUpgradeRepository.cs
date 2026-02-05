using UnityEngine;

namespace OutGame.Upgrades.Repository
{
    public class LocalUpgradeRepository : IUpgradeRepository
    {
        private const string SaveKey = "upgrade_save";

        public UpgradeSaveData Load()
        {
            string json = PlayerPrefs.GetString(SaveKey, "");
            if (string.IsNullOrEmpty(json))
                return UpgradeSaveData.Default;
            return JsonUtility.FromJson<UpgradeSaveData>(json);
        }

        public void Save(UpgradeSaveData data)
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }
    }
}
