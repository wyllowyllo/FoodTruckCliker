namespace OutGame.Upgrades.Repository
{
    public interface IUpgradeRepository
    {
        UpgradeSaveData Load();
        void Save(UpgradeSaveData data);
    }
}
