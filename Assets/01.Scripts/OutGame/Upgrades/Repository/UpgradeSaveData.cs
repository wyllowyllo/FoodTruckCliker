using System;
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    [Serializable]
    public struct UpgradeSaveData
    {
        public int[] Levels;
        public string LastSaveTime;

        public static UpgradeSaveData Default => new UpgradeSaveData
        {
            Levels = new int[(int)EUpgradeType.Count],
            LastSaveTime = DateTime.Now.ToString("o")
        };
    }
}
