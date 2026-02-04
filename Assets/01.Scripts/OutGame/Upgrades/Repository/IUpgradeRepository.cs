using System.Collections.Generic;
using Upgrade.Domain;

namespace Upgrade.Repository
{
    public interface IUpgradeRepository
    {
        int LoadLevel(EUpgradeType type);
        void SaveLevel(EUpgradeType type, int level);
        Dictionary<EUpgradeType, int> LoadAll(IEnumerable<EUpgradeType> types);
    }
}
