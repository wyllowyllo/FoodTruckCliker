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
            PlayerPrefs.SetString(_saveKey, saveData.Currency.ToString());
        }

        public CurrencySaveData Load()
        {
            CurrencySaveData data = new CurrencySaveData();
            if (PlayerPrefs.HasKey(_saveKey))
            {
                string saved = PlayerPrefs.GetString(_saveKey, "0");
                long.TryParse(saved, out data.Currency);
            }
            return data;
        }
    }
}
