using FoodTruckClicker.Currency;
using FoodTruckClicker.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FoodTruckClicker.Click
{
    /// <summary>
    /// 클릭 입력 처리 및 골드 획득 컨트롤러
    /// </summary>
    public class ClickController : MonoBehaviour, IPointerClickHandler
    {
        private IRevenueCalculator _revenueCalculator;
        private ICurrencyModifier _currencyModifier;

        public void Initialize(IRevenueCalculator revenueCalculator, ICurrencyModifier currencyModifier)
        {
            _revenueCalculator = revenueCalculator;
            _currencyModifier = currencyModifier;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[ClickController] OnPointerClick 호출됨");
            ProcessClick();
        }

        private void ProcessClick()
        {
            if (_revenueCalculator == null || _currencyModifier == null)
            {
                Debug.LogWarning("[ClickController] 초기화 안됨 - RevenueCalculator: " +
                    (_revenueCalculator == null ? "NULL" : "OK") +
                    ", CurrencyModifier: " +
                    (_currencyModifier == null ? "NULL" : "OK"));
                return;
            }

            ClickResult result = _revenueCalculator.Calculate();
            int goldToAdd = Mathf.FloorToInt(result.Revenue);

            Debug.Log($"[ClickController] 클릭! 수익: {result.Revenue:F2}, 크리티컬: {result.IsCritical}, 골드 추가: {goldToAdd}");

            if (goldToAdd > 0)
            {
                _currencyModifier.AddGold(goldToAdd);
            }

            GameEvents.RaiseClicked(result.Revenue, result.IsCritical);
        }
    }
}
