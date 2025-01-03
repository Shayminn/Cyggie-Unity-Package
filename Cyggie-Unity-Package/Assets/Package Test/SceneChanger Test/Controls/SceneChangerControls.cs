//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.1
//     from Assets/Package Test/SceneChanger Test/Controls/SceneChangerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @SceneChangerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @SceneChangerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SceneChangerControls"",
    ""maps"": [
        {
            ""name"": ""SceneChangeControls"",
            ""id"": ""3ba6a1ec-6c63-49f2-9915-e2e0e336f150"",
            ""actions"": [
                {
                    ""name"": ""ChangeScene"",
                    ""type"": ""Button"",
                    ""id"": ""71e6bc64-4e6c-4e8b-8078-5b2e20a4cb2c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fade"",
                    ""type"": ""Button"",
                    ""id"": ""5f50e660-1064-4637-9fc1-56c7d4837028"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ef0421e2-37ed-4ddd-8acf-f6ad6bf7b845"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeScene"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bbdbe0c9-3401-473c-aa0f-649075c16407"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fade"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // SceneChangeControls
        m_SceneChangeControls = asset.FindActionMap("SceneChangeControls", throwIfNotFound: true);
        m_SceneChangeControls_ChangeScene = m_SceneChangeControls.FindAction("ChangeScene", throwIfNotFound: true);
        m_SceneChangeControls_Fade = m_SceneChangeControls.FindAction("Fade", throwIfNotFound: true);
    }

    ~@SceneChangerControls()
    {
        UnityEngine.Debug.Assert(!m_SceneChangeControls.enabled, "This will cause a leak and performance issues, SceneChangerControls.SceneChangeControls.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // SceneChangeControls
    private readonly InputActionMap m_SceneChangeControls;
    private List<ISceneChangeControlsActions> m_SceneChangeControlsActionsCallbackInterfaces = new List<ISceneChangeControlsActions>();
    private readonly InputAction m_SceneChangeControls_ChangeScene;
    private readonly InputAction m_SceneChangeControls_Fade;
    public struct SceneChangeControlsActions
    {
        private @SceneChangerControls m_Wrapper;
        public SceneChangeControlsActions(@SceneChangerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChangeScene => m_Wrapper.m_SceneChangeControls_ChangeScene;
        public InputAction @Fade => m_Wrapper.m_SceneChangeControls_Fade;
        public InputActionMap Get() { return m_Wrapper.m_SceneChangeControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SceneChangeControlsActions set) { return set.Get(); }
        public void AddCallbacks(ISceneChangeControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_SceneChangeControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SceneChangeControlsActionsCallbackInterfaces.Add(instance);
            @ChangeScene.started += instance.OnChangeScene;
            @ChangeScene.performed += instance.OnChangeScene;
            @ChangeScene.canceled += instance.OnChangeScene;
            @Fade.started += instance.OnFade;
            @Fade.performed += instance.OnFade;
            @Fade.canceled += instance.OnFade;
        }

        private void UnregisterCallbacks(ISceneChangeControlsActions instance)
        {
            @ChangeScene.started -= instance.OnChangeScene;
            @ChangeScene.performed -= instance.OnChangeScene;
            @ChangeScene.canceled -= instance.OnChangeScene;
            @Fade.started -= instance.OnFade;
            @Fade.performed -= instance.OnFade;
            @Fade.canceled -= instance.OnFade;
        }

        public void RemoveCallbacks(ISceneChangeControlsActions instance)
        {
            if (m_Wrapper.m_SceneChangeControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ISceneChangeControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_SceneChangeControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_SceneChangeControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public SceneChangeControlsActions @SceneChangeControls => new SceneChangeControlsActions(this);
    public interface ISceneChangeControlsActions
    {
        void OnChangeScene(InputAction.CallbackContext context);
        void OnFade(InputAction.CallbackContext context);
    }
}
