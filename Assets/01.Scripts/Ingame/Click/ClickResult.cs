using Menu;

namespace Click
{
    /// <summary>
    /// 클릭 수익 계산 결과
    /// </summary>
    public struct ClickResult
    {
        public float Revenue;
        public bool IsCritical;
        public int MenuCount;
        public MenuData SelectedMenu;

        public ClickResult(float revenue, bool isCritical, int menuCount, MenuData selectedMenu)
        {
            Revenue = revenue;
            IsCritical = isCritical;
            MenuCount = menuCount;
            SelectedMenu = selectedMenu;
        }
    }

}
