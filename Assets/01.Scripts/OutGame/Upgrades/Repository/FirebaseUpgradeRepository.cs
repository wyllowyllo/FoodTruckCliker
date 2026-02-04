using System;
using System.Collections.Generic;
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    public class FirebaseUpgradeRepository : IUpgradeRepository
    {
        private readonly string _userId;

        public FirebaseUpgradeRepository(string userId)
        {
            _userId = userId;
        }

        public int LoadLevel(EUpgradeType type)
        {
            throw new NotImplementedException();
        }

        public void SaveLevel(EUpgradeType type, int level)
        {
            throw new NotImplementedException();
        }

        public Dictionary<EUpgradeType, int> LoadAll(IEnumerable<EUpgradeType> types)
        {
            throw new NotImplementedException();
        }
    }
}
