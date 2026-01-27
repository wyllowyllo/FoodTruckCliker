using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 날아가는 음식 - 트럭에서 손님으로 포물선 이동
    /// </summary>
    public class FlyingFood : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField]
        private Image _foodImage;

        [SerializeField]
        private RectTransform _rectTransform;

        [Header("타이밍")]
        [SerializeField]
        private float _popDuration = 0.1f;

        [SerializeField]
        private float _flyDuration = 0.35f;

        [Header("포물선")]
        [SerializeField]
        private float _arcHeight = 180f;

        [Header("스케일")]
        [SerializeField]
        private float _popScale = 1.3f;

        [SerializeField]
        private float _flyScale = 0.8f;

        [SerializeField]
        private float _endScale = 0.4f;

        [Header("회전")]
        [SerializeField]
        private float _minRotation = 360f;

        [SerializeField]
        private float _maxRotation = 720f;

        private Sequence _flySequence;

        private void Awake()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 음식 날리기
        /// </summary>
        public void Fly(Vector2 startPos, Vector2 endPos, Sprite foodSprite)
        {
            // 스프라이트 설정
            if (_foodImage != null && foodSprite != null)
            {
                _foodImage.sprite = foodSprite;
            }

            // 초기화
            _rectTransform.anchoredPosition = startPos;
            transform.localScale = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // 랜덤 회전량
            float rotation = Random.Range(_minRotation, _maxRotation);
            if (Random.value > 0.5f) rotation *= -1f;

            // 기존 애니메이션 정리
            _flySequence?.Kill();
            _flySequence = DOTween.Sequence();

            // === 팝업 (트럭에서 튀어나옴) ===
            _flySequence.Append(
                transform.DOScale(_popScale, _popDuration).SetEase(Ease.OutBack)
            );

            // === 포물선 비행 ===
            Vector2 midPoint = (startPos + endPos) / 2f + Vector2.up * _arcHeight;

            // 상승 구간
            _flySequence.Append(
                _rectTransform.DOAnchorPos(midPoint, _flyDuration * 0.45f).SetEase(Ease.OutQuad)
            );
            _flySequence.Join(
                transform.DOScale(_flyScale, _flyDuration * 0.45f).SetEase(Ease.OutQuad)
            );

            // 하강 구간
            _flySequence.Append(
                _rectTransform.DOAnchorPos(endPos, _flyDuration * 0.55f).SetEase(Ease.InQuad)
            );
            _flySequence.Join(
                transform.DOScale(_endScale, _flyDuration * 0.55f).SetEase(Ease.InQuad)
            );

            // 회전 (비행 전체 구간)
            _flySequence.Join(
                transform.DORotate(new Vector3(0, 0, rotation), _flyDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
            );

            // 완료 시 비활성화
            _flySequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                transform.localRotation = Quaternion.identity;
            });
        }

        private void OnDestroy()
        {
            _flySequence?.Kill();
        }
    }
}
