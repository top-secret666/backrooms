using System;
using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Шина шума (Observer / event bus): кто угодно Emit, кто подписан — слышит.
    /// Игрок не знает про врагов; слушатели не ищут игрока через Find.
    /// Учи вместе с PlayerNoiseEmitter + NoiseListener (подписка OnEnable/OnDisable).
    /// </summary>
    public static class NoiseBus
    {
        /// <summary>position = откуда шум, loudness = 0..1+ (бег громче шага).</summary>
        public static event Action<Vector3, float> NoiseEmitted;

        public static void Emit(Vector3 worldPosition, float loudness)
        {
            NoiseEmitted?.Invoke(worldPosition, loudness);
        }
    }
}
