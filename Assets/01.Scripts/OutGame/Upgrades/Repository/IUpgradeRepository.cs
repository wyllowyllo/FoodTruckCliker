using Cysharp.Threading.Tasks;

namespace OutGame.Upgrades.Repository
{
    public interface IUpgradeRepository
    {
        UniTask<UpgradeSaveData> Load();
        UniTaskVoid Save(UpgradeSaveData data);
    }
}
