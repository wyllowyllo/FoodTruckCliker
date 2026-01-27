namespace FoodTruckClicker.Currency
{
    /// <summary>
    /// 재화 조회 인터페이스
    /// </summary>
    public interface ICurrencyProvider
    {
        /// <summary>
        /// 현재 보유 골드
        /// </summary>
        int CurrentGold { get; }

        /// <summary>
        /// 특정 금액을 보유하고 있는지 확인
        /// </summary>
        bool HasEnough(int amount);
    }
}
