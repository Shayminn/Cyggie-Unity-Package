using Cyggie.Runtime.SQLite.Models;
using Cyggie.Runtime.SQLite.Utils.Attributes;

public class DBTestObject : SQLiteObject
{
    [SQLiteProperty]
    public string Test { get; private set; } = "Ayaya";

    public DBTestObject(string test)
    {
        Test = test;
    }
}