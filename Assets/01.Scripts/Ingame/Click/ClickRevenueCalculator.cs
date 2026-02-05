using Menu;
using OutGame.Upgrades.Domain;
using OutGame.Upgrades.Manager;
using UnityEngine;

namespace Click
{
    public class ClickRevenueCalculator
    {
        private readonly UpgradeManager _upgradeProvider;
        private readonly MenuManager _menuProvider;

        public ClickRevenueCalculator(
            UpgradeManager upgradeProvider,
            MenuManager menuProvider)
        {
            _upgradeProvider = upgradeProvider;
            _menuProvider = menuProvider;
        }

        public ClickResult Calculate()
        {
            MenuData selectedMenu = _menuProvider?.GetRandomMenu();

            int menuPrice = _menuProvider?.GetFinalPrice(selectedMenu) ?? 10;

            var clickUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.ClickRevenue);
            float clickRevenueMultiplier = clickUpgrade?.Effect ?? 1f;

            var critChanceUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.CriticalChance);
            float criticalChance = critChanceUpgrade?.Effect ?? 0f;
            bool isCritical = Random.Range(0f, 1f) < criticalChance;

            var critProfitUpgrade = _upgradeProvider.GetUpgradeData(EUpgradeType.CriticalProfit);
            int criticalDamage = Mathf.FloorToInt(critProfitUpgrade?.Effect ?? 1f);
            int menuCount = isCritical ? Mathf.Max(1, criticalDamage) : 1;

            float revenue = menuPrice * clickRevenueMultiplier * menuCount;

            return new ClickResult(revenue, isCritical, menuCount, selectedMenu);
        }
    }
}
