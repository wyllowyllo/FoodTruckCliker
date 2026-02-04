using System.Collections.Generic;
using OutGame.Upgrades.Domain;

namespace OutGame.Upgrades.Repository
{
    public interface IUpgradeRepository
    {
        int LoadLevel(EUpgradeType type);
        void SaveLevel(EUpgradeType type, int level);
        Dictionary<EUpgradeType, int> LoadAll(IEnumerable<EUpgradeType> types);
    }
}
