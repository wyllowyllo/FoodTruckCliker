using System;
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
        public int MaxLevel => _spec.ScalingMode == EScalingMode.Formula ? int.MaxValue : _spec.ArrayMaxLevel;
        public int Level => _level;

        public float Effect => GetEffect(_level);
        public long NextLevelCost => GetCost(_level + 1);
        public bool IsMaxLevel => _level >= MaxLevel;

        internal void AddLevel()
        {
            _level++;
        }

        private long GetCost(int level)
        {
            if (level < 1)
            {
                return 0;
            }

            if (_spec.ScalingMode == EScalingMode.Formula)
            {
                return (long)(_spec.BaseCost * Math.Pow(_spec.CostMultiplier, level - 1));
            }

            if (level > _spec.ArrayMaxLevel)
            {
                return 0;
            }

            int index = level - 1;
            if (_spec.CostsPerLevel == null || index >= _spec.CostsPerLevel.Length)
            {
                return 0;
            }

            return _spec.CostsPerLevel[index];
        }

        private float GetEffect(int level)
        {
            if (level < 1)
            {
                return GetDefaultEffect();
            }

            if (_spec.ScalingMode == EScalingMode.Formula)
            {
                return _spec.BaseEffectValue + _spec.ValueIncrement * (level - 1);
            }

            if (level > _spec.ArrayMaxLevel)
            {
                return GetDefaultEffect();
            }

            int index = level - 1;
            if (_spec.ValuesPerLevel == null || index >= _spec.ValuesPerLevel.Length)
            {
                return GetDefaultEffect();
            }

            return _spec.ValuesPerLevel[index];
        }

        private float GetDefaultEffect()
        {
            switch (_spec.Type)
            {
                case EUpgradeType.ClickRevenue:
                case EUpgradeType.CookingSpeed:
                case EUpgradeType.FoodTruck:
                    return 1f;

                default:
                    return 0f;
            }
        }
    }
}
