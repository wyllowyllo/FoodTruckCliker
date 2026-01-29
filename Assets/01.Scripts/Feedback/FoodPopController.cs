using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 음식 팝 이펙트 관리 (오브젝트 풀링)
    /// </summary>
    public class FoodPopController : MonoBehaviour
    {
        [Header("프리팹")]
        [SerializeField]
        private FoodPopEffect[] _foodPrefabs;

        [SerializeField]
        private Transform _poolParent;

        [SerializeField]
        private int _poolSizePerPrefab = 5;

        [Header("위치 설정")]
        [SerializeField]
        private Transform _spawnPoint;

        [Header("VFX")]
        [SerializeField]
        private ParticleSystem _spawnVfx;

        [Header("스폰 설정 (일반)")]
        [SerializeField]
        private int _minSpawnCount = 3;

        [SerializeField]
        private int _maxSpawnCount = 5;

        [SerializeField]
        private float _spawnRandomOffsetX = 0.2f;

        [SerializeField]
        private float _spawnRandomOffsetY = 0.1f;

        private FoodPopEffect[] _pool;
        private int _currentPoolIndex;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (_foodPrefabs == null || _foodPrefabs.Length == 0 || _poolParent == null)
            {
                Debug.LogWarning("[FoodPopController] 프리팹 또는 부모가 설정되지 않음");
                return;
            }

            int totalPoolSize = _foodPrefabs.Length * _poolSizePerPrefab;
            _pool = new FoodPopEffect[totalPoolSize];

            int index = 0;
            for (int i = 0; i < _foodPrefabs.Length; i++)
            {
                for (int j = 0; j < _poolSizePerPrefab; j++)
                {
                    FoodPopEffect instance = Instantiate(_foodPrefabs[i], _poolParent);
                    instance.gameObject.SetActive(false);
                    _pool[index] = instance;
                    index++;
                }
            }

            ShufflePool();
        }

        private void ShufflePool()
        {
            for (int i = _pool.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_pool[i], _pool[j]) = (_pool[j], _pool[i]);
            }
        }

        /// <summary>
        /// 음식 팝 이펙트 스폰 (일반 - 기본 개수)
        /// </summary>
        public void SpawnFoodPop()
        {
            SpawnFoodPop(1);
        }

        /// <summary>
        /// 음식 팝 이펙트 스폰 (menuCount만큼 그룹 생성)
        /// </summary>
        public void SpawnFoodPop(int menuCount)
        {
            if (_pool == null || _pool.Length == 0)
            {
                return;
            }

            if (_spawnPoint == null)
            {
                return;
            }

            PlaySpawnVfx();

            // menuCount 만큼 그룹 스폰 (크리티컬 시 더 많은 음식)
            int baseCount = Random.Range(_minSpawnCount, _maxSpawnCount + 1);
            int totalSpawnCount = baseCount * Mathf.Max(1, menuCount);

            for (int i = 0; i < totalSpawnCount; i++)
            {
                SpawnSingleFood();
            }
        }

        private void PlaySpawnVfx()
        {
            if (_spawnVfx == null)
            {
                return;
            }

            _spawnVfx.transform.position = _spawnPoint.position;
            _spawnVfx.Play();
        }

        private void SpawnSingleFood()
        {
            FoodPopEffect food = _pool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _pool.Length;

            Vector3 startPos = _spawnPoint.position;
            startPos.x += Random.Range(-_spawnRandomOffsetX, _spawnRandomOffsetX);
            startPos.y += Random.Range(-_spawnRandomOffsetY, _spawnRandomOffsetY);

            food.gameObject.SetActive(true);
            food.Play(startPos);
        }
    }
}
