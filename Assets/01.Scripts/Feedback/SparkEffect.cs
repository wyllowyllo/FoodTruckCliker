using UnityEngine;

namespace Feedback
{
    /// <summary>
    /// 철판 스파크 이펙트 - 클릭 시 기름 튀는 효과
    /// </summary>
    public class SparkEffect : MonoBehaviour
    {
        [Header("파티클 시스템")]
        [SerializeField]
        private ParticleSystem _sparkParticle;

        [Header("카메라")]
        [SerializeField]
        private Camera _camera;

        [Header("발사 설정")]
        [SerializeField]
        private int _normalEmitCount = 5;

        [SerializeField]
        private int _criticalEmitCount = 12;

        [SerializeField]
        private float _spawnDepth = 10f;

        [Header("크리티컬 색상")]
        [SerializeField]
        private Color _normalColor = new Color(1f, 0.8f, 0.3f);

        [SerializeField]
        private Color _criticalColor = new Color(1f, 0.95f, 0.5f);

        private ParticleSystem.MainModule _mainModule;
        private ParticleSystem.EmitParams _emitParams;
        private bool _initialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_sparkParticle == null)
            {
                Debug.LogWarning("[SparkEffect] ParticleSystem이 설정되지 않음");
                return;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _mainModule = _sparkParticle.main;
            _emitParams = new ParticleSystem.EmitParams();
            _initialized = true;
        }

        /// <summary>
        /// 스파크 이펙트 재생 (터치 위치에서)
        /// </summary>
        public void Play(bool isCritical)
        {
            if (!_initialized || _camera == null)
            {
                return;
            }

            // 터치/클릭 위치를 월드 좌표로 변환
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = _spawnDepth;
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);

            // 파티클 발사 위치 설정
            _emitParams.position = worldPos;

            // 크리티컬 여부에 따라 색상 변경
            _mainModule.startColor = isCritical ? _criticalColor : _normalColor;

            // 파티클 발사
            int emitCount = isCritical ? _criticalEmitCount : _normalEmitCount;
            _sparkParticle.Emit(_emitParams, emitCount);
        }
    }
}
