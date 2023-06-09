using Cyggie.FileManager.Runtime.ServicesNS;
using Cyggie.Main.Runtime;
using Cyggie.Main.Runtime.ServicesNS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileManagerTest : MonoBehaviour
{
    private FileManagerService _service = null;
    private FileManagerService Service => _service ??= ServiceManager.Get<FileManagerService>();

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<FileManagerTestObject> testObjects = Service.Get<FileManagerTestObject>();
        FileManagerTestObject testObject = testObjects.FirstOrDefault() ?? new FileManagerTestObject();

        ++testObject.Test;
        Log.Debug("Test object: " + testObject.Test, nameof(FileManagerTest));
        testObject.Save();

        Log.Debug("Go to the save folder path and notice the test object save file.", nameof(FileManagerTest));
    }
}
