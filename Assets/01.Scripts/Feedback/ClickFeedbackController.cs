using FoodTruckClicker.Events;
using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 클릭 피드백 컨트롤러 - 모든 클릭 피드백 통합 관리
    /// </summary>
    public class ClickFeedbackController : MonoBehaviour
    {
        [Header("플로팅 텍스트")]
        [SerializeField]
        private FloatingText _floatingTextPrefab;

        [SerializeField]
        private Transform _floatingTextParent;

        [SerializeField]
        private int _poolSize = 10;

        [Header("트럭 효과")]
        [SerializeField]
        private TruckPunchEffect _truckPunchEffect;

        [Header("스파크 효과")]
        [SerializeField]
        private SparkEffect _sparkEffect;

        [Header("음식 팝 이펙트")]
        [SerializeField]
        private FoodPopController _foodPopController;

        private FloatingText[] _floatingTextPool;
        private int _currentPoolIndex;

        private void Awake()
        {
            InitializePool();
        }

        private void OnEnable()
        {
            GameEvents.OnClicked += HandleClicked;
        }

        private void OnDisable()
        {
            GameEvents.OnClicked -= HandleClicked;
        }

        private void InitializePool()
        {
            if (_floatingTextPrefab == null || _floatingTextParent == null)
            {
                Debug.LogWarning("[ClickFeedbackController] FloatingText 프리팹 또는 부모가 설정되지 않음");
                return;
            }

            _floatingTextPool = new FloatingText[_poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                FloatingText instance = Instantiate(_floatingTextPrefab, _floatingTextParent);
                instance.gameObject.SetActive(false);
                _floatingTextPool[i] = instance;
            }
        }

        private void HandleClicked(float revenue, bool isCritical, int menuCount)
        {
            SpawnFloatingText(revenue, isCritical, menuCount);
            TriggerTruckPunch();
            TriggerSpark(isCritical);
            SpawnFoodPop(menuCount);
            TriggerHaptic(isCritical);
        }

        private void SpawnFoodPop(int menuCount)
        {
            if (_foodPopController != null)
            {
                _foodPopController.SpawnFoodPop(menuCount);
            }
        }

        private void TriggerSpark(bool isCritical)
        {
            if (_sparkEffect != null)
            {
                _sparkEffect.Play(isCritical);
            }
        }

        private void TriggerHaptic(bool isCritical)
        {
            if (isCritical)
            {
                HapticFeedback.HeavyImpact();
            }
            else
            {
                HapticFeedback.LightImpact();
            }
        }

        private void SpawnFloatingText(float revenue, bool isCritical, int menuCount)
        {
            if (_floatingTextPool == null || _floatingTextPool.Length == 0)
            {
                return;
            }

            FloatingText text = _floatingTextPool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _poolSize;

            text.gameObject.SetActive(true);
            text.Play(revenue, isCritical, menuCount);
        }

        private void TriggerTruckPunch()
        {
            if (_truckPunchEffect != null)
            {
                _truckPunchEffect.Punch();
            }
        }
    }
}
