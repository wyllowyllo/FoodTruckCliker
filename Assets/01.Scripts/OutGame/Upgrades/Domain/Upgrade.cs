using UnityEngine;

namespace OutGame.Upgrades.Domain
{
    public class Upgrade
    {
        private readonly UpgradeSpec _spec;
        private int _level;

        public Upgrade(UpgradeSpec spec, int level)
        {
            _spec = spec;
            _level = level;
        }

        public UpgradeSpec Spec => _spec;
        public EUpgradeType Type => _spec.Type;
        public string DisplayName => _spec.DisplayName;
        public Sprite Icon => _spec.Icon;
        public int MaxLevel => _spec.MaxLevel;

        public int Level => _level;

        public float Effect => _spec.GetEffect(_level);
        public long NextLevelCost => _spec.GetCost(_level + 1);
        public bool IsMaxLevel => _level >= MaxLevel;

        internal void AddLevel()
        {
            _level++;
        }
    }
}
