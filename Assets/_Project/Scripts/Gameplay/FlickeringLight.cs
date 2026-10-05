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
        [SerializeField] private float _baseIntensity = 3f;
        [SerializeField] private float _minIntensity = 0.05f;
        [SerializeField] private float _flickerSpeed = 12f;
        [SerializeField, Range(0f, 1f)] private float _blackoutChance = 0.04f;
        [SerializeField] private float _blackoutDuration = 0.08f;

        private Light _light;
        private float _seed;
        private float _blackoutUntil;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _seed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (Time.time < _blackoutUntil)
            {
                _light.intensity = _minIntensity;
                return;
            }

            if (Random.value < _blackoutChance)
            {
                _blackoutUntil = Time.time + _blackoutDuration;
                _light.intensity = _minIntensity;
                return;
            }

            // Два слоя шума: медленный гул + быстрые вспышки
            float slow = Mathf.PerlinNoise(Time.time * (_flickerSpeed * 0.35f) + _seed, _seed);
            float fast = Mathf.PerlinNoise(_seed, Time.time * _flickerSpeed * 1.7f);
            float mix = slow * 0.55f + fast * 0.45f;
            _light.intensity = Mathf.Lerp(_minIntensity, _baseIntensity, mix);
        }
    }
}
