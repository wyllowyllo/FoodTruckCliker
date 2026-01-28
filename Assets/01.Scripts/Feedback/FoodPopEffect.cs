using System.Collections;
using UnityEngine;

namespace FoodTruckClicker.Feedback
{
    [RequireComponent(typeof(Rigidbody))]
    public class FoodPopEffect : MonoBehaviour
    {
        [Header("팝 설정")]
        [SerializeField]
        private float _upwardForce = 8f;

        [SerializeField]
        private float _horizontalForce = 3f;

        [SerializeField]
        private float _torqueForce = 10f;

        [Header("생명주기")]
        [SerializeField]
        private float _lifeTime = 2f;

        [SerializeField]
        private float _fadeOutDuration = 0.5f;

        private Rigidbody _rigidbody;
        private Vector3 _originalScale;
        private Coroutine _deactivateCoroutine;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _originalScale = transform.localScale;
        }

        public void Play(Vector3 startPosition)
        {
            transform.position = startPosition;
            transform.rotation = Random.rotation;
            transform.localScale = _originalScale;

            ResetVelocity();
            ApplyPopForce();

            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
            }
            _deactivateCoroutine = StartCoroutine(DeactivateAfterDelay());
        }

        private void ResetVelocity()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void ApplyPopForce()
        {
            float randomX = Random.Range(-_horizontalForce, _horizontalForce);
            float randomZ = Random.Range(-_horizontalForce, _horizontalForce);
            Vector3 force = new Vector3(randomX, _upwardForce, randomZ);

            _rigidbody.AddForce(force, ForceMode.VelocityChange);

            Vector3 torque = Random.insideUnitSphere * _torqueForce;
            _rigidbody.AddTorque(torque, ForceMode.VelocityChange);
        }

        private IEnumerator DeactivateAfterDelay()
        {
            float waitTime = _lifeTime - _fadeOutDuration;
            if (waitTime > 0f)
            {
                yield return new WaitForSeconds(waitTime);
            }

            yield return ScaleDown();
            gameObject.SetActive(false);
        }

        private IEnumerator ScaleDown()
        {
            float elapsed = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeOutDuration;
                float scale = Mathf.Lerp(1f, 0f, t * t);
                transform.localScale = _originalScale * scale;
                yield return null;
            }

            transform.localScale = Vector3.zero;
        }
    }
}
