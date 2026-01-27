using UnityEngine;

namespace FoodTruckClicker.Core
{
    /// <summary>
    /// 게임 전역 설정
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "FoodTruck/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("초기 값")]
        [SerializeField]
        private int _startingGold = 0;

        [SerializeField]
        private float _baseClickRevenue = 1f;

        [Header("크리티컬")]
        [SerializeField]
        private float _criticalMultiplier = 2f;

        [Header("자동 수익")]
        [SerializeField]
        private float _autoIncomeInterval = 1f;

        // Properties
        public int StartingGold => _startingGold;
        public float BaseClickRevenue => _baseClickRevenue;
        public float CriticalMultiplier => _criticalMultiplier;
        public float AutoIncomeInterval => _autoIncomeInterval;
    }
}
