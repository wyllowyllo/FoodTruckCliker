using System;
using OutGame.Upgrades.Domain;

namespace Events
{
    /// <summary>
    /// 업그레이드 관련 이벤트
    /// </summary>
    public static class UpgradeEvents
    {
        /// <summary>
        /// 업그레이드가 구매되었을 때 (업그레이드 타입, 새 레벨)
        /// </summary>
        public static event Action<EUpgradeType, int> OnUpgradePurchased;

        public static void RaiseUpgradePurchased(EUpgradeType type, int newLevel)
        {
            OnUpgradePurchased?.Invoke(type, newLevel);
        }

        public static void Clear()
        {
            OnUpgradePurchased = null;
        }
    }
}
