using Cyggie.Main.Runtime.Utils.Helpers;
using System;
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

        [SerializeField, Tooltip("Value difference between start and target value to be considered having reached the target value.")]
        private float _valueDifference = 0.1f;

        /// <summary>
        /// Reference to the slider that this smoother affects
        /// </summary>
        public Slider Slider => _slider;

        /// <summary>
        /// Property for setting the value of the slider <br/>
        /// Use <see cref="SetValue(float, Action{float}, Action)"/> to register to transition actions
        /// </summary>
        public float Value { set => SetValue(value); }

        private bool _transitioning = false;

        private void Awake()
        {
            if (_slider == null)
            {
                _slider = GetComponent<Slider>();
            }
        }

        /// <summary>
        /// Sets the value of the slider for a smooth transition
        /// </summary>
        /// <param name="targetValue">Target value to transition to</param>
        /// <param name="onValueChanged">Called whenever the value is changed</param>
        /// <param name="onTransitionCompleted">Called when the transition is completed</param>
        public void SetValue(float targetValue, Action<float> onValueChanged = null, Action onTransitionCompleted = null)
        {
            if (_transitioning)
            {
                FloatHelper.CancelSmoothTransition(this);
            }

            onValueChanged += OnSliderValueChanged;
            _transitioning = true;
            FloatHelper.SmoothTransition(this, _slider.value, targetValue, _smoothSpeed, _valueDifference, onValueChanged, () =>
            {
                _transitioning = false;
                onValueChanged -= OnSliderValueChanged;
                onTransitionCompleted?.Invoke();
            });
        }

        /// <summary>
        /// Sets the value of the slider without a smooth transition
        /// </summary>
        /// <param name="targetValue">Target value to set</param>
        public void SetValueWithoutSmoothing(float targetValue)
        {
            _slider.value = targetValue;
        }

        private void OnSliderValueChanged(float value)
        {
            _slider.value = value;
        }
    }
}
