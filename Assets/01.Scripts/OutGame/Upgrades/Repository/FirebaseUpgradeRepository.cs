using System;

namespace OutGame.Upgrades.Repository
{
    public class FirebaseUpgradeRepository : IUpgradeRepository
    {
        private readonly string _userId;

        public FirebaseUpgradeRepository(string userId)
        {
            _userId = userId;
        }

        public UpgradeSaveData Load()
        {
            throw new NotImplementedException();
        }

        public void Save(UpgradeSaveData data)
        {
            throw new NotImplementedException();
        }
    }
}
