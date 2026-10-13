using UnityEngine;
using UnityEngine.InputSystem;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// FPS на Rigidbody + CapsuleCollider.
    /// Учи по порядку: PlayerConfig (SO) → Look → Move/FixedUpdate → crouch → NoiseEmitter.
    /// Фонарик — отдельный компонент BrokenFlashlight (не здесь).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private PlayerNoiseEmitter _noiseEmitter;

        private Rigidbody _body;
        private CapsuleCollider _capsule;
        private float _pitch;
        private float _cameraLocalY;
        private bool _isCrouching;
        private bool _grounded;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();

            var legacyCc = GetComponent<CharacterController>();
            if (legacyCc != null)
                legacyCc.enabled = false;

            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _body.constraints = RigidbodyConstraints.FreezeRotation;
            _body.useGravity = true;

            _capsule.direction = 1;
            _capsule.isTrigger = false;
            ApplyStandingCapsule();

            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_noiseEmitter == null)
                _noiseEmitter = GetComponent<PlayerNoiseEmitter>();

            if (_config == null)
                Debug.LogError($"{nameof(PlayerController)}: назначь PlayerConfig в Inspector.", this);

            _cameraLocalY = _config != null ? _config.StandingCameraY : 1.65f;
            if (_cameraTransform != null)
            {
                var cam = _cameraTransform.GetComponent<Camera>();
                if (cam != null)
                    cam.nearClipPlane = 0.05f;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ApplyStandingCapsule()
        {
            float h = _config != null ? _config.StandingHeight : 1.8f;
            _capsule.height = h;
            _capsule.radius = 0.4f;
            _capsule.center = new Vector3(0f, h * 0.5f, 0f);
        }

        private void Update()
        {
            if (_config == null || Keyboard.current == null || Mouse.current == null)
                return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame
                && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                _body.linearVelocity = Vector3.zero;
                _body.position = new Vector3(0f, 0.05f, 0f);
                transform.position = new Vector3(0f, 0.05f, 0f);
            }

            Look();
            UpdateCrouchAndCamera();
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
                return;

            // Жёстко держим камеру в глазах — без съезда вбок (из‑за него и было «за текстурой»)
            _cameraTransform.localPosition = new Vector3(0f, _cameraLocalY, 0f);
            _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void FixedUpdate()
        {
            if (_config == null || Keyboard.current == null)
                return;

            ProbeGround();
            MoveFixed();
        }

        private void Look()
        {
            if (_cameraTransform == null)
                return;

            Vector2 delta = Mouse.current.delta.ReadValue();
            float yaw = delta.x * _config.MouseSensitivity;
            float pitchDelta = -delta.y * _config.MouseSensitivity;

            transform.Rotate(0f, yaw, 0f);
            _pitch = Mathf.Clamp(_pitch + pitchDelta, _config.MinPitch, _config.MaxPitch);
        }

        private void UpdateCrouchAndCamera()
        {
            _isCrouching = Keyboard.current.leftCtrlKey.isPressed
                           || Keyboard.current.cKey.isPressed;

            float targetHeight = _isCrouching ? _config.CrouchHeight : _config.StandingHeight;
            float targetCamY = _isCrouching ? _config.CrouchCameraY : _config.StandingCameraY;

            float newHeight = Mathf.Lerp(
                _capsule.height,
                targetHeight,
                1f - Mathf.Exp(-_config.CrouchLerp * Time.deltaTime));

            _capsule.height = newHeight;
            _capsule.radius = 0.4f;
            _capsule.center = new Vector3(0f, newHeight * 0.5f, 0f);

            _cameraLocalY = Mathf.Lerp(
                _cameraLocalY,
                targetCamY,
                1f - Mathf.Exp(-_config.CrouchLerp * Time.deltaTime));
        }

        private void ProbeGround()
        {
            float rayLen = _capsule.center.y + 0.15f;
            _grounded = Physics.SphereCast(
                transform.position + Vector3.up * (_capsule.radius + 0.05f),
                _capsule.radius * 0.9f,
                Vector3.down,
                out _,
                rayLen,
                ~0,
                QueryTriggerInteraction.Ignore);
        }

        private void MoveFixed()
        {
            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;

            bool sprint = !_isCrouching
                          && (Keyboard.current.leftShiftKey.isPressed
                              || Keyboard.current.rightShiftKey.isPressed);

            float speed = sprint ? _config.SprintSpeed : _config.WalkSpeed;
            if (_isCrouching)
                speed *= _config.CrouchSpeedMultiplier;

            bool isMoving = input.sqrMagnitude > 0.001f;
            Vector3 wish = Vector3.zero;
            if (isMoving)
            {
                input.Normalize();
                wish = (transform.right * input.x + transform.forward * input.y) * speed;
            }

            Vector3 v = _body.linearVelocity;
            v.x = wish.x;
            v.z = wish.z;

            if (_grounded && v.y < 0f)
                v.y = -0.5f;

            _body.linearVelocity = v;

            if (_noiseEmitter != null)
                _noiseEmitter.Notify(isMoving, sprint, _isCrouching);
        }
    }
}
