

using Events;
using Goods.Domain;
using Goods.Repository;
using UnityEngine;

namespace Goods.Manager
{
    public class GoldManager : MonoBehaviour
    {
        [SerializeField] private int _startingGold = 0;

        private Domain.Currency _gold;
        private ICurrencyRepository _repository;

        public int CurrentGold => _gold.Value;

        public void Initialize(ICurrencyRepository repository)
        {
            _repository = repository;

            int savedGold = _repository.Load();
            int initialGold = savedGold > 0 ? savedGold : _startingGold;
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

       

        public void AddGold(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[GoldManager] AddGold 호출됨 - 잘못된 금액: {amount}");
                return;
            }

            _gold = _gold + new Currency(amount);
            OnGoldChanged();
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0 || !HasEnough(amount))
            {
                return false;
            }

            _gold = _gold - new Currency(amount);
            OnGoldChanged();
            return true;
        }
        
        public bool HasEnough(int amount)
        {
            return _gold.Value >= amount;
        }
        
        private void OnGoldChanged()
        {
            GameEvents.RaiseGoldChanged(_gold.Value);
            _repository?.Save(_gold.Value);
        }
    }
}
