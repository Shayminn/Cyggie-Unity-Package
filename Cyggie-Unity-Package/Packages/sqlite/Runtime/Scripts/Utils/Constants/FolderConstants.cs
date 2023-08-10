using UnityEngine;
/// <summary>
/// Struct that contains SQLite folder constants
/// </summary>
internal struct FolderConstants
{
    internal const string cBlueprints = "Blueprints/";
    internal const string cSQLite = "SQLite/";

    internal static string cSQLiteBlueprints => $"{Application.dataPath}/{Cyggie.Main.Runtime.Utils.Constants.FolderConstants.cCyggie}{cSQLite}{cBlueprints}";

    internal static string cSQLiteStreamingAssets => $"{Application.streamingAssetsPath}/{Cyggie.Main.Runtime.Utils.Constants.FolderConstants.cCyggie}{cSQLite}";
}