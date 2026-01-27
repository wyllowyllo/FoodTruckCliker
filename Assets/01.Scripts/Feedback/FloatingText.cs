using DG.Tweening;
using TMPro;
using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 플로팅 텍스트 - 클릭 시 골드 수익 표시
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("애니메이션 설정")]
        [SerializeField]
        private float _moveDistance = 100f;

        [SerializeField]
        private float _duration = 0.8f;

        [SerializeField]
        private Ease _moveEase = Ease.OutCubic;

        [SerializeField]
        private Ease _fadeEase = Ease.InQuad;

        [Header("크리티컬 설정")]
        [SerializeField]
        private Color _normalColor = Color.white;

        [SerializeField]
        private Color _criticalColor = Color.yellow;

        [SerializeField]
        private float _normalScale = 1f;

        [SerializeField]
        private float _criticalScale = 1.5f;

        private RectTransform _rectTransform;
        private Sequence _animationSequence;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// 플로팅 텍스트 재생
        /// </summary>
        public void Play(float revenue, bool isCritical)
        {
            // 텍스트 설정
            int goldAmount = Mathf.FloorToInt(revenue);
            string prefix = isCritical ? "CRITICAL!\n" : "";
            _text.text = $"{prefix}+{goldAmount}G";

            // 색상/크기 설정
            _text.color = isCritical ? _criticalColor : _normalColor;
            float targetScale = isCritical ? _criticalScale : _normalScale;
            transform.localScale = Vector3.one * targetScale;

            // 초기화
            _canvasGroup.alpha = 1f;
            Vector3 startPosition = _rectTransform.anchoredPosition;

            // 기존 애니메이션 정리
            _animationSequence?.Kill();

            // 애니메이션 시퀀스
            _animationSequence = DOTween.Sequence();

            // 위로 이동
            _animationSequence.Join(
                _rectTransform.DOAnchorPosY(startPosition.y + _moveDistance, _duration)
                    .SetEase(_moveEase)
            );

            // 페이드아웃 (후반부에)
            _animationSequence.Join(
                _canvasGroup.DOFade(0f, _duration * 0.5f)
                    .SetEase(_fadeEase)
                    .SetDelay(_duration * 0.5f)
            );

            // 완료 시 비활성화
            _animationSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                _rectTransform.anchoredPosition = startPosition;
            });
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }
    }
}
