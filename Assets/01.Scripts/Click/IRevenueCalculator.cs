namespace FoodTruckClicker.Click
{
    /// <summary>
    /// 클릭 수익 계산 결과
    /// </summary>
    public struct ClickResult
    {
        public float Revenue;
        public bool IsCritical;
        public int MenuCount;

        public ClickResult(float revenue, bool isCritical, int menuCount = 1)
        {
            Revenue = revenue;
            IsCritical = isCritical;
            MenuCount = menuCount;
        }
    }

    /// <summary>
    /// 수익 계산 인터페이스
    /// </summary>
    public interface IRevenueCalculator
    {
        /// <summary>
        /// 클릭 수익 계산
        /// </summary>
        ClickResult Calculate();
    }
}
