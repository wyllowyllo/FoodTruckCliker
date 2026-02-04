using System;
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    [Serializable]
    public class UpgradeSaveData
    {
        // 레벨 배열 (EUpgradeType 순서대로 저장)
        public int[] Levels;
    
       

        /// <summary>기본값 (새 게임)</summary>
        public static UpgradeSaveData Default => new UpgradeSaveData
        {
            Levels = new int[(int)EUpgradeType.Count], // 모든 레벨 0
           
        };
    }
}