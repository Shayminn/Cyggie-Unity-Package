using Cyggie.SceneChanger.Runtime;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class SceneChangerTest : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private string _sceneToChangeTo = "";

    private SceneChangerControls _controls;

    // Start is called before the first frame update
    private void Awake()
    {
        _controls = new SceneChangerControls();
        _controls.SceneChangeControls.ChangeScene.performed += OnChangeScenePerformed;
        _controls.SceneChangeControls.Fade.performed += OnFadePerformed;
    }

    private void OnEnable()
    {
        _controls?.Enable();
    }

    private void OnDisable()
    {
        _controls?.Disable();
    }

    private void OnChangeScenePerformed(CallbackContext ctx)
    {
        SceneChanger.ChangeScene(_sceneToChangeTo, ChangeSceneSettings.EnableAllWithKeyboardInput);
    }

    private void OnFadePerformed(CallbackContext ctx)
    {
        SceneChanger.Fade(1);
    }
}