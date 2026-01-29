namespace FoodTruckClicker.Menu
{
    /// <summary>
    /// 메뉴 정보 조회 인터페이스
    /// </summary>
    public interface IMenuProvider
    {
        /// <summary>
        /// 랜덤 메뉴 선택 및 반환
        /// </summary>
        MenuData GetRandomMenu();

        /// <summary>
        /// 메뉴의 최종 가격 계산 (기본가격 × 가격배율)
        /// </summary>
        int GetFinalPrice(MenuData menu);

        /// <summary>
        /// 현재 메뉴 가격 배율
        /// </summary>
        float PriceMultiplier { get; }

        /// <summary>
        /// 평균 메뉴 가격 (자동 수익 계산용)
        /// </summary>
        float AveragePrice { get; }

        /// <summary>
        /// 현재 메뉴 해금 레벨
        /// </summary>
        int CurrentUnlockLevel { get; }
    }
}
