using AutoIncome;
using Click;
using Events;
using Goods.Manager;
using Menu;
using OutGame.Upgrades.Manager;
using UI;
using OutGame.UserData.Manager;
using UnityEngine;

namespace Core
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
        [SerializeField] private CurrencyManager _currencyManager;
        [SerializeField] private UpgradeManager _upgradeManager;
        [SerializeField] private AutoIncomeManager _autoIncomeManager;
        [SerializeField] private ClickController _clickController;
        [SerializeField] private MenuManager _menuManager;

        [Header("UI 참조")]
        [SerializeField] private UpgradeButtonUI[] _upgradeButtons;

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

            if (_currencyManager == null)
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
            // 0. GoldManager 초기화
            string userId = AccountManager.Instance.Email;
            if (_currencyManager != null)
            {
                _currencyManager.Initialize(userId);
            }

            // 1. UpgradeManager 초기화
            if (_upgradeManager != null && _currencyManager != null)
            {
                _upgradeManager.Initialize(_currencyManager);
            }
            else
            {
                Debug.LogError("[GameManager] UpgradeManager 또는 GoldManager가 NULL!");
            }

            // 2. MenuManager 초기화 (UpgradeManager에서 FoodTruck 레벨 읽어옴)
            if (_menuManager != null && _upgradeManager != null)
            {
                _menuManager.Initialize(_upgradeManager);
            }
            else
            {
                Debug.LogError("[GameManager] MenuManager가 NULL!");
            }

            // 3. ClickRevenueCalculator 생성 (MenuManager 연동)
            _clickRevenueCalculator = new ClickRevenueCalculator(
                _upgradeManager,
                _menuManager
            );
            

            // 4. ClickController 초기화
            if (_clickController != null)
            {
                _clickController.Initialize(_clickRevenueCalculator, _currencyManager);
               
            }
            else
            {
                Debug.LogError("[GameManager] ClickController가 NULL!");
            }

            // 5. AutoIncomeManager 초기화 (MenuManager 연동)
            if (_autoIncomeManager != null)
            {
                _autoIncomeManager.Initialize(_upgradeManager, _currencyManager, _menuManager);
                
            }

            // 6. UI 초기화
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
