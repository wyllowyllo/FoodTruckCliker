using UnityEngine;

namespace Goods.Repository
{
    public class LocalCurrencyRepository : ICurrencyRepository
    {
       
        private const string KEY_PREFIX = "currency_";
        private readonly string _saveKey;

        public LocalCurrencyRepository(string userId)
        {
            _saveKey = KEY_PREFIX + userId;
        }
        
        public void Save(CurrencySaveData saveData)
        {
            PlayerPrefs.SetInt(_saveKey, saveData.Currency);
        }

        public CurrencySaveData Load()
        {
            CurrencySaveData data = new CurrencySaveData();
            if (PlayerPrefs.HasKey(_saveKey))
            {
                data.Currency = PlayerPrefs.GetInt(_saveKey, 0);
            }
            return data;
        }
    }
}
