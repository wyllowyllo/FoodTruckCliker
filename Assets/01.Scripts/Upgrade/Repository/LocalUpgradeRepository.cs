using System.Collections.Generic;
using UnityEngine;

namespace Upgrade.Repository
{
    public class LocalUpgradeRepository : IUpgradeRepository
    {
        private const string KeyPrefix = "upgrade";

        public int LoadLevel(string upgradeId)
        {
            return PlayerPrefs.GetInt(KeyPrefix + upgradeId, 0);
        }

        public void SaveLevel(string upgradeId, int level)
        {
            PlayerPrefs.SetInt(KeyPrefix + upgradeId, level);
            PlayerPrefs.Save();
        }

        public Dictionary<string, int> LoadAll(IEnumerable<string> upgradeIds)
        {
            var levels = new Dictionary<string, int>();

            foreach (string id in upgradeIds)
            {
                levels[id] = LoadLevel(id);
            }

            return levels;
        }
    }
}
