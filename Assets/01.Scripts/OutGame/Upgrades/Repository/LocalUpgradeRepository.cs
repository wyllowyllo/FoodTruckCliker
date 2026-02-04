using System.Collections.Generic;
using OutGame.Upgrades.Domain;
using UnityEngine;

namespace OutGame.Upgrades.Repository
{
    public class LocalUpgradeRepository : IUpgradeRepository
    {
        private const string KeyPrefix = "upgrade";

        public int LoadLevel(EUpgradeType type)
        {
            return PlayerPrefs.GetInt(KeyPrefix + type.ToString(), 0);
        }

        public void SaveLevel(EUpgradeType type, int level)
        {
            PlayerPrefs.SetInt(KeyPrefix + type.ToString(), level);
            PlayerPrefs.Save();
        }

        public Dictionary<EUpgradeType, int> LoadAll(IEnumerable<EUpgradeType> types)
        {
            var levels = new Dictionary<EUpgradeType, int>();

            foreach (EUpgradeType type in types)
            {
                levels[type] = LoadLevel(type);
            }

            return levels;
        }
    }
}
