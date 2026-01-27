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
            ProcessClick();
        }

        private void ProcessClick()
        {
            if (_revenueCalculator == null || _currencyModifier == null)
            {
                Debug.LogWarning("ClickController not initialized");
                return;
            }

            ClickResult result = _revenueCalculator.Calculate();
            int goldToAdd = Mathf.FloorToInt(result.Revenue);

            if (goldToAdd > 0)
            {
                _currencyModifier.AddGold(goldToAdd);
            }

            GameEvents.RaiseClicked(result.Revenue, result.IsCritical);
        }
    }
}
