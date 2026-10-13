using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Слушатель шума. Пока без монстра: пишет в Console, когда «услышал» бег рядом.
    /// Подписка в OnEnable, отписка в OnDisable — обязательно (иначе утечка/ошибки).
    /// </summary>
    public sealed class NoiseListener : MonoBehaviour
    {
        [SerializeField] private float _hearRange = 18f;
        [SerializeField] private float _minLoudness = 0.5f;

        private void OnEnable()
        {
            NoiseBus.NoiseEmitted += OnNoise;
        }

        private void OnDisable()
        {
            NoiseBus.NoiseEmitted -= OnNoise;
        }

        private void OnNoise(Vector3 position, float loudness)
        {
            if (loudness < _minLoudness)
                return;

            float distance = Vector3.Distance(transform.position, position);
            if (distance > _hearRange)
                return;

            Debug.Log(
                $"[NoiseListener] {name} услышал шум {loudness:0.00} (~{distance:0.0} м)",
                this);
        }
    }
}
