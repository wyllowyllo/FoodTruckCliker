using System;
using System.Collections.Generic;

namespace Upgrade.Repository
{
    public class FirebaseUpgradeRepository : IUpgradeRepository
    {
        private readonly string _userId;

        public FirebaseUpgradeRepository(string userId)
        {
            _userId = userId;
        }

        public int LoadLevel(string upgradeId)
        {
            throw new NotImplementedException();
        }

        public void SaveLevel(string upgradeId, int level)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> LoadAll(IEnumerable<string> upgradeIds)
        {
            throw new NotImplementedException();
        }
    }
}
