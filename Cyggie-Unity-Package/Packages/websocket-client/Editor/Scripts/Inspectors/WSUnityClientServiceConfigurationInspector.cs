using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Plugins.SignalR.Models.Enums;
using Cyggie.Plugins.SignalR.Services;
using UnityEditor;

namespace Cyggie.WebSocket.Editor
{
    /// <summary>
    /// Inspector for <see cref="WSUnityClientServiceConfiguration"/>
    /// </summary>
    [CustomEditor(typeof(SignalRUnityClientServiceConfiguration))]
    public class WSUnityClientServiceConfigurationInspector : UnityEditor.Editor
    {
        private SerializedProperty _reconnectionType = null;
        private SerializedProperty _reconnectionDelay = null;

        private SignalRUnityClientServiceConfiguration _config = null;

        private void OnEnable()
        {
            _config = target as SignalRUnityClientServiceConfiguration;

            _reconnectionType = serializedObject.FindProperty("_reconnectionType");
            _reconnectionDelay = serializedObject.FindProperty("_reconnectionDelay");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayoutHelper.DrawScriptReference(_config);

            EditorGUILayout.PropertyField(_reconnectionType);

            SignalRReconnectionType reconnectionType = (SignalRReconnectionType) _reconnectionType.intValue;
            switch (reconnectionType)
            {
                case SignalRReconnectionType.NoReconnect:
                    break;
                case SignalRReconnectionType.ReconnectOnce:
                case SignalRReconnectionType.ReconnectConsistentDelay:
                case SignalRReconnectionType.ReconnectIncrementalDelay:
                    EditorGUILayout.PropertyField(_reconnectionDelay);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
