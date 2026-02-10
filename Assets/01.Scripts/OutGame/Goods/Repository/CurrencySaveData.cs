using System;
#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Firestore;
#endif

namespace OutGame.Goods.Repository
{

    [Serializable]
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreData]
#endif
    public class CurrencySaveData
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty]
#endif
        public long Currency { get; set; }

#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty]
#endif
        public string LastSaveTime { get; set; }

        // 재화 기본값
        public static CurrencySaveData Default => new CurrencySaveData()
        {
            Currency = 0,
            LastSaveTime = DateTime.Now.ToString("o")
        };

    }
}
