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
        private Renderer[] _renderers;
        private Material[][] _materials;
        private Coroutine _deactivateCoroutine;

        private static readonly int SurfaceProperty = Shader.PropertyToID("_Surface");
        private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
        private static readonly int SrcBlendProperty = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendProperty = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWriteProperty = Shader.PropertyToID("_ZWrite");
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _renderers = GetComponentsInChildren<Renderer>();
            InitializeMaterials();
        }

        private void InitializeMaterials()
        {
            _materials = new Material[_renderers.Length][];

            for (int i = 0; i < _renderers.Length; i++)
            {
                Material[] originalMaterials = _renderers[i].materials;
                _materials[i] = new Material[originalMaterials.Length];

                for (int j = 0; j < originalMaterials.Length; j++)
                {
                    _materials[i][j] = originalMaterials[j];
                    SetMaterialTransparent(_materials[i][j]);
                }

                _renderers[i].materials = _materials[i];
            }
        }

        private void SetMaterialTransparent(Material material)
        {
            material.SetFloat(SurfaceProperty, 1f);
            material.SetFloat(BlendProperty, 0f);
            material.SetFloat(SrcBlendProperty, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat(DstBlendProperty, (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat(ZWriteProperty, 0f);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        public void Play(Vector3 startPosition)
        {
            transform.position = startPosition;
            transform.rotation = Random.rotation;

            ResetVelocity();
            ApplyPopForce();
            SetAlpha(1f);

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

            yield return FadeOut();
            gameObject.SetActive(false);
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeOutDuration);
                SetAlpha(alpha);
                yield return null;
            }

            SetAlpha(0f);
        }

        private void SetAlpha(float alpha)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                for (int j = 0; j < _materials[i].Length; j++)
                {
                    Color color = _materials[i][j].GetColor(BaseColorProperty);
                    color.a = alpha;
                    _materials[i][j].SetColor(BaseColorProperty, color);
                }
            }
        }
    }
}
