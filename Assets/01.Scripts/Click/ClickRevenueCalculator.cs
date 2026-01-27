using FoodTruckClicker.Upgrade;
using UnityEngine;

namespace FoodTruckClicker.Click
{
    /// <summary>
    /// 클릭 수익 계산기
    /// 공식: (1 + 메뉴추가) × 고급재료 × 크리티컬 × 마케팅 × 트럭
    /// </summary>
    public class ClickRevenueCalculator : IRevenueCalculator
    {
        private readonly IUpgradeProvider _upgradeProvider;
        private readonly float _baseRevenue;
        private readonly float _criticalMultiplier;

        public ClickRevenueCalculator(
            IUpgradeProvider upgradeProvider,
            float baseRevenue = 1f,
            float criticalMultiplier = 2f)
        {
            _upgradeProvider = upgradeProvider;
            _baseRevenue = baseRevenue;
            _criticalMultiplier = criticalMultiplier;
        }

        public ClickResult Calculate()
        {
            // 메뉴 추가 (가산)
            float menuAddBonus = _upgradeProvider.GetValue(UpgradeTargetType.ClickBase);

            // 고급 재료 (승산)
            float ingredientMultiplier = _upgradeProvider.GetValue(UpgradeTargetType.ClickMultiplier);

            // 마케팅 (승산)
            float marketingMultiplier = _upgradeProvider.GetValue(UpgradeTargetType.GlobalMultiplier);

            // 트럭 보너스 (승산)
            float truckMultiplier = _upgradeProvider.GetValue(UpgradeTargetType.TruckBonus);

            // 크리티컬 확률
            float criticalChance = _upgradeProvider.GetValue(UpgradeTargetType.Critical);
            bool isCritical = Random.Range(0f, 1f) < criticalChance;
            float criticalMult = isCritical ? _criticalMultiplier : 1f;

            // 공식: (1 + 메뉴추가) × 고급재료 × 크리티컬 × 마케팅 × 트럭
            float revenue = (_baseRevenue + menuAddBonus)
                            * ingredientMultiplier
                            * criticalMult
                            * marketingMultiplier
                            * truckMultiplier;

            return new ClickResult(revenue, isCritical);
        }
    }
}
