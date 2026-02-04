using System;
using Firebase.Firestore;

namespace Goods.Repository
{
    
    [Serializable]
    [FirestoreData]
    public class CurrencySaveData
    {
        // 재화 
        public long Currency;

        // 재화 기본값
        public static CurrencySaveData Default => new CurrencySaveData()
        {
            
        };
        
    }
}