using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Мягкое мерцание потолочной лампы Level 0 (как люминесцентная трубка в офисе).
    /// Lifecycle: Awake → OnEnable → Start → Update (каждый кадр).
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class FlickeringLight : MonoBehaviour
    {
        [SerializeField] private float _baseIntensity = 2.2f;
        [SerializeField] private float _minIntensity = 1.6f;
        [SerializeField] private float _flickerSpeed = 1.2f;
        [SerializeField, Range(0f, 1f)] private float _rareDipChance = 0.003f;
        [SerializeField] private float _rareDipDuration = 0.12f;
        [SerializeField] private float _rareDipIntensity = 0.7f;

        private Light _light;
        private float _seed;
        private float _dipUntil;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _seed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (Time.time < _dipUntil)
            {
                _light.intensity = _rareDipIntensity;
                return;
            }

            // Редкий короткий "провал", как у старой лампы — не каждый кадр
            if (Random.value < _rareDipChance)
            {
                _dipUntil = Time.time + _rareDipDuration;
                _light.intensity = _rareDipIntensity;
                return;
            }

            // Медленный шум: яркость гуляет в узком диапазоне
            float n = Mathf.PerlinNoise(Time.time * _flickerSpeed + _seed, _seed);
            _light.intensity = Mathf.Lerp(_minIntensity, _baseIntensity, n);
        }
    }
}
