using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.Plugins.Logs;
using Cyggie.Plugins.SQLite;
using Cyggie.SQLite.Runtime.ServicesNS;
using System.Collections.Generic;
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
        if (Service.TryGetDatabase(_databaseName, out _db))
        {
            if (_db.ReadAll(_testTableName, out IEnumerable<Test> objs))
            {
                foreach (Test test in objs)
                {
                    Log.Debug($"Read All: Col1: {test.TestCol1} | Col2: {test.TestCol2}", nameof(SQLiteTest));
                }
            }
        }
    }
}