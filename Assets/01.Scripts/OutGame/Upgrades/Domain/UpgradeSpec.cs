using System;
using UnityEngine;

namespace OutGame.Upgrades.Domain
{
    [Serializable]
    public class UpgradeSpec
    {
        [Header("기본 정보")]
        [SerializeField] private EUpgradeType _type;
        [SerializeField] private string _displayName;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("스케일링 모드")]
        [SerializeField] private EScalingMode _scalingMode = EScalingMode.Formula;

        [Header("배열 기반 레벨 데이터 (Array 모드)")]
        [SerializeField] private int _maxLevel = 3;
        [SerializeField, Tooltip("레벨별 비용")] private int[] _costsPerLevel;
        [SerializeField, Tooltip("레벨별 효과 값")] private float[] _valuesPerLevel;

        [Header("공식 기반 데이터 (Formula 모드)")]
        [SerializeField, Tooltip("기본 비용")] private long _baseCost = 10;
        [SerializeField] private float _baseValue = 1f;
        [SerializeField] private float _costMultiplier = 1.15f;
        [SerializeField, Tooltip("레벨당 효과 증가량")] private float _valueIncrement = 0.1f;

        public EUpgradeType Type => _type;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public EScalingMode ScalingMode => _scalingMode;
        public int MaxLevel
        {
            get
            {
                if (_scalingMode == EScalingMode.Formula) return int.MaxValue;
                return _maxLevel;
            }
        }

        public long GetCost(int level)
        {
            if (level < 1)
            {
                return 0;
            }

            if (_scalingMode == EScalingMode.Formula)
            {
                return (long)(_baseCost * Math.Pow(_costMultiplier, level - 1));
            }

            if (level > _maxLevel)
            {
                return 0;
            }

            int index = level - 1;
            if (_costsPerLevel == null || index >= _costsPerLevel.Length)
            {
                return 0;
            }

            return _costsPerLevel[index];
        }

        public float GetEffect(int level)
        {
            if (level < 1)
            {
                return GetDefaultEffect();
            }

            if (_scalingMode == EScalingMode.Formula)
            {
                return _baseValue + _valueIncrement * (level - 1);
            }

            if (level > _maxLevel)
            {
                return GetDefaultEffect();
            }

            int index = level - 1;
            if (_valuesPerLevel == null || index >= _valuesPerLevel.Length)
            {
                return GetDefaultEffect();
            }

            return _valuesPerLevel[index];
        }

        public float GetDefaultEffect()
        {
            switch (_type)
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
