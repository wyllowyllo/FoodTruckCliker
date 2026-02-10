using System;
using Cysharp.Threading.Tasks;
using OutGame.Goods.Repository;

namespace OutGame.Upgrades.Repository
{
    public class HybridCurrencyRepository : ICurrencyRepository
    {
        private ICurrencyRepository _localRepository;
#if !UNITY_WEBGL || UNITY_EDITOR
        private ICurrencyRepository _firebaseRepository;
#endif

        public HybridCurrencyRepository()
        {
            _localRepository = new LocalCurrencyRepository("");
#if !UNITY_WEBGL || UNITY_EDITOR
            _firebaseRepository = new FirebaseCurrencyRepository();
#endif
        }

        public async UniTask<CurrencySaveData> Load()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            CurrencySaveData localData = await _localRepository.Load();
            CurrencySaveData serverData = await _firebaseRepository.Load();

            DateTime localTime = DateTime.Parse(localData.LastSaveTime);
            DateTime serverTime = DateTime.Parse(serverData.LastSaveTime);

            return localTime > serverTime ? localData : serverData;
#else
            return await _localRepository.Load();
#endif
        }

        public async UniTask Save(CurrencySaveData saveData)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _localRepository.Save(saveData);
            await _firebaseRepository.Save(saveData);
#else
            await _localRepository.Save(saveData);
#endif
        }
    }
}
