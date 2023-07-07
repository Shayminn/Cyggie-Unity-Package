using Cyggie.Plugins.Logs;
using Cyggie.SceneChanger.Runtime.InputSystems;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Model class to handle input bindings for <see cref="SceneChangerService.ChangeScene"/> blocking the scene change until the desired input is performed
    /// </summary>
    public class ChangeSceneInputSettings
    {
        #region Static defaults

        /// <summary>
        /// Default input binding for keyboards
        /// </summary>
        public static readonly ChangeSceneInputSettings DefaultKeyboard = new ChangeSceneInputSettings(
            new InputBinding("<Keyboard>/anyKey")
        );

        public static readonly ChangeSceneInputSettings DefaultMouse = new ChangeSceneInputSettings(
            new InputBinding("<Mouse>/press")
        );

        public static readonly ChangeSceneInputSettings DefaultKeyboardAndMouse = new ChangeSceneInputSettings(DefaultKeyboard, DefaultMouse);

        public static readonly ChangeSceneInputSettings DefaultTouch = new ChangeSceneInputSettings(
            new InputBinding("<Touchscreen>/Press")
        );

        public static readonly ChangeSceneInputSettings DefaultGamepad = new ChangeSceneInputSettings(
            new InputBinding("<Gamepad>/start")
        );

        public static readonly ChangeSceneInputSettings DefaultAll = new ChangeSceneInputSettings(DefaultKeyboardAndMouse, DefaultTouch, DefaultGamepad);

        #endregion

        /// <summary>
        /// Static input system for handling inputs
        /// </summary>
        private readonly static SceneChangerInputSystem _inputSystem = new SceneChangerInputSystem();

        /// <summary>
        /// Input bindings of current loading screen
        /// </summary>
        private readonly InputBinding[] _inputBindings = null;

        /// <summary>
        /// Create an Input setting based on a singular binding
        /// </summary>
        /// <param name="binding">InputBinding to use</param>
        public ChangeSceneInputSettings(InputBinding binding)
        {
            _inputBindings = new InputBinding[] { binding };
        }

        /// <summary>
        /// Create an Input setting based on multiple bindings
        /// </summary>
        /// <param name="bindings">InputBindings to use</param>
        public ChangeSceneInputSettings(InputBinding[] bindings)
        {
            _inputBindings = bindings;
        }

        /// <summary>
        /// Create an Input setting based on another <see cref="ChangeSceneInputSettings"/>
        /// </summary>
        /// <param name="inputSettings">InputSettings to concatenate</param>
        public ChangeSceneInputSettings(params ChangeSceneInputSettings[] inputSettings)
        {
            _inputBindings = new InputBinding[0];
            foreach (ChangeSceneInputSettings inputSetting in inputSettings)
            {
                _inputBindings = _inputBindings.Concat(inputSetting._inputBindings).ToArray();
            }
        }

        /// <summary>
        /// Create an Input setting based on another <see cref="ChangeSceneInputSettings"/> with the addition 
        /// </summary>
        /// <param name="inputSetting">InputSettings to concatenate</param>
        /// <param name="bindings">Bindings to add to <paramref name="inputSetting"/></param>
        public ChangeSceneInputSettings(ChangeSceneInputSettings inputSetting, params InputBinding[] bindings)
        {
            _inputBindings = new InputBinding[inputSetting._inputBindings.Length + bindings.Length];

            // Copy first array
            inputSetting._inputBindings.CopyTo(_inputBindings, 0);

            // Copy second array
            bindings.CopyTo(_inputBindings, inputSetting._inputBindings.Length);
        }

        /// <summary>
        /// Coroutine to wait for input from input bindings <see cref="_inputBindings"/>
        /// </summary>
        internal IEnumerator WaitForInput()
        {
            // Update the input system with the new input bindings
            int index = 0;
            foreach (InputBinding inputBinding in _inputBindings)
            {
                if (string.IsNullOrEmpty(inputBinding.path))
                {
                    Log.Error($"Input binding's path is null or empty.", nameof(ChangeSceneInputSettings));
                    continue;
                }

                // Override existing binding if any
                if (index < _inputSystem.SceneChanger.Wait.bindings.Count)
                {
                    _inputSystem.SceneChanger.Wait.ApplyBindingOverride(index, inputBinding);
                }
                // Add new binding
                else
                {
                    _inputSystem.SceneChanger.Wait.AddBinding(inputBinding);
                }
            }

            // Register handler for input system performed
            bool inputPerformed = false;
            void OnWaitPerformed(CallbackContext ctx)
            {
                inputPerformed = true;
                _inputSystem.SceneChanger.Wait.performed -= OnWaitPerformed;
            }
            _inputSystem.SceneChanger.Wait.performed += OnWaitPerformed;

            _inputSystem.Enable();

            // Wait for input binding
            while (!inputPerformed) yield return null;

            _inputSystem.Disable();

            // Clear bindings by setting them to empty string
            InputBinding[] usedBindings = _inputSystem.SceneChanger.Wait.bindings.Where(x => !string.IsNullOrEmpty(x.action)).ToArray();
            for (int i = 0; i < usedBindings.Length; i++)
            {
                _inputSystem.SceneChanger.Wait.ApplyBindingOverride(i, "");
            }
        }
    }
}
