using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Мерцание потолочной лампы Level 0.
    /// Lifecycle: Awake → OnEnable → Start → Update (каждый кадр).
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class FlickeringLight : MonoBehaviour
    {
        [SerializeField] private float _baseIntensity = 1.2f;
        [SerializeField] private float _flickerAmplitude = 0.35f;
        [SerializeField] private float _flickerSpeed = 8f;
        [SerializeField] private float _noiseScale = 0.7f;

        private Light _light;
        private float _seed;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _seed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            float t = Time.time * _flickerSpeed + _seed;
            float noise = Mathf.PerlinNoise(t, _seed);
            float wobble = (noise - 0.5f) * 2f * _flickerAmplitude * _noiseScale;
            float spike = Random.value > 0.97f ? -_flickerAmplitude * 1.5f : 0f;
            _light.intensity = Mathf.Max(0f, _baseIntensity + wobble + spike);
        }
    }
}
