
using Events;
using Goods.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Click
{
    /// <summary>
    /// 클릭 입력 처리 및 골드 획득 컨트롤러
    /// </summary>
    public class ClickController : MonoBehaviour, IPointerClickHandler
    {
        private ClickRevenueCalculator _revenueCalculator;
        private GoldManager _goldManager;

        public void Initialize(ClickRevenueCalculator revenueCalculator, GoldManager goldManager)
        {
            _revenueCalculator = revenueCalculator;
            _goldManager = goldManager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[ClickController] OnPointerClick 호출됨");
            ProcessClick();
        }

        private void ProcessClick()
        {
            if (_revenueCalculator == null || _goldManager == null)
            {
                Debug.LogWarning("[ClickController] 초기화 안됨 - RevenueCalculator: " +
                    (_revenueCalculator == null ? "NULL" : "OK") +
                    ", GoldManager: " +
                    (_goldManager == null ? "NULL" : "OK"));
                return;
            }

            ClickResult result = _revenueCalculator.Calculate();
            int goldToAdd = Mathf.FloorToInt(result.Revenue);

            Debug.Log($"[ClickController] 클릭! 수익: {result.Revenue:F2}, 크리티컬: {result.IsCritical}, 메뉴개수: {result.MenuCount}, 골드 추가: {goldToAdd}");

            if (goldToAdd > 0)
            {
                _goldManager.AddGold(goldToAdd);
            }

            GameEvents.RaiseRevenueEarned(result.Revenue, result.IsCritical, result.MenuCount, false);
        }
    }
}
