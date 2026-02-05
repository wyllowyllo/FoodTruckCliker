using System;
using Firebase.Firestore;

namespace OutGame.Goods.Repository
{
    
    [Serializable]
    [FirestoreData]
    public class CurrencySaveData
    {
        [FirestoreProperty]
        public long Currency { get; set; }

        // 재화 기본값
        public static CurrencySaveData Default => new CurrencySaveData()
        {
            
        };
        
    }
}