namespace FoodTruckClicker.Upgrade
{
    /// <summary>
    /// 업그레이드가 영향을 미치는 대상 타입
    /// </summary>
    public enum UpgradeTargetType
    {
        ClickRevenue,       // 클릭 수익 배율
        CriticalChance,     // 크리티컬 확률
        CriticalDamage,     // 크리티컬 메뉴 개수
        ChefCount,          // 요리사 수 (자동 클릭 수)
        CookingSpeed,       // 요리 속도 배율
        MenuUnlock          // 메뉴 해금 레벨
    }

    /// <summary>
    /// 업그레이드 수정자 타입
    /// </summary>
    public enum ModifierType
    {
        Additive,       // 가산 (+N)
        Multiplicative  // 승산 (xN)
    }
}
