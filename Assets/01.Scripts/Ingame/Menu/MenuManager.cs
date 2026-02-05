using System.Collections.Generic;
using System.Linq;
using Events;
using OutGame.Upgrades.Domain;
using OutGame.Upgrades.Manager;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// 메뉴 관리자
    /// 해금된 메뉴 중 랜덤 선택 및 가격 배율 적용
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("메뉴 목록")]
        [SerializeField] private List<MenuData> _allMenus = new List<MenuData>();

        private UpgradeManager _upgradeManager;
        private int _currentUnlockLevel = 0;
        private float _priceMultiplier = 1f;
        private List<MenuData> _unlockedMenus = new List<MenuData>();

        public float PriceMultiplier => _priceMultiplier;
        public int CurrentUnlockLevel => _currentUnlockLevel;

        public float AveragePrice
        {
            get
            {
                if (_unlockedMenus == null || _unlockedMenus.Count == 0)
                {
                    return 10f * _priceMultiplier;
                }
                return (float)_unlockedMenus.Average(m => m.BasePrice) * _priceMultiplier;
            }
        }

        public void Initialize(UpgradeManager upgradeManager)
        {
            _upgradeManager = upgradeManager;
            SortMenusByUnlockLevel();

            var foodTruck = upgradeManager.GetUpgradeData(EUpgradeType.FoodTruck);
            if (foodTruck != null && foodTruck.Level > 0)
            {
                _currentUnlockLevel = foodTruck.Level;
                _priceMultiplier = Mathf.Max(1f, foodTruck.Effect);
            }

            UpdateUnlockedMenus();
        }

        private void OnEnable()
        {
            UpgradeEvents.OnUpgradePurchased += HandleUpgradePurchased;
        }

        private void OnDisable()
        {
            UpgradeEvents.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        private void HandleUpgradePurchased(EUpgradeType type, int newLevel)
        {
            if (type != EUpgradeType.FoodTruck || _upgradeManager == null)
            {
                return;
            }

            var foodTruck = _upgradeManager.GetUpgradeData(EUpgradeType.FoodTruck);
            if (foodTruck == null)
            {
                return;
            }

            SetUnlockLevel(foodTruck.Level);
            SetPriceMultiplier(foodTruck.Effect);
            Debug.Log($"[MenuManager] 트럭 업그레이드 - 해금 레벨: {foodTruck.Level}, 가격 배율: {foodTruck.Effect}");
        }

        private void SetUnlockLevel(int level)
        {
            if (level == _currentUnlockLevel)
            {
                return;
            }

            _currentUnlockLevel = level;
            UpdateUnlockedMenus();
        }

        private void SetPriceMultiplier(float multiplier)
        {
            _priceMultiplier = Mathf.Max(1f, multiplier);
        }

        /// <summary>
        /// 해금된 메뉴 중 랜덤 선택
        /// </summary>
        public MenuData GetRandomMenu()
        {
            if (_unlockedMenus == null || _unlockedMenus.Count == 0)
            {
                return _allMenus.FirstOrDefault();
            }

            int randomIndex = Random.Range(0, _unlockedMenus.Count);
            return _unlockedMenus[randomIndex];
        }

        /// <summary>
        /// 메뉴의 최종 가격 계산 (기본가격 × 가격배율)
        /// </summary>
        public int GetFinalPrice(MenuData menu)
        {
            if (menu == null)
            {
                return Mathf.RoundToInt(10 * _priceMultiplier);
            }

            return Mathf.RoundToInt(menu.BasePrice * _priceMultiplier);
        }

        /// <summary>
        /// 모든 메뉴 목록 반환
        /// </summary>
        public IReadOnlyList<MenuData> GetAllMenus()
        {
            return _allMenus;
        }

        /// <summary>
        /// 해금된 메뉴 목록 반환
        /// </summary>
        public IReadOnlyList<MenuData> GetUnlockedMenus()
        {
            return _unlockedMenus;
        }

        private void SortMenusByUnlockLevel()
        {
            _allMenus = _allMenus.OrderBy(m => m.UnlockLevel).ToList();
        }

        private void UpdateUnlockedMenus()
        {
            _unlockedMenus = _allMenus
                .Where(m => m.UnlockLevel <= _currentUnlockLevel)
                .ToList();

            // 최소 1개는 있어야 함
            if (_unlockedMenus.Count == 0 && _allMenus.Count > 0)
            {
                _unlockedMenus.Add(_allMenus[0]);
            }
        }
    }
}
