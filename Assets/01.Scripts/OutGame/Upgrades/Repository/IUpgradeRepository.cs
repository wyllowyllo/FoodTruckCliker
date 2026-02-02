using System.Collections.Generic;

namespace Upgrade.Repository
{
    public interface IUpgradeRepository
    {
        int LoadLevel(string upgradeId);
        void SaveLevel(string upgradeId, int level);
        Dictionary<string, int> LoadAll(IEnumerable<string> upgradeIds);
    }
}
