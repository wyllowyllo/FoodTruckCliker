using DG.Tweening;
using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 음식 팝 이펙트 - 팝콘처럼 튀어오르고 사라짐 (3D 오브젝트)
    /// </summary>
    public class FoodPopEffect : MonoBehaviour
    {
        [Header("타이밍")]
        [SerializeField]
        private float _popDuration = 0.1f;

        [SerializeField]
        private float _floatDuration = 0.35f;

        [Header("이동")]
        [SerializeField]
        private float _minHeight = 0.5f;

        [SerializeField]
        private float _maxHeight = 1.2f;

        [SerializeField]
        private float _spreadX = 0.4f;

        [SerializeField]
        private float _spreadZ = 0.3f;

        [Header("스케일")]
        [SerializeField]
        private float _popScale = 1.2f;

        [SerializeField]
        private float _endScale = 0.3f;

        [Header("회전")]
        [SerializeField]
        private float _minRotation = 180f;

        [SerializeField]
        private float _maxRotation = 540f;

        private Sequence _popSequence;
        private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;
        private Color _originalColor;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();

            if (_renderer != null)
            {
                _renderer.GetPropertyBlock(_propertyBlock);
                if (_renderer.sharedMaterial.HasProperty(BaseColorProperty))
                {
                    _originalColor = _renderer.sharedMaterial.GetColor(BaseColorProperty);
                }
                else if (_renderer.sharedMaterial.HasProperty(ColorProperty))
                {
                    _originalColor = _renderer.sharedMaterial.GetColor(ColorProperty);
                }
                else
                {
                    _originalColor = Color.white;
                }
            }
        }

        /// <summary>
        /// 팝 이펙트 재생
        /// </summary>
        public void Pop(Vector3 startPos)
        {
            // 초기화
            transform.position = startPos;
            transform.localScale = Vector3.zero;
            transform.localRotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );

            SetAlpha(1f);

            // 랜덤 방향 계산
            float randomX = Random.Range(-_spreadX, _spreadX);
            float randomZ = Random.Range(-_spreadZ, _spreadZ);
            float randomHeight = Random.Range(_minHeight, _maxHeight);
            Vector3 endPos = startPos + new Vector3(randomX, randomHeight, randomZ);

            // 랜덤 회전
            Vector3 randomRotation = new Vector3(
                Random.Range(_minRotation, _maxRotation) * (Random.value > 0.5f ? 1f : -1f),
                Random.Range(_minRotation, _maxRotation) * (Random.value > 0.5f ? 1f : -1f),
                Random.Range(_minRotation, _maxRotation) * (Random.value > 0.5f ? 1f : -1f)
            );

            // 기존 애니메이션 정리
            _popSequence?.Kill();
            _popSequence = DOTween.Sequence();

            float totalDuration = _popDuration + _floatDuration;

            // === 회전 (처음부터 끝까지) ===
            _popSequence.Insert(
                0f,
                transform.DORotate(randomRotation, totalDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
            );

            // === 팝업 (튀어나옴) ===
            _popSequence.Append(
                transform.DOScale(_popScale, _popDuration).SetEase(Ease.OutBack)
            );

            // === 위로 떠오르며 작아짐 ===
            _popSequence.Append(
                transform.DOMove(endPos, _floatDuration).SetEase(Ease.OutQuad)
            );
            _popSequence.Join(
                transform.DOScale(_endScale, _floatDuration).SetEase(Ease.InQuad)
            );

            // 페이드 아웃
            _popSequence.Insert(
                _popDuration + _floatDuration * 0.4f,
                DOTween.To(() => 1f, SetAlpha, 0f, _floatDuration * 0.6f)
            );

            // 완료 시 비활성화
            _popSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        private void SetAlpha(float alpha)
        {
            if (_renderer == null)
            {
                return;
            }

            Color color = _originalColor;
            color.a = alpha;

            _renderer.GetPropertyBlock(_propertyBlock);

            if (_renderer.sharedMaterial.HasProperty(BaseColorProperty))
            {
                _propertyBlock.SetColor(BaseColorProperty, color);
            }
            else if (_renderer.sharedMaterial.HasProperty(ColorProperty))
            {
                _propertyBlock.SetColor(ColorProperty, color);
            }

            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void OnDestroy()
        {
            _popSequence?.Kill();
        }
    }
}
