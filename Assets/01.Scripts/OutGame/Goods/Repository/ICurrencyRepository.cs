using Cysharp.Threading.Tasks;

namespace OutGame.Goods.Repository
{
    
    public interface ICurrencyRepository
    {
        UniTask<CurrencySaveData> Load();
        UniTask Save(CurrencySaveData amount);
    }
}
