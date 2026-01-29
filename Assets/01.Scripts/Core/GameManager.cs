using FoodTruckClicker.AutoIncome;
using FoodTruckClicker.Click;
using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using FoodTruckClicker.Menu;
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

        [SerializeField]
        private MenuManager _menuManager;

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

            if (_menuManager == null)
            {
                Debug.LogError("MenuManager is not assigned!");
            }
        }

        private void InitializeSystems()
        {
            Debug.Log("[GameManager] 시스템 초기화 시작");

            // 1. MenuManager 초기화
            if (_menuManager != null)
            {
                _menuManager.Initialize();
                Debug.Log("[GameManager] MenuManager 초기화 완료");
            }
            else
            {
                Debug.LogError("[GameManager] MenuManager가 NULL!");
            }

            // 2. UpgradeManager 초기화 (MenuManager 연동)
            if (_upgradeManager != null && _goldManager != null)
            {
                _upgradeManager.Initialize(
                    _goldManager,
                    _goldManager,
                    _menuManager,
                    OnFoodTruckUpgraded
                );
                Debug.Log("[GameManager] UpgradeManager 초기화 완료");
            }
            else
            {
                Debug.LogError("[GameManager] UpgradeManager 또는 GoldManager가 NULL!");
            }

            // 3. ClickRevenueCalculator 생성 (MenuManager 연동)
            _clickRevenueCalculator = new ClickRevenueCalculator(
                _upgradeManager,
                _menuManager
            );
            Debug.Log("[GameManager] ClickRevenueCalculator 생성 완료");

            // 4. ClickController 초기화
            if (_clickController != null)
            {
                _clickController.Initialize(_clickRevenueCalculator, _goldManager);
                Debug.Log("[GameManager] ClickController 초기화 완료");
            }
            else
            {
                Debug.LogError("[GameManager] ClickController가 NULL!");
            }

            // 5. AutoIncomeManager 초기화 (MenuManager 연동)
            if (_autoIncomeManager != null)
            {
                _autoIncomeManager.Initialize(_upgradeManager, _goldManager, _menuManager);
                Debug.Log("[GameManager] AutoIncomeManager 초기화 완료");
            }

            // 6. UI 초기화
            InitializeUI();
            Debug.Log("[GameManager] 시스템 초기화 완료");
        }

        private void OnFoodTruckUpgraded(int unlockLevel, float priceMultiplier)
        {
            if (_menuManager != null)
            {
                _menuManager.SetUnlockLevel(unlockLevel);
                _menuManager.SetPriceMultiplier(priceMultiplier);
                Debug.Log($"[GameManager] 트럭 업그레이드 - 해금 레벨: {unlockLevel}, 가격 배율: {priceMultiplier}");
            }
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
