using System;
using OutGame.Upgrades.Domain;

namespace Events
{
    /// <summary>
    /// 게임 전역 이벤트 정의
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// 골드가 변경되었을 때 발생
        /// </summary>
        public static event Action<long> OnGoldChanged;

        public static void RaiseGoldChanged(long newGold)
        {
            OnGoldChanged?.Invoke(newGold);
        }

        /// <summary>
        /// 수익이 발생했을 때 (수익, 크리티컬 여부, 메뉴 개수, 자동 여부)
        /// </summary>
        public static event Action<float, bool, int, bool> OnRevenueEarned;

        public static void RaiseRevenueEarned(float revenue, bool isCritical, int menuCount, bool isAuto)
        {
            OnRevenueEarned?.Invoke(revenue, isCritical, menuCount, isAuto);
        }

        /// <summary>
        /// 업그레이드가 구매되었을 때 (업그레이드 타입, 새 레벨)
        /// </summary>
        public static event Action<EUpgradeType, int> OnUpgradePurchased;

        public static void RaiseUpgradePurchased(EUpgradeType type, int newLevel)
        {
            OnUpgradePurchased?.Invoke(type, newLevel);
        }

        /// <summary>
        /// 자동 수익이 변경되었을 때 (초당 수익)
        /// </summary>
        public static event Action<float> OnAutoIncomeChanged;

        public static void RaiseAutoIncomeChanged(float incomePerSecond)
        {
            OnAutoIncomeChanged?.Invoke(incomePerSecond);
        }

        /// <summary>
        /// 모든 이벤트 구독 해제 (씬 전환 시 사용)
        /// </summary>
        public static void ClearAllSubscriptions()
        {
            OnGoldChanged = null;
            OnRevenueEarned = null;
            OnUpgradePurchased = null;
            OnAutoIncomeChanged = null;
        }
    }
}
