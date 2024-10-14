using Cyggie.Main.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Cyggie.Main.Runtime.Components.UI
{
    /// <summary>
    /// Utility class that applies a smoother transition of value for a <see cref="Slider"/>
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class SliderSmoother : MonoBehaviour
    {
        [SerializeField, Tooltip("Reference to the target slider that will transition smoothly.")]
        private Slider _slider = null;

        [SerializeField, Tooltip("Smoothing speed multiplier.")]
        private float _smoothSpeed = 1f;

        private float _oldValue = 0;
        private bool _transitioning = false;

        private void Awake()
        {
            if (_slider == null)
            {
                _slider = GetComponent<Slider>();
            }

            _oldValue = _slider.value;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            if (_transitioning)
            {
                if (value == _oldValue) return;
                FloatHelper.CancelSmoothTransition(this);
            }

            _transitioning = true;
            FloatHelper.SmoothTransition(this, _oldValue, value, _smoothSpeed, OnValueChanged, OnTransitionCompleted);
        }

        private void OnValueChanged(float newValue)
        {
            _oldValue = newValue;
            _slider.value = newValue;
        }

        private void OnTransitionCompleted()
        {
            _transitioning = false;
        }
    }
}
