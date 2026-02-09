using System;
using Cysharp.Threading.Tasks;
using OutGame.Goods.Repository;

namespace OutGame.Upgrades.Repository
{
    public class HybridCurrencyRepository : ICurrencyRepository
    {
        private ICurrencyRepository _localRepository;
        private ICurrencyRepository _firebaseRepository;

        public HybridCurrencyRepository()
        {
            _localRepository = new LocalCurrencyRepository();
            _firebaseRepository = new FirebaseCurrencyRepository();
        }

        public async UniTask<CurrencySaveData> Load()
        {
            CurrencySaveData localData = await _localRepository.Load();
            CurrencySaveData serverData = await _firebaseRepository.Load();

            DateTime localTime = DateTime.Parse(localData.LastSaveTime);
            DateTime serverTime = DateTime.Parse(serverData.LastSaveTime);

            return localTime > serverTime ? localData : serverData;
        }

        public async UniTask Save(CurrencySaveData saveData)
        {
            _localRepository.Save(saveData);
            await _firebaseRepository.Save(saveData);
        }
    }
}