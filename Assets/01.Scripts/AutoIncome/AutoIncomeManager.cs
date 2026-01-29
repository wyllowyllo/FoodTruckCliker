using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using FoodTruckClicker.Menu;
using FoodTruckClicker.Upgrade;
using UnityEngine;

namespace FoodTruckClicker.AutoIncome
{
    /// <summary>
    /// 자동 수익 관리자
    /// 공식: 요리사 수 × 클릭 수익 × 요리 속도 배율
    /// </summary>
    public class AutoIncomeManager : MonoBehaviour, IAutoIncomeProvider
    {
        [SerializeField]
        private float _incomeInterval = 1f;

        private IUpgradeProvider _upgradeProvider;
        private ICurrencyModifier _currencyModifier;
        private IMenuProvider _menuProvider;

        private float _timer;
        private float _cachedIncomePerSecond;

        public float IncomePerSecond => _cachedIncomePerSecond;

        public void Initialize(
            IUpgradeProvider upgradeProvider,
            ICurrencyModifier currencyModifier,
            IMenuProvider menuProvider)
        {
            _upgradeProvider = upgradeProvider;
            _currencyModifier = currencyModifier;
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
            if (_upgradeProvider == null || _currencyModifier == null)
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
            int goldToAdd = Mathf.FloorToInt(_cachedIncomePerSecond * _incomeInterval);

            if (goldToAdd > 0)
            {
                _currencyModifier.AddGold(goldToAdd);
            }
        }

        private void HandleUpgradePurchased(string upgradeId, int newLevel)
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

            // 요리사 수
            int chefCount = _upgradeProvider.GetIntValue(UpgradeTargetType.ChefCount);

            if (chefCount <= 0)
            {
                _cachedIncomePerSecond = 0f;
                GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
                return;
            }

            // 메뉴 가격
            int menuPrice = _menuProvider?.CurrentMenuPrice ?? 10;

            // 클릭 수익 배율
            float clickRevenue = _upgradeProvider.GetValue(UpgradeTargetType.ClickRevenue);

            // 요리 속도 배율
            float cookingSpeed = _upgradeProvider.GetValue(UpgradeTargetType.CookingSpeed);

            // 기본 클릭 수익 = 메뉴 가격 × 클릭 수익 배율
            float baseClickIncome = menuPrice * clickRevenue;

            // 공식: 요리사 수 × 클릭 수익 × 요리 속도
            _cachedIncomePerSecond = chefCount * baseClickIncome * cookingSpeed;

            GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
        }
    }
}
