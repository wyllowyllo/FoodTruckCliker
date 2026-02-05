using System;

namespace Events
{
    /// <summary>
    /// 재화 관련 이벤트
    /// </summary>
    public static class CurrencyEvents
    {
        /// <summary>
        /// 골드가 변경되었을 때 발생
        /// </summary>
        public static event Action<long> OnGoldChanged;

        public static void RaiseGoldChanged(long newGold)
        {
            OnGoldChanged?.Invoke(newGold);
        }

        public static void Clear()
        {
            OnGoldChanged = null;
        }
    }
}
