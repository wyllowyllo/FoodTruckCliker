using System.Collections.Generic;
using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
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

        private Dictionary<string, UpgradeData> _upgradeDataMap;
        private Dictionary<string, int> _upgradeLevels;

        public void Initialize(ICurrencyProvider currencyProvider, ICurrencyModifier currencyModifier)
        {
            _currencyProvider = currencyProvider;
            _currencyModifier = currencyModifier;

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
            // 승산 타입만 있으면 product 반환
            // 가산 타입만 있으면 sum 반환
            // 둘 다 있으면 (sum) * product 반환
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

            // 기본값 반환 (승산은 1, 가산은 0)
            return GetDefaultValueForType(targetType);
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

            // 자동 수익 변경 알림 (요리사/마케팅/트럭 업그레이드 시)
            if (_upgradeDataMap.TryGetValue(upgradeId, out UpgradeData data))
            {
                if (data.TargetType == UpgradeTargetType.AutoBase ||
                    data.TargetType == UpgradeTargetType.GlobalMultiplier ||
                    data.TargetType == UpgradeTargetType.TruckBonus)
                {
                    float autoIncome = CalculateAutoIncome();
                    GameEvents.RaiseAutoIncomeChanged(autoIncome);
                }
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

        private float GetDefaultValueForType(UpgradeTargetType targetType)
        {
            // Critical과 AutoBase는 기본값 0
            // 나머지 승산 타입은 기본값 1
            switch (targetType)
            {
                case UpgradeTargetType.Critical:
                case UpgradeTargetType.AutoBase:
                case UpgradeTargetType.ClickBase:
                    return 0f;
                default:
                    return 1f;
            }
        }

        private float CalculateAutoIncome()
        {
            float chefBase = GetValue(UpgradeTargetType.AutoBase);
            float marketing = GetValue(UpgradeTargetType.GlobalMultiplier);
            float truck = GetValue(UpgradeTargetType.TruckBonus);

            return chefBase * marketing * truck;
        }
    }
}
