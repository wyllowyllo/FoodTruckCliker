using System;
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

        private CurrencyManager _currencyManager;
        private IUpgradeRepository _repository;
        private UpgradeSaveData _saveData;

        private Dictionary<EUpgradeType, Upgrade> _upgrades;

        public void Initialize(CurrencyManager currencyManager)
        {
            _currencyManager = currencyManager;

            _repository = new LocalUpgradeRepository();
            _saveData = _repository.Load();
            _upgrades = new Dictionary<EUpgradeType, Upgrade>();

            foreach (var spec in _table.AllSpecs)
            {
                if (spec == null) continue;

                int typeIndex = (int)spec.Type;
                int level = (typeIndex < _saveData.Levels.Length) ? _saveData.Levels[typeIndex] : 0;
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
            if (cost <= 0 || !_currencyManager.HasEnough(cost))
            {
                Debug.LogWarning($"[UpgradeManager] 업그레이드 불가 - Type: {type}, " +
                    $"비용: {cost}, 현재 레벨: {upgrade.Level}");
                return false;
            }

            if (!_currencyManager.SpendGold(cost))
            {
                return false;
            }

            upgrade.AddLevel();
            int newLevel = upgrade.Level;

            _saveData.Levels[(int)type] = newLevel;
            _saveData.LastSaveTime = DateTime.Now.ToString("o");
            _repository.Save(_saveData);

            Debug.Log($"[UpgradeManager] 업그레이드 성공 - {upgrade.DisplayName}({type}) " +
                $"Lv.{newLevel}, Value: {upgrade.Effect}");

            GameEvents.RaiseUpgradePurchased(type, newLevel);

            return true;
        }

        public bool CanUpgrade(EUpgradeType type)
        {
            var upgrade = GetUpgradeData(type);
            if (upgrade == null || upgrade.IsMaxLevel) return false;

            long cost = upgrade.NextLevelCost;
            return cost > 0 && _currencyManager.HasEnough(cost);
        }
    }
}
