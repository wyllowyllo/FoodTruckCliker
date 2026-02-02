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
          
            PlayerPrefs.SetString(_userId, saveData.Currency.ToString("G17"));
            
        }

        public CurrencySaveData Load()
        {
            CurrencySaveData data = new CurrencySaveData();
            if (PlayerPrefs.HasKey(_userId))
            {
                data.Currency = int.Parse(PlayerPrefs.GetString(_userId, "0"));
            }
            return data;
        }
    }
}
