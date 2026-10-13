using UnityEngine;
using UnityEngine.InputSystem;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Фонарик игрока: Spot (луч) + Point fill (заливка коридора рядом).
    /// F — вкл/выкл. Баланс в <see cref="FlashlightConfig"/> (SO).
    /// Учи: ScriptableObject + LightType.Spot/Point + toggle state.
    /// </summary>
    public sealed class BrokenFlashlight : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private FlashlightConfig _config;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Light _spot;
        [SerializeField] private Light _fill;

        private bool _isOn;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_config == null)
                Debug.LogError($"{nameof(BrokenFlashlight)}: назначь FlashlightConfig.", this);

            EnsureLights();
            ApplyBeam();
            SetOn(_config != null && _config.StartOn);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
                SetOn(!_isOn);
        }

        public void SetOn(bool on)
        {
            _isOn = on;
            ApplyBeam();
            if (_spot != null)
                _spot.enabled = on;
            if (_fill != null)
                _fill.enabled = on;
        }

        public bool IsOn => _isOn;

        /// <summary>
        /// Если свет ещё не проставлен в Inspector — создаём под камерой (fallback).
        /// На чистой архитектуре Spot/Fill лучше держать на префабе игрока.
        /// </summary>
        private void EnsureLights()
        {
            Transform parent = _cameraTransform != null ? _cameraTransform : transform;

            if (_spot == null)
                _spot = GetOrCreateLight(parent, "BrokenFlashlight", new Vector3(0.12f, -0.08f, 0.2f));

            if (_fill == null)
                _fill = GetOrCreateLight(parent, "BrokenFlashlightFill", new Vector3(0f, -0.2f, 0.6f));
        }

        private static Light GetOrCreateLight(Transform parent, string name, Vector3 localPos)
        {
            Transform existing = parent.Find(name);
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
            }
            else
            {
                go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localPos;
                go.transform.localRotation = Quaternion.identity;
            }

            var light = go.GetComponent<Light>();
            if (light == null)
                light = go.AddComponent<Light>();
            return light;
        }

        private void ApplyBeam()
        {
            if (_config == null)
                return;

            if (_spot != null)
            {
                _spot.type = LightType.Spot;
                _spot.color = _config.Color;
                _spot.intensity = _config.SpotIntensity;
                _spot.range = _config.SpotRange;
                _spot.spotAngle = _config.SpotAngle;
                _spot.innerSpotAngle = _config.InnerSpotAngle;
                _spot.shadows = LightShadows.None;
                _spot.renderMode = LightRenderMode.ForcePixel;
            }

            if (_fill != null)
            {
                _fill.type = LightType.Point;
                _fill.color = _config.Color;
                _fill.intensity = _config.FillIntensity;
                _fill.range = _config.FillRange;
                _fill.shadows = LightShadows.None;
                _fill.renderMode = LightRenderMode.ForcePixel;
            }
        }
    }
}
