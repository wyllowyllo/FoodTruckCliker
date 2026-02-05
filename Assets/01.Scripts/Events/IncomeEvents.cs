using System;

namespace Events
{
    /// <summary>
    /// 수익 관련 이벤트
    /// </summary>
    public static class IncomeEvents
    {
        /// <summary>
        /// 수익이 발생했을 때 (수익, 크리티컬 여부, 메뉴 개수, 자동 여부)
        /// </summary>
        public static event Action<float, bool, int, bool> OnRevenueEarned;

        public static void RaiseRevenueEarned(float revenue, bool isCritical, int menuCount, bool isAuto)
        {
            OnRevenueEarned?.Invoke(revenue, isCritical, menuCount, isAuto);
        }

        /// <summary>
        /// 자동 수익이 변경되었을 때 (초당 수익)
        /// </summary>
        public static event Action<float> OnAutoIncomeChanged;

        public static void RaiseAutoIncomeChanged(float incomePerSecond)
        {
            OnAutoIncomeChanged?.Invoke(incomePerSecond);
        }

        public static void Clear()
        {
            OnRevenueEarned = null;
            OnAutoIncomeChanged = null;
        }
    }
}
