namespace Goods.Repository
{
    
    public interface ICurrencyRepository
    {
        CurrencySaveData Load();
        void Save(CurrencySaveData amount);
    }
}
