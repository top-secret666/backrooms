using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Вешай на Player. PlayerController вызывает Notify каждый кадр движения.
    /// Бег = громкий шум, шаг тише, присед почти тихо.
    /// </summary>
    public sealed class PlayerNoiseEmitter : MonoBehaviour
    {
        [SerializeField] private float _walkLoudness = 0.35f;
        [SerializeField] private float _sprintLoudness = 0.9f;
        [SerializeField] private float _crouchLoudness = 0.1f;
        [SerializeField] private float _emitInterval = 0.2f;

        private float _nextEmitTime;

        public void Notify(bool isMoving, bool isSprinting, bool isCrouching)
        {
            if (!isMoving || Time.time < _nextEmitTime)
                return;

            // Тихий шаг/присед тоже шлём в шину (для будущего стелса),
            // но NoiseProbe с minLoudness 0.5 услышит в основном спринт (~0.9).
            float loudness = isCrouching
                ? _crouchLoudness
                : (isSprinting ? _sprintLoudness : _walkLoudness);

            NoiseBus.Emit(transform.position, loudness);
            _nextEmitTime = Time.time + _emitInterval;
        }
    }
}
