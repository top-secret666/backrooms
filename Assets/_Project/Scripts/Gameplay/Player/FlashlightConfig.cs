using UnityEngine;

namespace Backrooms.Gameplay
{
    /// <summary>
    /// Баланс фонарика в ассете (ScriptableObject), не в коде на сцене.
    /// Создай: ПКМ → Create → Backrooms → Flashlight Config.
    /// </summary>
    [CreateAssetMenu(
        fileName = "FlashlightConfig",
        menuName = "Backrooms/Flashlight Config")]
    public sealed class FlashlightConfig : ScriptableObject
    {
        [Header("Spot beam")]
        [SerializeField] private Color _color = new Color(0.92f, 0.92f, 0.84f, 1f);
        [SerializeField] private float _spotIntensity = 5.5f;
        [SerializeField] private float _spotRange = 26f;
        [SerializeField] private float _spotAngle = 110f;
        [SerializeField] private float _innerSpotAngle = 70f;

        [Header("Near fill (коридор рядом)")]
        [SerializeField] private float _fillIntensity = 1.35f;
        [SerializeField] private float _fillRange = 7.5f;

        [Header("Start")]
        [SerializeField] private bool _startOn = false;

        public Color Color => _color;
        public float SpotIntensity => _spotIntensity;
        public float SpotRange => _spotRange;
        public float SpotAngle => _spotAngle;
        public float InnerSpotAngle => _innerSpotAngle;
        public float FillIntensity => _fillIntensity;
        public float FillRange => _fillRange;
        public bool StartOn => _startOn;
    }
}
