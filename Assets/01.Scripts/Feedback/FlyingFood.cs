using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 날아가는 음식 개별 오브젝트
    /// </summary>
    public class FlyingFood : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField]
        private Image _foodImage;

        [SerializeField]
        private RectTransform _rectTransform;

        [Header("애니메이션 설정")]
        [SerializeField]
        private float _duration = 0.4f;

        [SerializeField]
        private float _arcHeight = 150f;

        [SerializeField]
        private Ease _moveEase = Ease.OutQuad;

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

            // 시작 위치 설정
            _rectTransform.anchoredPosition = startPos;
            transform.localScale = Vector3.zero;

            // 기존 애니메이션 정리
            _flySequence?.Kill();
            _flySequence = DOTween.Sequence();

            // 팝업 효과 (스케일)
            _flySequence.Append(
                transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack)
            );
            _flySequence.Append(
                transform.DOScale(1f, 0.05f)
            );

            // 포물선 이동
            Vector2 midPoint = (startPos + endPos) / 2f + Vector2.up * _arcHeight;

            _flySequence.Append(
                _rectTransform.DOAnchorPos(midPoint, _duration * 0.5f).SetEase(Ease.OutQuad)
            );
            _flySequence.Append(
                _rectTransform.DOAnchorPos(endPos, _duration * 0.5f).SetEase(Ease.InQuad)
            );

            // 도착 시 축소
            _flySequence.Join(
                transform.DOScale(0.5f, _duration * 0.3f).SetEase(Ease.InQuad).SetDelay(_duration * 0.7f)
            );

            // 완료 시 비활성화
            _flySequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        private void OnDestroy()
        {
            _flySequence?.Kill();
        }
    }
}
