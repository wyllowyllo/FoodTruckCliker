namespace FoodTruckClicker.Currency
{
    /// <summary>
    /// 재화 수정 인터페이스
    /// </summary>
    public interface ICurrencyModifier
    {
        /// <summary>
        /// 골드 추가
        /// </summary>
        void AddGold(int amount);

        /// <summary>
        /// 골드 차감 (성공 여부 반환)
        /// </summary>
        bool SpendGold(int amount);
    }
}
