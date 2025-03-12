using UnityEngine;

namespace Cyggie.SQLite.Editor.Utils.Styles
{
    /// <summary>
    /// Styles for all EditorGUI drawings
    /// </summary>
    internal class GUIContents
    {
        internal static readonly GUIContent cIsEncrypted = new GUIContent("Is Encrypted:", "Whether the database should be encrypted.");
        internal static readonly GUIContent cIsReadOnly = new GUIContent("Is ReadOnly:", "Whether the database should be read-only.");

        internal static readonly GUIContent cOpenAllOnStart = new GUIContent("Read All On Start", "Whether the service should read everything from the database on initialization.");
        internal static readonly GUIContent cCreateBlueprint = new GUIContent("Create blueprint", "A blueprint can be used to restore a database to its previous state.");
    }
}
