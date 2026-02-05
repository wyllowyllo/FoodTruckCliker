using Events;
using Goods.Manager;
using Menu;
using OutGame.Upgrades.Domain;
using OutGame.Upgrades.Manager;
using UnityEngine;

namespace AutoIncome
{
    public class AutoIncomeManager : MonoBehaviour
    {
        [SerializeField]
        private float _incomeInterval = 1f;

        private UpgradeManager _upgradeProvider;
        private CurrencyManager _currencyManager;
        private MenuManager _menuProvider;

        private float _timer;
        private float _cachedIncomePerSecond;

        public float IncomePerSecond => _cachedIncomePerSecond;

        public void Initialize(
            UpgradeManager upgradeProvider,
            CurrencyManager currencyManager,
            MenuManager menuProvider)
        {
            _upgradeProvider = upgradeProvider;
            _currencyManager = currencyManager;
            _menuProvider = menuProvider;

            RecalculateIncome();
        }

        private void OnEnable()
        {
            GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
        }

        private void OnDisable()
        {
            GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        private void Update()
        {
            if (_upgradeProvider == null || _currencyManager == null)
            {
                return;
            }

            if (_cachedIncomePerSecond <= 0)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer >= _incomeInterval)
            {
                _timer -= _incomeInterval;
                ProcessAutoIncome();
            }
        }

        private void ProcessAutoIncome()
        {
            long goldToAdd = (long)(_cachedIncomePerSecond * _incomeInterval);

            if (goldToAdd > 0)
            {
                _currencyManager.AddGold(goldToAdd);
                GameEvents.RaiseRevenueEarned(goldToAdd, false, 1, true);
            }
        }

        private void HandleUpgradePurchased(EUpgradeType type, int newLevel)
        {
            RecalculateIncome();
        }

        private void RecalculateIncome()
        {
            if (_upgradeProvider == null)
            {
                _cachedIncomePerSecond = 0f;
                return;
            }

            var chefUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.ChefCount);
            int chefCount = Mathf.FloorToInt(chefUpgrade?.Effect ?? 0f);

            if (chefCount <= 0)
            {
                _cachedIncomePerSecond = 0f;
                GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
                return;
            }

            float menuPrice = _menuProvider?.AveragePrice ?? 10f;

            var clickUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.ClickRevenue);
            float clickRevenue = clickUpgrade?.Effect ?? 1f;

            var speedUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.CookingSpeed);
            float cookingSpeed = speedUpgrade?.Effect ?? 1f;

            float baseClickIncome = menuPrice * clickRevenue;

            _cachedIncomePerSecond = chefCount * baseClickIncome * cookingSpeed;

            GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
        }
    }
}
