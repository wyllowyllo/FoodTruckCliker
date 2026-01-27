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
        private FoodPopEffect _foodPopPrefab;

        [SerializeField]
        private Transform _poolParent;

        [SerializeField]
        private int _poolSize = 8;

        [Header("음식 스프라이트")]
        [SerializeField]
        private Sprite[] _foodSprites;

        [SerializeField]
        private int _currentFoodIndex;

        private FoodPopEffect[] _pool;
        private int _currentPoolIndex;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (_foodPopPrefab == null || _poolParent == null)
            {
                Debug.LogWarning("[FoodPopController] 프리팹 또는 부모가 설정되지 않음");
                return;
            }

            _pool = new FoodPopEffect[_poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                FoodPopEffect instance = Instantiate(_foodPopPrefab, _poolParent);
                instance.gameObject.SetActive(false);
                _pool[i] = instance;
            }
        }

        /// <summary>
        /// 음식 팝 재생
        /// </summary>
        public void Pop()
        {
            if (_pool == null || _pool.Length == 0)
            {
                return;
            }

            FoodPopEffect food = _pool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _poolSize;

            Sprite foodSprite = GetCurrentFoodSprite();

            food.gameObject.SetActive(true);
            food.Play(foodSprite);
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
