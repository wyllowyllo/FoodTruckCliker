using System;
using Firebase.Firestore;
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    [Serializable]
    [FirestoreData]
    public class UpgradeSaveData
    {
        [FirestoreProperty]
        public int[] Levels { get; set; }

        [FirestoreProperty]
        public string LastSaveTime { get; set; }

        public static UpgradeSaveData Default => new UpgradeSaveData
        {
            Levels = new int[(int)EUpgradeType.Count],
            LastSaveTime = DateTime.Now.ToString("o")
        };
    }
}
