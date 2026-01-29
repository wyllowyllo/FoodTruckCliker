namespace FoodTruckClicker.Menu
{
    /// <summary>
    /// 메뉴 정보 조회 인터페이스
    /// </summary>
    public interface IMenuProvider
    {
        /// <summary>
        /// 현재 해금된 최고 메뉴 반환
        /// </summary>
        MenuData CurrentMenu { get; }

        /// <summary>
        /// 현재 메뉴 가격 반환
        /// </summary>
        int CurrentMenuPrice { get; }

        /// <summary>
        /// 현재 메뉴 해금 레벨 반환
        /// </summary>
        int CurrentUnlockLevel { get; }
    }
}
