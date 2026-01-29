using FoodTruckClicker.Menu;
using FoodTruckClicker.Upgrade;
using UnityEngine;

namespace FoodTruckClicker.Click
{
    /// <summary>
    /// 클릭 수익 계산기
    /// 공식:
    /// - 일반: (랜덤메뉴 기본가격 × 메뉴가격배율) × 클릭수익배율
    /// - 크리티컬: (랜덤메뉴 기본가격 × 메뉴가격배율) × 클릭수익배율 × 크리티컬데미지(메뉴개수)
    /// </summary>
    public class ClickRevenueCalculator : IRevenueCalculator
    {
        private readonly IUpgradeProvider _upgradeProvider;
        private readonly IMenuProvider _menuProvider;

        public ClickRevenueCalculator(
            IUpgradeProvider upgradeProvider,
            IMenuProvider menuProvider)
        {
            _upgradeProvider = upgradeProvider;
            _menuProvider = menuProvider;
        }

        public ClickResult Calculate()
        {
            // 랜덤 메뉴 선택
            MenuData selectedMenu = _menuProvider?.GetRandomMenu();

            // 메뉴 가격 (기본가격 × 메뉴가격배율)
            int menuPrice = _menuProvider?.GetFinalPrice(selectedMenu) ?? 10;

            // 클릭 수익 배율
            float clickRevenueMultiplier = _upgradeProvider.GetValue(EUpgradeTargetType.ClickRevenue);

            // 크리티컬 확률
            float criticalChance = _upgradeProvider.GetValue(EUpgradeTargetType.CriticalChance);
            bool isCritical = Random.Range(0f, 1f) < criticalChance;

            // 크리티컬 데미지 (메뉴 개수)
            int criticalDamage = _upgradeProvider.GetIntValue(EUpgradeTargetType.CriticalProfit);
            int menuCount = isCritical ? Mathf.Max(1, criticalDamage) : 1;

            // 공식: 메뉴가격 × 클릭수익배율 × 메뉴개수
            float revenue = menuPrice * clickRevenueMultiplier * menuCount;

            return new ClickResult(revenue, isCritical, menuCount, selectedMenu);
        }
    }
}
