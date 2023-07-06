using Mono.Data.Sqlite;
using System;
using System.IO;
using UnityEngine;

public class SQLiteTest : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private string _databasePath = "";

    private void Start()
    {
        try
        {
            string databasePath = $"{Application.streamingAssetsPath}/{_databasePath}";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            }

            Debug.Log($"Start: {databasePath}");
            SqliteConnection conn = new SqliteConnection($"Data Source={databasePath}");
            conn.Open();
            conn.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        //if (!Service.Read(out DBTestObject testObject))
        //{
        //    Service.Execute(@"CREATE TABLE IF NOT EXISTS ""dbtestobjects"" (
        //                     ""id""	INTEGER,
        //                     ""test""	TEXT,
        //                     PRIMARY KEY(""id"" AUTOINCREMENT)
        //                    );");

        //    Service.Execute(@"INSERT INTO ""dbtestobjects"" (test) VALUES(@ayaya)", new SqliteParameter("ayaya", "ayaya"));
        //}

        //if (testObject != null)
        //{
        //    Log.Debug("Test object params length: " + testObject.SQLParams.Length, nameof(SQLiteTest));
        //    Log.Debug(testObject.Test, nameof(SQLiteTest));
        //}

        //IEnumerable<DBTestObject> testObjects = Service.Get<DBTestObject>();
        //if (testObjects.Count() > 0)
        //{
        //    Log.Debug("Get count: " + testObjects.Count(), nameof(SQLiteTest));
        //    Log.Debug(testObjects.First().Test, nameof(SQLiteTest));
        //}
    }
}