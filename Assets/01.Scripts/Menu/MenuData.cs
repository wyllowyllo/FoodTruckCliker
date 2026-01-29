using UnityEngine;

namespace FoodTruckClicker.Menu
{
    /// <summary>
    /// 메뉴 데이터 ScriptableObject
    /// 메뉴 이름, 가격, 아이콘, 해금 레벨 정보를 담는다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMenu", menuName = "FoodTruckClicker/Menu Data")]
    public class MenuData : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private string _menuId;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        [Header("게임 데이터")]
        [SerializeField] private int _basePrice = 10;
        [SerializeField] private int _unlockLevel = 0;

        public string MenuId => _menuId;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public int BasePrice => _basePrice;
        public int UnlockLevel => _unlockLevel;
    }
}
