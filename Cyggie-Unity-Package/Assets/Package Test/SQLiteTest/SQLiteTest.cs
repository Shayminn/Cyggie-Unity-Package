using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite;
using Cyggie.SQLite.Runtime.ServicesNS;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Test class for SQLite package
/// </summary>
public class SQLiteTest : MonoBehaviour
{
    [SerializeField, Tooltip("Name of the database.")]
    private string _databaseName = "database";

    [SerializeField, Tooltip("Test table name in the database.")]
    private string _testTableName = "dbtest";

    [Space]
    [SerializeField, Tooltip("Reference to the dropdown to test the retrieved rows from the test database.")]
    private TMP_Dropdown _dropdown = null;

    private SQLiteService _service = null;
    private SQLiteService Service => _service ?? ServiceManager.Get<SQLiteService>();

    private SQLiteDatabase _db = null;

    private class Test : SQLiteObject
    {
        [SQLiteProperty]
        public int TestCol1 { get; set; }

        [SQLiteProperty]
        public string TestCol2 { get; set; }

        public Test(int testCol1, string testCol2)
        {
            TestCol1 = testCol1;
            TestCol2 = testCol2;
        }
    }

    private void Start()
    {
        if (_dropdown == null)
        {
            Log.Error("Unable to test database, dropdown reference is null.", nameof(SQLiteTest));
            return;
        }

        if (Service.TryGetDatabase(_databaseName, out _db))
        {
            if (_db.ReadAll(_testTableName, out IEnumerable<Test> objs))
            {
                _dropdown.ClearOptions();
                _dropdown.AddOptions(objs.Select(x => $"{x.TestCol1} ({x.TestCol2})").ToList());
            }
        }
        else
        {
            Log.Error("Unable to test database, database not found/missing.", nameof(SQLiteTest));
            return;
        }
    }
}