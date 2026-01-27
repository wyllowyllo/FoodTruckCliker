using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 날아가는 음식 이펙트 관리 (오브젝트 풀링)
    /// </summary>
    public class FlyingFoodController : MonoBehaviour
    {
        [Header("프리팹")]
        [SerializeField]
        private FlyingFood _flyingFoodPrefab;

        [SerializeField]
        private Transform _poolParent;

        [SerializeField]
        private int _poolSize = 10;

        [Header("위치 설정")]
        [SerializeField]
        private RectTransform _spawnPoint;

        [SerializeField]
        private RectTransform _targetPoint;

        [Header("음식 스프라이트")]
        [SerializeField]
        private Sprite[] _foodSprites;

        [SerializeField]
        private int _currentFoodIndex;

        [Header("스폰 위치 랜덤")]
        [SerializeField]
        private float _spawnRandomOffsetX = 50f;

        [SerializeField]
        private float _targetRandomOffsetX = 30f;

        private FlyingFood[] _pool;
        private int _currentPoolIndex;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (_flyingFoodPrefab == null || _poolParent == null)
            {
                Debug.LogWarning("[FlyingFoodController] 프리팹 또는 부모가 설정되지 않음");
                return;
            }

            _pool = new FlyingFood[_poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                FlyingFood instance = Instantiate(_flyingFoodPrefab, _poolParent);
                instance.gameObject.SetActive(false);
                _pool[i] = instance;
            }
        }

        /// <summary>
        /// 음식 날리기
        /// </summary>
        public void SpawnFood()
        {
            if (_pool == null || _pool.Length == 0)
            {
                return;
            }

            if (_spawnPoint == null || _targetPoint == null)
            {
                return;
            }

            FlyingFood food = _pool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _poolSize;

            // 시작/도착 위치 계산 (랜덤 오프셋 추가)
            Vector2 startPos = _spawnPoint.anchoredPosition;
            startPos.x += Random.Range(-_spawnRandomOffsetX, _spawnRandomOffsetX);

            Vector2 endPos = _targetPoint.anchoredPosition;
            endPos.x += Random.Range(-_targetRandomOffsetX, _targetRandomOffsetX);

            // 현재 음식 스프라이트
            Sprite foodSprite = GetCurrentFoodSprite();

            food.gameObject.SetActive(true);
            food.Fly(startPos, endPos, foodSprite);
        }

        private Sprite GetCurrentFoodSprite()
        {
            if (_foodSprites == null || _foodSprites.Length == 0)
            {
                return null;
            }

            int index = Mathf.Clamp(_currentFoodIndex, 0, _foodSprites.Length - 1);
            return _foodSprites[index];
        }

        /// <summary>
        /// 현재 음식 인덱스 설정 (메뉴 업그레이드 시 호출)
        /// </summary>
        public void SetFoodIndex(int index)
        {
            _currentFoodIndex = index;
        }
    }
}
