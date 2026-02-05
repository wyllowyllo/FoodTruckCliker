namespace OutGame.Upgrades.Domain
{
    /// <summary>
    /// 업그레이드가 영향을 미치는 대상 타입
    /// </summary>
    public enum EUpgradeType
    {
        ClickRevenue,       // 클릭 수익 배율
        CriticalChance,     // 크리티컬 확률
        CriticalProfit,     // 크리티컬 시 판매 메뉴 개수
        ChefCount,          // 요리사 수 (자동 클릭 수)
        CookingSpeed,       // 요리 속도 배율
        FoodTruck,
        
        Count
    }

    /// <summary>
    /// 업그레이드 스케일링 모드
    /// </summary>
    public enum EScalingMode
    {
        Array,      // 배열 기반 (고정 레벨)
        Formula     // 공식 기반 (무한 레벨)
    }
}
