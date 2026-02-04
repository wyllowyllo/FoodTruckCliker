using System;
using System.Collections.Generic;
using Events;
using Goods.Manager;
using Menu;
using Upgrade.Domain;
using Upgrade.Repository;
using UnityEngine;

namespace Upgrade.Manager
{
    /// <summary>
    /// 업그레이드 관리자
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField]
        private UpgradeData[] _upgrades;

        private GoldManager _goldManager;
        private MenuManager _menuProvider;
        private Action<int, float> _onFoodTruckUpgraded;
        private IUpgradeRepository _repository;

        private Dictionary<EUpgradeType, UpgradeData> _upgradeDataMap;
        private Dictionary<EUpgradeType, int> _upgradeLevels;

        public void Initialize(
            GoldManager goldManager,
            string userId,
            MenuManager menuProvider = null,
            Action<int, float> onFoodTruckUpgraded = null)
        {
            _goldManager = goldManager;
            _repository = new LocalUpgradeRepository();
            _menuProvider = menuProvider;
            _onFoodTruckUpgraded = onFoodTruckUpgraded;

            _upgradeDataMap = new Dictionary<EUpgradeType, UpgradeData>();

            var upgradeTypes = new List<EUpgradeType>();
            foreach (var upgrade in _upgrades)
            {
                if (upgrade != null)
                {
                    _upgradeDataMap[upgrade.Type] = upgrade;
                    upgradeTypes.Add(upgrade.Type);
                }
            }

            _upgradeLevels = _repository.LoadAll(upgradeTypes);

            ApplyLoadedEffects();
        }

        /// <summary>
        /// 저장된 업그레이드 효과 적용 (게임 로드 시)
        /// </summary>
        private void ApplyLoadedEffects()
        {
            foreach (var upgrade in _upgrades)
            {
                if (upgrade == null)
                {
                    continue;
                }

                int level = GetLevel(upgrade.Type);
                if (level <= 0)
                {
                    continue;
                }

                HandleUpgradeEffects(upgrade);
            }
        }

        public float GetValue(EUpgradeType type)
        {
            if (!_upgradeDataMap.TryGetValue(type, out UpgradeData upgrade))
            {
                return GetDefaultValueForType(type);
            }

            int level = GetLevel(type);
            if (level <= 0)
            {
                return GetDefaultValueForType(type);
            }

            return upgrade.GetValue(level);
        }

        /// <summary>
        /// 정수 값 반환 (요리사 수, 크리티컬 개수, 메뉴 레벨 등)
        /// </summary>
        public int GetIntValue(EUpgradeType type)
        {
            return Mathf.FloorToInt(GetValue(type));
        }

        public int GetLevel(EUpgradeType type)
        {
            if (_upgradeLevels != null && _upgradeLevels.TryGetValue(type, out int level))
            {
                return level;
            }
            return 0;
        }

        public long GetNextLevelCost(EUpgradeType type)
        {
            if (!_upgradeDataMap.TryGetValue(type, out UpgradeData data))
            {
                return 0;
            }

            int currentLevel = GetLevel(type);
            int nextLevel = currentLevel + 1;

            if (nextLevel > data.MaxLevel)
            {
                return 0;
            }

            return data.GetCost(nextLevel);
        }

        public bool CanUpgrade(EUpgradeType type)
        {
            long cost = GetNextLevelCost(type);
            if (cost <= 0)
            {
                return false;
            }

            return _goldManager.HasEnough(cost);
        }

        /// <summary>
        /// 업그레이드 구매 시도
        /// </summary>
        public bool TryPurchase(EUpgradeType type)
        {
            if (!CanUpgrade(type))
            {
                Debug.LogWarning($"[UpgradeManager] 업그레이드 불가 - Type: {type}, " +
                    $"DataMap 포함: {_upgradeDataMap.ContainsKey(type)}, " +
                    $"비용: {GetNextLevelCost(type)}, " +
                    $"현재 레벨: {GetLevel(type)}");
                return false;
            }

            long cost = GetNextLevelCost(type);
            if (!_goldManager.SpendGold(cost))
            {
                return false;
            }

            _upgradeLevels[type]++;
            int newLevel = _upgradeLevels[type];

            _repository.SaveLevel(type, newLevel);

            if (_upgradeDataMap.TryGetValue(type, out UpgradeData data))
            {
                Debug.Log($"[UpgradeManager] 업그레이드 성공 - {data.DisplayName}({type}) " +
                    $"Lv.{newLevel}, Type: {data.Type}, Value: {data.GetValue(newLevel)}");
            }

            GameEvents.RaiseUpgradePurchased(type, newLevel);

            if (data != null)
            {
                HandleUpgradeEffects(data);
            }

            return true;
        }

        /// <summary>
        /// 업그레이드 데이터 조회
        /// </summary>
        public UpgradeData GetUpgradeData(EUpgradeType type)
        {
            if (_upgradeDataMap.TryGetValue(type, out UpgradeData data))
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
            switch (data.Type)
            {
                case EUpgradeType.ChefCount:
                case EUpgradeType.CookingSpeed:
                    NotifyAutoIncomeChanged();
                    break;

                case EUpgradeType.FoodTruck:
                    int unlockLevel = GetLevel(data.Type);
                    float priceMultiplier = GetValue(EUpgradeType.FoodTruck);
                    _onFoodTruckUpgraded?.Invoke(unlockLevel, priceMultiplier);
                    NotifyAutoIncomeChanged();
                    break;
            }
        }

        private void NotifyAutoIncomeChanged()
        {
            float autoIncome = CalculateAutoIncome();
            GameEvents.RaiseAutoIncomeChanged(autoIncome);
        }

        private float GetDefaultValueForType(EUpgradeType type)
        {
            switch (type)
            {
                case EUpgradeType.CriticalChance:
                case EUpgradeType.ChefCount:
                    return 0f;
                case EUpgradeType.FoodTruck:
                    return 1f;
                case EUpgradeType.ClickRevenue:
                case EUpgradeType.CookingSpeed:
                    return 1f;
                case EUpgradeType.CriticalProfit:
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
            int chefCount = GetIntValue(EUpgradeType.ChefCount);
            float cookingSpeed = GetValue(EUpgradeType.CookingSpeed);

            if (chefCount <= 0)
            {
                return 0f;
            }

            float menuPrice = _menuProvider?.AveragePrice ?? 10f;
            float clickRevenue = GetValue(EUpgradeType.ClickRevenue);
            float baseClickIncome = menuPrice * clickRevenue;

            return chefCount * baseClickIncome * cookingSpeed;
        }
    }
}
