namespace FoodTruckClicker.Upgrade
{
    /// <summary>
    /// 업그레이드가 영향을 미치는 대상 타입
    /// </summary>
    public enum UpgradeTargetType
    {
        ClickBase,          // 메뉴 추가: 기본 클릭 수익 가산
        ClickMultiplier,    // 고급 재료: 클릭 수익 배율
        Critical,           // 황금 손: 크리티컬 확률
        AutoBase,           // 요리사: 초당 자동 수익
        GlobalMultiplier,   // 마케팅: 전체 수익 배율
        TruckBonus          // 트럭 업그레이드: 추가 보너스 배율
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
