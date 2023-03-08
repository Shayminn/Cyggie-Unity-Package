using Cyggie.Main.Runtime.Services;
using Cyggie.SQLite.Runtime.Services;
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
            Debug.Log("Test object params length: " + testObject.SQLParams.Length);
            Debug.Log(testObject.Test);
        }

        IEnumerable<DBTestObject> testObjects = Service.Get<DBTestObject>();
        if (testObjects.Count() > 0)
        {
            Debug.Log("Get count: " + testObjects.Count());
            Debug.Log(testObjects.First().Test);
        }
    }
}