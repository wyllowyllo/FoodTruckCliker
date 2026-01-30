namespace Goods.Repository
{
    
    public interface ICurrencyRepository
    {
        int Load();
        void Save(int amount);
    }
}
