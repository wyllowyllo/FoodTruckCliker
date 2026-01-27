using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using FoodTruckClicker.Upgrade;
using UnityEngine;

namespace FoodTruckClicker.AutoIncome
{
    /// <summary>
    /// 자동 수익 관리자
    /// 공식: 요리사 × 마케팅 × 트럭
    /// </summary>
    public class AutoIncomeManager : MonoBehaviour, IAutoIncomeProvider
    {
        [SerializeField]
        private float _incomeInterval = 1f;

        private IUpgradeProvider _upgradeProvider;
        private ICurrencyModifier _currencyModifier;

        private float _timer;
        private float _cachedIncomePerSecond;

        public float IncomePerSecond => _cachedIncomePerSecond;

        public void Initialize(IUpgradeProvider upgradeProvider, ICurrencyModifier currencyModifier)
        {
            _upgradeProvider = upgradeProvider;
            _currencyModifier = currencyModifier;

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

            // 요리사 기본 수익
            float chefBase = _upgradeProvider.GetValue(UpgradeTargetType.AutoBase);

            // 마케팅 배율
            float marketingMultiplier = _upgradeProvider.GetValue(UpgradeTargetType.GlobalMultiplier);

            // 트럭 보너스
            float truckMultiplier = _upgradeProvider.GetValue(UpgradeTargetType.TruckBonus);

            // 공식: 요리사 × 마케팅 × 트럭
            _cachedIncomePerSecond = chefBase * marketingMultiplier * truckMultiplier;

            GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
        }
    }
}
