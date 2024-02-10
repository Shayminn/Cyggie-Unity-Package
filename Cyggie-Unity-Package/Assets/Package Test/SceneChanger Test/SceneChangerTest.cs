using Cyggie.Plugins.Services.Models;
using Cyggie.SceneChanger.Runtime.ServicesNS;
using Cyggie.SceneChanger.Runtime.Settings;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class SceneChangerTest : MonoBehaviour
{
    [SerializeField, Tooltip("Target scene to change to.")]
    private string _sceneToChangeTo = "";

    private SceneChangerControls _controls;

    private SceneChangerService _sceneChangerService = null;
    private SceneChangerService SceneChangerService => _sceneChangerService ??= ServiceManager.Get<SceneChangerService>();

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

    private void Start()
    {
    }

    private void OnChangeScenePerformed(CallbackContext ctx)
    {
        SceneChangerService.ChangeScene(_sceneToChangeTo, ChangeSceneSettings.EnableAllWithKeyboardInput);
    }

    private void OnFadePerformed(CallbackContext ctx)
    {
        SceneChangerService.Fade(1);
    }
}