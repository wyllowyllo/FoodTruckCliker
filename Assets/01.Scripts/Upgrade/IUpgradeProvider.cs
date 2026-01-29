namespace FoodTruckClicker.Upgrade
{
    /// <summary>
    /// 업그레이드 정보 조회 인터페이스
    /// </summary>
    public interface IUpgradeProvider
    {
        /// <summary>
        /// 특정 타입의 업그레이드 효과 값 반환 (float)
        /// </summary>
        float GetValue(UpgradeTargetType targetType);

        /// <summary>
        /// 특정 타입의 업그레이드 효과 값 반환 (정수)
        /// 요리사 수, 크리티컬 개수, 메뉴 레벨 등에 사용
        /// </summary>
        int GetIntValue(UpgradeTargetType targetType);

        /// <summary>
        /// 특정 업그레이드의 현재 레벨 반환
        /// </summary>
        int GetLevel(string upgradeId);

        /// <summary>
        /// 특정 업그레이드의 다음 레벨 비용 반환 (최대 레벨이면 0)
        /// </summary>
        int GetNextLevelCost(string upgradeId);

        /// <summary>
        /// 업그레이드 가능 여부 확인
        /// </summary>
        bool CanUpgrade(string upgradeId);
    }
}
