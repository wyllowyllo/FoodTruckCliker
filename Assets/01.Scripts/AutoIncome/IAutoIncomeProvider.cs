namespace FoodTruckClicker.AutoIncome
{
    /// <summary>
    /// 자동 수익 조회 인터페이스
    /// </summary>
    public interface IAutoIncomeProvider
    {
        /// <summary>
        /// 초당 자동 수익
        /// </summary>
        float IncomePerSecond { get; }
    }
}
