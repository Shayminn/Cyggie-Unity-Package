using Cyggie.Main.Runtime;
using Cyggie.Main.Runtime.ServicesNS;
using Cyggie.SQLite.Runtime.ServicesNS;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SQLiteTest : MonoBehaviour
{
    private SQLiteService _service = null;
    private SQLiteService Service => _service ??= ServiceManager.Get<SQLiteService>();

    private void Start()
    {
        if (!Service.Read(out DBTestObject testObject))
        {
            Service.Execute(@"CREATE TABLE IF NOT EXISTS ""dbtestobjects"" (
	                            ""id""	INTEGER,
	                            ""test""	TEXT,
	                            PRIMARY KEY(""id"" AUTOINCREMENT)
                            );");

            Service.Execute(@"INSERT INTO ""dbtestobjects"" (test) VALUES(@ayaya)", new SqliteParameter("ayaya", "ayaya"));
        }

        if (testObject != null)
        {
            Log.Debug("Test object params length: " + testObject.SQLParams.Length, nameof(SQLiteTest));
            Log.Debug(testObject.Test, nameof(SQLiteTest));
        }

        IEnumerable<DBTestObject> testObjects = Service.Get<DBTestObject>();
        if (testObjects.Count() > 0)
        {
            Log.Debug("Get count: " + testObjects.Count(), nameof(SQLiteTest));
            Log.Debug(testObjects.First().Test, nameof(SQLiteTest));
        }
    }
}