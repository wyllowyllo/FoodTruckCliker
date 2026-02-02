using UnityEngine;

namespace Goods.Repository
{
    public class LocalCurrencyRepository : ICurrencyRepository
    {
       
        private readonly string _userId;

        public LocalCurrencyRepository(string userId)
        {
            _userId = userId;
        }
        
        public void Save(CurrencySaveData saveData)
        {
            PlayerPrefs.SetInt(_userId, saveData.Currency);
        }

        public CurrencySaveData Load()
        {
            CurrencySaveData data = new CurrencySaveData();
            if (PlayerPrefs.HasKey(_userId))
            {
                data.Currency = PlayerPrefs.GetInt(_userId, 0);
            }
            return data;
        }
    }
}
