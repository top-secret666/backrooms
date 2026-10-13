using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Баланс игрока в ассете (ScriptableObject), а не в коде на сцене.
    /// Создай: ПКМ в Configs → Create → Backrooms → Player Config.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PlayerConfig",
        menuName = "Backrooms/Player Config")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Header("Move")]
        [SerializeField] private float _walkSpeed = 3.2f;
        [SerializeField] private float _sprintSpeed = 5.5f;
        [SerializeField] private float _crouchSpeedMultiplier = 0.55f;
        [SerializeField] private float _gravity = -20f;

        [Header("Look")]
        [SerializeField] private float _mouseSensitivity = 0.12f;
        [SerializeField] private float _minPitch = -80f;
        [SerializeField] private float _maxPitch = 80f;

        [Header("Crouch / height")]
        [SerializeField] private float _standingHeight = 1.8f;
        [SerializeField] private float _crouchHeight = 1.1f;
        [SerializeField] private float _standingCameraY = 1.65f;
        [SerializeField] private float _crouchCameraY = 0.9f;
        [SerializeField] private float _crouchLerp = 12f;

        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float CrouchSpeedMultiplier => _crouchSpeedMultiplier;
        public float Gravity => _gravity;
        public float MouseSensitivity => _mouseSensitivity;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
        public float StandingHeight => _standingHeight;
        public float CrouchHeight => _crouchHeight;
        public float StandingCameraY => _standingCameraY;
        public float CrouchCameraY => _crouchCameraY;
        public float CrouchLerp => _crouchLerp;
    }
}
