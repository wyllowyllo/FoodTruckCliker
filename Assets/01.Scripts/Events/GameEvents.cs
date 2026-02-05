namespace Events
{
    /// <summary>
    /// 게임 전역 이벤트 관리
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// 모든 이벤트 구독 해제 (씬 전환 시 사용)
        /// </summary>
        public static void ClearAllSubscriptions()
        {
            CurrencyEvents.Clear();
            UpgradeEvents.Clear();
            IncomeEvents.Clear();
        }
    }
}
