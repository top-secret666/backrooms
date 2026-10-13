using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Мерцание люминесцентной лампы: Light.intensity + emission панели.
    /// Emission через MaterialPropertyBlock — у каждой лампы своё, shared material не ломаем.
    /// Учи: Perlin flicker, rare dip, PropertyBlock vs sharedMaterial.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class FlickeringLight : MonoBehaviour
    {
        [Header("Light intensity")]
        [SerializeField] private float _baseIntensity = 2.4f;
        [SerializeField] private float _minIntensity = 2.15f;
        [SerializeField] private float _flickerSpeed = 0.25f;
        [SerializeField, Range(0f, 1f)] private float _rareDipChance = 0.00035f;
        [SerializeField] private float _rareDipDuration = 0.08f;
        [SerializeField] private float _rareDipIntensity = 1.55f;

        [Header("Panel emission (прямоугольник)")]
        [SerializeField] private Renderer _panelRenderer;
        [SerializeField] private Color _baseEmission = new Color(0.42f, 0.4f, 0.28f, 1f);
        [Tooltip("0 = панель не мигает, 1 = сильно следует за светом.")]
        [SerializeField, Range(0f, 1f)] private float _panelFlickerAmount = 0.24f;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private Light _light;
        private float _seed;
        private float _dipUntil;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _seed = Random.Range(0f, 100f);
            _block = new MaterialPropertyBlock();

            if (_panelRenderer == null)
                _panelRenderer = FindPanelRenderer();

            CacheBaseEmissionFromMaterial();
        }

        private void Update()
        {
            Apply(ComputeIntensity());
        }

        /// <summary>Считаем яркость кадра (спокойный шум + редкий провал).</summary>
        private float ComputeIntensity()
        {
            if (Time.time < _dipUntil)
                return _rareDipIntensity;

            if (Random.value < _rareDipChance)
            {
                _dipUntil = Time.time + _rareDipDuration;
                return _rareDipIntensity;
            }

            float n = Mathf.PerlinNoise(Time.time * _flickerSpeed + _seed, _seed);
            return Mathf.Lerp(_minIntensity, _baseIntensity, n);
        }

        /// <summary>Пишем в Light и (слабо) в emission панели.</summary>
        private void Apply(float intensity)
        {
            if (_light != null)
                _light.intensity = intensity;

            if (_panelRenderer == null)
                return;

            float t = _baseIntensity > 0.001f
                ? Mathf.Clamp01(intensity / _baseIntensity)
                : 0f;

            float emissionMul = Mathf.Lerp(1f - _panelFlickerAmount, 1f, t);
            _panelRenderer.GetPropertyBlock(_block);
            _block.SetColor(EmissionColorId, _baseEmission * emissionMul);
            _panelRenderer.SetPropertyBlock(_block);
        }

        private void CacheBaseEmissionFromMaterial()
        {
            if (_panelRenderer == null)
                return;

            var mat = _panelRenderer.sharedMaterial;
            if (mat != null && mat.HasProperty(EmissionColorId))
                _baseEmission = mat.GetColor(EmissionColorId);
        }

        private Renderer FindPanelRenderer()
        {
            Transform root = transform.parent != null ? transform.parent : transform;
            foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            {
                if (r.GetComponent<Light>() != null)
                    continue;
                if (r.gameObject == gameObject)
                    continue;
                return r;
            }

            return root.GetComponent<Renderer>();
        }
    }
}
