using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 음식 팝 이펙트 - 클릭 시 음식이 팝업되며 터짐
    /// </summary>
    public class FoodPopEffect : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField]
        private Image _foodImage;

        [SerializeField]
        private RectTransform _rectTransform;

        [Header("팝업 설정")]
        [SerializeField]
        private float _popScale = 1.3f;

        [SerializeField]
        private float _popDuration = 0.15f;

        [Header("터짐 설정")]
        [SerializeField]
        private float _burstScale = 1.8f;

        [SerializeField]
        private float _burstDuration = 0.2f;

        [Header("위치 랜덤")]
        [SerializeField]
        private float _randomOffsetX = 60f;

        [SerializeField]
        private float _randomOffsetY = 40f;

        private CanvasGroup _canvasGroup;
        private Sequence _sequence;
        private Vector2 _basePosition;

        private void Awake()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _basePosition = _rectTransform.anchoredPosition;
        }

        /// <summary>
        /// 음식 팝 재생
        /// </summary>
        public void Play(Sprite foodSprite)
        {
            // 스프라이트 설정
            if (_foodImage != null && foodSprite != null)
            {
                _foodImage.sprite = foodSprite;
            }

            // 랜덤 위치
            Vector2 randomOffset = new Vector2(
                Random.Range(-_randomOffsetX, _randomOffsetX),
                Random.Range(-_randomOffsetY, _randomOffsetY)
            );
            _rectTransform.anchoredPosition = _basePosition + randomOffset;

            // 초기화
            transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 1f;

            // 기존 시퀀스 정리
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            // 팝업 (커지면서 등장)
            _sequence.Append(
                transform.DOScale(_popScale, _popDuration).SetEase(Ease.OutBack)
            );

            // 잠깐 유지
            _sequence.AppendInterval(0.05f);

            // 터짐 (더 커지면서 페이드아웃)
            _sequence.Append(
                transform.DOScale(_burstScale, _burstDuration).SetEase(Ease.OutQuad)
            );
            _sequence.Join(
                _canvasGroup.DOFade(0f, _burstDuration).SetEase(Ease.InQuad)
            );

            // 완료 시 비활성화
            _sequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                _rectTransform.anchoredPosition = _basePosition;
            });
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}
