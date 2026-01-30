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
        [SerializeField]
        private string _upgradeId;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        [TextArea]
        private string _description;

        [SerializeField]
        private Sprite _icon;

        [Header("레벨 데이터")]
        [SerializeField]
        private int _maxLevel = 3;

        [SerializeField]
        [Tooltip("레벨별 비용 (배열 크기 = MaxLevel)")]
        private int[] _costsPerLevel;

        [SerializeField]
        [Tooltip("레벨별 효과 값 (배열 크기 = MaxLevel)")]
        private float[] _valuesPerLevel;

        [Header("수정자 설정")]
        [SerializeField]
        private EModifierType _eModifierType;

        [SerializeField]
        private EUpgradeTargetType _targetType;

        // Properties
        public string UpgradeId => _upgradeId;
        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int MaxLevel => _maxLevel;
        public EModifierType EModifierType => _eModifierType;
        public EUpgradeTargetType TargetType => _targetType;

        /// <summary>
        /// 특정 레벨의 비용 반환 (1-based)
        /// </summary>
        public int GetCost(int level)
        {
            if (level < 1 || level > _maxLevel)
            {
                return 0;
            }

            int index = level - 1;
            if (index >= _costsPerLevel.Length)
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
            if (level < 1 || level > _maxLevel)
            {
                return GetDefaultValue();
            }

            int index = level - 1;
            if (index >= _valuesPerLevel.Length)
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
