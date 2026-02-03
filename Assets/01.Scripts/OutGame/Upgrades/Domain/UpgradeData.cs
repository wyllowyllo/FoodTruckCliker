using UnityEngine;

namespace Upgrade.Domain
{
    /// <summary>
    /// 업그레이드 데이터 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "FoodTruck/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private EUpgradeType _type;
        [SerializeField] private string _upgradeId;
        [SerializeField] private string _displayName;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("스케일링 모드")]
        [SerializeField] private EScalingMode _scalingMode = EScalingMode.Formula;

        [Header("배열 기반 레벨 데이터 (Array 모드)")]
        [SerializeField] private int _maxLevel = 3;

        [SerializeField,Tooltip("레벨별 비용")] private int[] _costsPerLevel;
        

        [SerializeField,Tooltip("레벨별 효과 값")] private float[] _valuesPerLevel;
        

        [Header("공식 기반 데이터 (Formula 모드)")]
        [SerializeField,Tooltip("기본 비용")] private long _baseCost = 10;
      

        [SerializeField]  private float _baseValue = 1f;
        [SerializeField]  private float _costMultiplier = 1.15f;
        [SerializeField,Tooltip("레벨당 효과 증가량")] private float _valueIncrement = 0.1f;
        

        [Header("수정자 설정")]
        [SerializeField] private EModifierType _eModifierType;

       

        // Properties
        public string UpgradeId => _upgradeId;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public EScalingMode ScalingMode => _scalingMode;
        public EModifierType EModifierType => _eModifierType;
        public EUpgradeType Type => _type;

        public int MaxLevel
        {
            get
            {
                if (_scalingMode == EScalingMode.Formula)
                {
                    return int.MaxValue;
                }

                return _maxLevel;
            }
        }

        /// <summary>
        /// 특정 레벨의 비용 반환 (1-based)
        /// </summary>
        public long GetCost(int level)
        {
            if (level < 1)
            {
                return 0;
            }

            if (_scalingMode == EScalingMode.Formula)
            {
                return (long)(_baseCost * System.Math.Pow(_costMultiplier, level - 1));
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

        /// <summary>
        /// 특정 레벨의 효과 값 반환 (1-based)
        /// </summary>
        public float GetValue(int level)
        {
            if (level < 1)
            {
                return GetDefaultValue();
            }

            if (_scalingMode == EScalingMode.Formula)
            {
                return _baseValue + _valueIncrement * (level - 1);
            }

            if (level > _maxLevel)
            {
                return GetDefaultValue();
            }

            int index = level - 1;
            if (_valuesPerLevel == null || index >= _valuesPerLevel.Length)
            {
                return GetDefaultValue();
            }

            return _valuesPerLevel[index];
        }

        /// <summary>
        /// 업그레이드 미적용 시 기본값
        /// </summary>
        public float GetDefaultValue()
        {
            // 승산 타입은 1, 가산 타입은 0이 기본값
            return _eModifierType == EModifierType.Multiplicative ? 1f : 0f;
        }

        private void OnValidate()
        {
            if (_scalingMode != EScalingMode.Array)
            {
                return;
            }

            // 배열 크기 자동 조정
            if (_costsPerLevel == null || _costsPerLevel.Length != _maxLevel)
            {
                System.Array.Resize(ref _costsPerLevel, _maxLevel);
            }

            if (_valuesPerLevel == null || _valuesPerLevel.Length != _maxLevel)
            {
                System.Array.Resize(ref _valuesPerLevel, _maxLevel);
            }
        }
    }
}
