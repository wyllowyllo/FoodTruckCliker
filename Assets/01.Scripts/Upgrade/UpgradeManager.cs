using System;
using System.Collections.Generic;
using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using FoodTruckClicker.Menu;
using UnityEngine;

namespace FoodTruckClicker.Upgrade
{
    /// <summary>
    /// 업그레이드 관리자
    /// </summary>
    public class UpgradeManager : MonoBehaviour, IUpgradeProvider
    {
        [SerializeField]
        private UpgradeData[] _upgrades;

        private ICurrencyProvider _currencyProvider;
        private ICurrencyModifier _currencyModifier;
        private IMenuProvider _menuProvider;
        private Action<int> _onMenuUnlockChanged;

        private Dictionary<string, UpgradeData> _upgradeDataMap;
        private Dictionary<string, int> _upgradeLevels;

        public void Initialize(
            ICurrencyProvider currencyProvider,
            ICurrencyModifier currencyModifier,
            IMenuProvider menuProvider = null,
            Action<int> onMenuUnlockChanged = null)
        {
            _currencyProvider = currencyProvider;
            _currencyModifier = currencyModifier;
            _menuProvider = menuProvider;
            _onMenuUnlockChanged = onMenuUnlockChanged;

            _upgradeDataMap = new Dictionary<string, UpgradeData>();
            _upgradeLevels = new Dictionary<string, int>();

            foreach (var upgrade in _upgrades)
            {
                if (upgrade != null && !string.IsNullOrEmpty(upgrade.UpgradeId))
                {
                    _upgradeDataMap[upgrade.UpgradeId] = upgrade;
                    _upgradeLevels[upgrade.UpgradeId] = 0;
                }
            }
        }

        public float GetValue(UpgradeTargetType targetType)
        {
            float additiveSum = 0f;
            float multiplicativeProduct = 1f;

            foreach (var upgrade in _upgrades)
            {
                if (upgrade == null || upgrade.TargetType != targetType)
                {
                    continue;
                }

                int level = GetLevel(upgrade.UpgradeId);
                if (level <= 0)
                {
                    continue;
                }

                float value = upgrade.GetValue(level);

                if (upgrade.ModifierType == ModifierType.Additive)
                {
                    additiveSum += value;
                }
                else
                {
                    multiplicativeProduct *= value;
                }
            }

            // 가산 + 승산 결합
            if (additiveSum > 0 && multiplicativeProduct > 1f)
            {
                return additiveSum * multiplicativeProduct;
            }
            else if (additiveSum > 0)
            {
                return additiveSum;
            }
            else if (multiplicativeProduct != 1f)
            {
                return multiplicativeProduct;
            }

            return GetDefaultValueForType(targetType);
        }

        /// <summary>
        /// 정수 값 반환 (요리사 수, 크리티컬 개수, 메뉴 레벨 등)
        /// </summary>
        public int GetIntValue(UpgradeTargetType targetType)
        {
            return Mathf.FloorToInt(GetValue(targetType));
        }

        public int GetLevel(string upgradeId)
        {
            if (_upgradeLevels != null && _upgradeLevels.TryGetValue(upgradeId, out int level))
            {
                return level;
            }
            return 0;
        }

        public int GetNextLevelCost(string upgradeId)
        {
            if (!_upgradeDataMap.TryGetValue(upgradeId, out UpgradeData data))
            {
                return 0;
            }

            int currentLevel = GetLevel(upgradeId);
            int nextLevel = currentLevel + 1;

            if (nextLevel > data.MaxLevel)
            {
                return 0;
            }

            return data.GetCost(nextLevel);
        }

        public bool CanUpgrade(string upgradeId)
        {
            int cost = GetNextLevelCost(upgradeId);
            if (cost <= 0)
            {
                return false;
            }

            return _currencyProvider.HasEnough(cost);
        }

        /// <summary>
        /// 업그레이드 구매 시도
        /// </summary>
        public bool TryPurchase(string upgradeId)
        {
            if (!CanUpgrade(upgradeId))
            {
                return false;
            }

            int cost = GetNextLevelCost(upgradeId);
            if (!_currencyModifier.SpendGold(cost))
            {
                return false;
            }

            _upgradeLevels[upgradeId]++;
            int newLevel = _upgradeLevels[upgradeId];

            GameEvents.RaiseUpgradePurchased(upgradeId, newLevel);

            if (_upgradeDataMap.TryGetValue(upgradeId, out UpgradeData data))
            {
                HandleUpgradeEffects(data);
            }

            return true;
        }

        /// <summary>
        /// 업그레이드 데이터 조회
        /// </summary>
        public UpgradeData GetUpgradeData(string upgradeId)
        {
            if (_upgradeDataMap.TryGetValue(upgradeId, out UpgradeData data))
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// 모든 업그레이드 데이터 반환
        /// </summary>
        public UpgradeData[] GetAllUpgrades()
        {
            return _upgrades;
        }

        private void HandleUpgradeEffects(UpgradeData data)
        {
            switch (data.TargetType)
            {
                case UpgradeTargetType.ChefCount:
                case UpgradeTargetType.CookingSpeed:
                    NotifyAutoIncomeChanged();
                    break;

                case UpgradeTargetType.MenuUnlock:
                    int menuLevel = GetIntValue(UpgradeTargetType.MenuUnlock);
                    _onMenuUnlockChanged?.Invoke(menuLevel);
                    NotifyAutoIncomeChanged();
                    break;
            }
        }

        private void NotifyAutoIncomeChanged()
        {
            float autoIncome = CalculateAutoIncome();
            GameEvents.RaiseAutoIncomeChanged(autoIncome);
        }

        private float GetDefaultValueForType(UpgradeTargetType targetType)
        {
            switch (targetType)
            {
                case UpgradeTargetType.CriticalChance:
                case UpgradeTargetType.ChefCount:
                case UpgradeTargetType.MenuUnlock:
                    return 0f;
                case UpgradeTargetType.ClickRevenue:
                case UpgradeTargetType.CookingSpeed:
                    return 1f;
                case UpgradeTargetType.CriticalDamage:
                    return 1f;
                default:
                    return 1f;
            }
        }

        /// <summary>
        /// 자동 수익 계산: 요리사 수 × 클릭 수익 × 요리 속도
        /// </summary>
        public float CalculateAutoIncome()
        {
            int chefCount = GetIntValue(UpgradeTargetType.ChefCount);
            float cookingSpeed = GetValue(UpgradeTargetType.CookingSpeed);

            if (chefCount <= 0)
            {
                return 0f;
            }

            int menuPrice = _menuProvider?.CurrentMenuPrice ?? 10;
            float clickRevenue = GetValue(UpgradeTargetType.ClickRevenue);
            float baseClickIncome = menuPrice * clickRevenue;

            return chefCount * baseClickIncome * cookingSpeed;
        }
    }
}
