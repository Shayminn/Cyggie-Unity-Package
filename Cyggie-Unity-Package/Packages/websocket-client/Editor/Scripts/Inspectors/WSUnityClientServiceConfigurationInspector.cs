using Cyggie.Main.Editor.Utils.Helpers;
using Cyggie.Plugins.WebSocket;
using Cyggie.Plugins.WebSocket.Models.Enums;
using UnityEditor;

namespace Cyggie.WebSocket.Editor
{
    /// <summary>
    /// Inspector for <see cref="WSUnityClientServiceConfiguration"/>
    /// </summary>
    [CustomEditor(typeof(WSUnityClientServiceConfiguration))]
    public class WSUnityClientServiceConfigurationInspector : UnityEditor.Editor
    {
        private SerializedProperty _reconnectionType = null;
        private SerializedProperty _reconnectionDelay = null;

        private WSUnityClientServiceConfiguration _config = null;

        private void OnEnable()
        {
            _config = target as WSUnityClientServiceConfiguration;

            _reconnectionType = serializedObject.FindProperty("_reconnectionType");
            _reconnectionDelay = serializedObject.FindProperty("_reconnectionDelay");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            EditorGUILayoutHelper.DrawScriptReference(_config);

            EditorGUILayout.PropertyField(_reconnectionType);

            WSReconnectionType reconnectionType = (WSReconnectionType) _reconnectionType.intValue;
            switch (reconnectionType)
            {
                case WSReconnectionType.NoReconnect:
                    break;
                case WSReconnectionType.ReconnectOnce:
                case WSReconnectionType.ReconnectConsistentDelay:
                case WSReconnectionType.ReconnectIncrementalDelay:
                    EditorGUILayout.PropertyField(_reconnectionDelay);
                    break;
            }
        }
    }
}
