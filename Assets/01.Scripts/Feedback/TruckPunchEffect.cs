using DG.Tweening;
using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    /// <summary>
    /// 트럭 Punch Scale 효과
    /// </summary>
    public class TruckPunchEffect : MonoBehaviour
    {
        [Header("Punch 설정")]
        [SerializeField]
        private Vector3 _punchScale = new Vector3(0.1f, 0.1f, 0.1f);

        [SerializeField]
        private float _duration = 0.15f;

        [SerializeField]
        private int _vibrato = 10;

        [SerializeField]
        private float _elasticity = 1f;

        private Tweener _punchTween;
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        /// <summary>
        /// Punch 효과 재생
        /// </summary>
        public void Punch()
        {
            // 진행 중인 트윈이 있으면 종료하고 원래 크기로 복원
            if (_punchTween != null && _punchTween.IsActive())
            {
                _punchTween.Kill();
                transform.localScale = _originalScale;
            }

            _punchTween = transform.DOPunchScale(_punchScale, _duration, _vibrato, _elasticity);
        }

        private void OnDestroy()
        {
            _punchTween?.Kill();
        }
    }
}
