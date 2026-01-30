using FoodTruckClicker.Events;
using UnityEngine;

namespace FoodTruckClicker.Currency
{
    /// <summary>
    /// 골드 재화 관리자
    /// </summary>
    public class GoldManager : MonoBehaviour
    {
        [SerializeField]
        private int _startingGold = 0;

        private int _currentGold;

        public int CurrentGold => _currentGold;

        private void Awake()
        {
            _currentGold = _startingGold;
        }

        private void Start()
        {
            GameEvents.RaiseGoldChanged(_currentGold);
        }

        public bool HasEnough(int amount)
        {
            return _currentGold >= amount;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[GoldManager] AddGold 호출됨 - 잘못된 금액: {amount}");
                return;
            }

            _currentGold += amount;
            Debug.Log($"[GoldManager] 골드 추가: +{amount}, 현재 골드: {_currentGold}");
            GameEvents.RaiseGoldChanged(_currentGold);
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (!HasEnough(amount))
            {
                return false;
            }

            _currentGold -= amount;
            GameEvents.RaiseGoldChanged(_currentGold);
            return true;
        }

        /// <summary>
        /// 골드를 특정 값으로 설정 (디버그/치트용)
        /// </summary>
        public void SetGold(int amount)
        {
            _currentGold = Mathf.Max(0, amount);
            GameEvents.RaiseGoldChanged(_currentGold);
        }
    }
}
