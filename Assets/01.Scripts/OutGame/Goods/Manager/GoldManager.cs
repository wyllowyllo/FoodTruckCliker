

using Events;
using Goods.Domain;
using Goods.Repository;
using UnityEngine;

namespace Goods.Manager
{
    public class GoldManager : MonoBehaviour
    {
        [SerializeField] private long _startingGold = 0;

        private Currency _gold;
        private ICurrencyRepository _repository;

        public long CurrentGold => _gold.Value;

        public void Initialize(string userId)
        {
            _repository = new LocalCurrencyRepository(userId);

            CurrencySaveData savedGold = _repository.Load();
            long initialGold = savedGold.Currency > 0 ? savedGold.Currency : _startingGold;
            _gold = new Currency(initialGold);
        }

        private void Start()
        {
            if (_gold.Value == 0 && _startingGold > 0)
            {
                _gold = new Currency(_startingGold);
            }

            GameEvents.RaiseGoldChanged(_gold.Value);
        }

       

        public void AddGold(long amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[GoldManager] AddGold 호출됨 - 잘못된 금액: {amount}");
                return;
            }

            _gold = _gold + new Currency(amount);
            OnGoldChanged();
        }

        public bool SpendGold(long amount)
        {
            if (amount <= 0 || !HasEnough(amount))
            {
                return false;
            }

            _gold = _gold - new Currency(amount);
            OnGoldChanged();
            return true;
        }
        
        public bool HasEnough(long amount)
        {
            return _gold.Value >= amount;
        }
        
        private void OnGoldChanged()
        {
            GameEvents.RaiseGoldChanged(_gold.Value);
            
            CurrencySaveData savedGold = new CurrencySaveData();
            savedGold.Currency = _gold.Value;
            _repository?.Save(savedGold);
        }
    }
}
