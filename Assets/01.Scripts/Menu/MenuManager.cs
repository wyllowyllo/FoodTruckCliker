using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoodTruckClicker.Menu
{
    /// <summary>
    /// 메뉴 관리자
    /// 해금된 메뉴 관리 및 현재 최고 메뉴 조회
    /// </summary>
    public class MenuManager : MonoBehaviour, IMenuProvider
    {
        [Header("메뉴 목록")]
        [SerializeField] private List<MenuData> _allMenus = new List<MenuData>();

        private int _currentUnlockLevel = 0;
        private MenuData _currentMenu;

        public MenuData CurrentMenu => _currentMenu;
        public int CurrentMenuPrice => _currentMenu != null ? _currentMenu.Price : 10;
        public int CurrentUnlockLevel => _currentUnlockLevel;

        public void Initialize()
        {
            SortMenusByUnlockLevel();
            UpdateCurrentMenu();
        }

        /// <summary>
        /// 메뉴 해금 레벨 설정 (트럭 업그레이드 시 호출)
        /// </summary>
        public void SetUnlockLevel(int level)
        {
            if (level == _currentUnlockLevel)
            {
                return;
            }

            _currentUnlockLevel = level;
            UpdateCurrentMenu();
        }

        /// <summary>
        /// 특정 해금 레벨에 해당하는 메뉴 반환
        /// </summary>
        public MenuData GetMenuByUnlockLevel(int unlockLevel)
        {
            return _allMenus.FirstOrDefault(m => m.UnlockLevel == unlockLevel);
        }

        /// <summary>
        /// 모든 메뉴 목록 반환
        /// </summary>
        public IReadOnlyList<MenuData> GetAllMenus()
        {
            return _allMenus;
        }

        /// <summary>
        /// 현재 해금된 메뉴 목록 반환
        /// </summary>
        public IEnumerable<MenuData> GetUnlockedMenus()
        {
            return _allMenus.Where(m => m.UnlockLevel <= _currentUnlockLevel);
        }

        private void SortMenusByUnlockLevel()
        {
            _allMenus = _allMenus.OrderBy(m => m.UnlockLevel).ToList();
        }

        private void UpdateCurrentMenu()
        {
            _currentMenu = _allMenus
                .Where(m => m.UnlockLevel <= _currentUnlockLevel)
                .OrderByDescending(m => m.UnlockLevel)
                .FirstOrDefault();

            if (_currentMenu == null && _allMenus.Count > 0)
            {
                _currentMenu = _allMenus[0];
            }
        }
    }
}
