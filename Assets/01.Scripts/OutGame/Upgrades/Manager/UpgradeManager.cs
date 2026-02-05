using System.Collections.Generic;
using Events;
using Goods.Manager;
using OutGame.Upgrades.Domain;
using OutGame.Upgrades.Repository;
using UnityEngine;

namespace OutGame.Upgrades.Manager
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private UpgradeTableSO _table;

        private GoldManager _goldManager;
        private IUpgradeRepository _repository;

        private Dictionary<EUpgradeType, Upgrade> _upgrades;

        public void Initialize(GoldManager goldManager)
        {
            _goldManager = goldManager;
            
            _repository = new LocalUpgradeRepository();
            _upgrades = new Dictionary<EUpgradeType, Upgrade>();

            var upgradeTypes = new List<EUpgradeType>();
            foreach (var spec in _table.AllSpecs)
            {
                if (spec != null)
                {
                    upgradeTypes.Add(spec.Type);
                }
            }

            // 불러오기
            var savedLevels = _repository.LoadAll(upgradeTypes);

            // Upgrade 객체 생성
            foreach (var spec in _table.AllSpecs)
            {
                if (spec == null) continue;
               
                savedLevels.TryGetValue(spec.Type, out int level);
                _upgrades[spec.Type] = new Upgrade(spec, level);
            }

        }

        public Upgrade GetUpgradeData(EUpgradeType type)
        {
            if (_upgrades != null && _upgrades.TryGetValue(type, out Upgrade upgrade))
            {
                return upgrade;
            }

            return null;
        }

        public bool TryPurchase(EUpgradeType type)
        {
            var upgrade = GetUpgradeData(type);
            if (upgrade == null || upgrade.IsMaxLevel) return false;

            long cost = upgrade.NextLevelCost;
            if (cost <= 0 || !_goldManager.HasEnough(cost))
            {
                Debug.LogWarning($"[UpgradeManager] 업그레이드 불가 - Type: {type}, " + $"비용: {cost}, 현재 레벨: {upgrade.Level}");
                return false;
            }

            if (!_goldManager.SpendGold(cost))
            {
                return false;
            }

            upgrade.AddLevel();
            int newLevel = upgrade.Level;

            _repository.SaveLevel(type, newLevel);

            Debug.Log($"[UpgradeManager] 업그레이드 성공 - {upgrade.DisplayName}({type}) " + $"Lv.{newLevel}, Value: {upgrade.Effect}");

            GameEvents.RaiseUpgradePurchased(type, newLevel);

            return true;
        }

        public bool CanUpgrade(EUpgradeType type)
        {
            var upgrade = GetUpgradeData(type);
            if (upgrade == null || upgrade.IsMaxLevel) return false;

            long cost = upgrade.NextLevelCost;
            return cost > 0 && _goldManager.HasEnough(cost);
        }
    }
}
