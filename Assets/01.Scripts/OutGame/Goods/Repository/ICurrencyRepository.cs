using Cysharp.Threading.Tasks;

namespace Goods.Repository
{
    
    public interface ICurrencyRepository
    {
        UniTask<CurrencySaveData> Load();
        UniTaskVoid Save(CurrencySaveData amount);
    }
}
