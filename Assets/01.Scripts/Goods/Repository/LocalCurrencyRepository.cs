using UnityEngine;

namespace Goods.Repository
{
    public class LocalCurrencyRepository : ICurrencyRepository
    {
        private const string GoldKey = "gold";

        public int Load()
        {
            return PlayerPrefs.GetInt(GoldKey, 0);
        }

        public void Save(int amount)
        {
            PlayerPrefs.SetInt(GoldKey, amount);
            PlayerPrefs.Save();
        }
    }
}
