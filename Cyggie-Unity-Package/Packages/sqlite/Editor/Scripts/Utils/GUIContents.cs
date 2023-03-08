using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyggie.SQLite.Editor.Utils
{
    /// <summary>
    /// Styles for all EditorGUI drawings
    /// </summary>
    internal class GUIContents
    {
        // 
        //  SQLite Tab
        //
        internal static readonly GUIContent cDatabaseName = new GUIContent("Database Name", "The relative path to the database (.sqlite extension will be automatically added if not included).");
        internal static readonly GUIContent cReadOnly = new GUIContent("Read Only", "Whether the database connection should be in read-only mode.");
        internal static readonly GUIContent cReadAllOnStart = new GUIContent("Read All On Start", "Whether the service should read everything from the database on initialization.");
        internal static readonly GUIContent cAddSToTableName = new GUIContent("Add \"s\" to Table Names", "Enabling this will add a trailing \"s\" to the table name when it is not explicitly specified in the method.");

        //
        //  SQLite Tools Window
        //
        internal static readonly GUIContent cBlueprintPath = new GUIContent("Blueprint Path", "This path is relative from the Assets path.");
        internal static readonly GUIContent cQuickRunPath = new GUIContent("Execute Path", "This path is relative from the Assets path.");
        internal static readonly GUIContent cIncludeSubfolders = new GUIContent("Include Subfolders", "Whether it should include all subfolders within the Execute Path (if it is a directory).");
    }
}
