using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.Goods.Repository
{
    public class LocalCurrencyRepository : ICurrencyRepository
    {
       
        private const string KEY_PREFIX = "currency_";
        private readonly string _saveKey;

        public LocalCurrencyRepository(string userId)
        {
            _saveKey = KEY_PREFIX + userId;
        }
        
        public async UniTask Save(CurrencySaveData saveData)
        {
            PlayerPrefs.SetString(_saveKey, saveData.Currency.ToString());
        }

        public async UniTask<CurrencySaveData> Load()
        {
            CurrencySaveData data = new CurrencySaveData();
            if (PlayerPrefs.HasKey(_saveKey))
            {
                string saved = PlayerPrefs.GetString(_saveKey, "0");
                data.Currency = long.Parse(saved);
            }
            return data;
        }
    }
}
