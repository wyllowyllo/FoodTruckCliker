using System;
#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Firestore;
#endif
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    [Serializable]
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreData]
#endif
    public class UpgradeSaveData
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty]
#endif
        public int[] Levels { get; set; }

#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty]
#endif
        public string LastSaveTime { get; set; }

        public static UpgradeSaveData Default => new UpgradeSaveData
        {
            Levels = new int[(int)EUpgradeType.Count],
            LastSaveTime = DateTime.Now.ToString("o")
        };
    }
}
