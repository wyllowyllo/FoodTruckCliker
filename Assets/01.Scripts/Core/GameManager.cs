using FoodTruckClicker.AutoIncome;
using FoodTruckClicker.Click;
using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using FoodTruckClicker.UI;
using FoodTruckClicker.Upgrade;
using UnityEngine;

namespace FoodTruckClicker.Core
{
    /// <summary>
    /// 게임 매니저 - 시스템 초기화 및 의존성 연결
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField]
        private GameConfig _gameConfig;

        [Header("시스템 참조")]
        [SerializeField]
        private GoldManager _goldManager;

        [SerializeField]
        private UpgradeManager _upgradeManager;

        [SerializeField]
        private AutoIncomeManager _autoIncomeManager;

        [SerializeField]
        private ClickController _clickController;

        [Header("UI 참조")]
        [SerializeField]
        private UpgradeButtonUI[] _upgradeButtons;

        private ClickRevenueCalculator _clickRevenueCalculator;

        private void Awake()
        {
            ValidateReferences();
            InitializeSystems();
        }

        private void OnDestroy()
        {
            GameEvents.ClearAllSubscriptions();
        }

        private void ValidateReferences()
        {
            if (_gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned!");
            }

            if (_goldManager == null)
            {
                Debug.LogError("GoldManager is not assigned!");
            }

            if (_upgradeManager == null)
            {
                Debug.LogError("UpgradeManager is not assigned!");
            }
        }

        private void InitializeSystems()
        {
            // 1. UpgradeManager 초기화
            if (_upgradeManager != null && _goldManager != null)
            {
                _upgradeManager.Initialize(_goldManager, _goldManager);
            }

            // 2. ClickRevenueCalculator 생성
            float baseRevenue = _gameConfig != null ? _gameConfig.BaseClickRevenue : 1f;
            float criticalMultiplier = _gameConfig != null ? _gameConfig.CriticalMultiplier : 2f;

            _clickRevenueCalculator = new ClickRevenueCalculator(
                _upgradeManager,
                baseRevenue,
                criticalMultiplier
            );

            // 3. ClickController 초기화
            if (_clickController != null)
            {
                _clickController.Initialize(_clickRevenueCalculator, _goldManager);
            }

            // 4. AutoIncomeManager 초기화
            if (_autoIncomeManager != null)
            {
                _autoIncomeManager.Initialize(_upgradeManager, _goldManager);
            }

            // 5. UI 초기화
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (_upgradeButtons == null || _upgradeManager == null)
            {
                return;
            }

            foreach (var button in _upgradeButtons)
            {
                if (button != null)
                {
                    button.Initialize(_upgradeManager);
                }
            }
        }
    }
}
