using Cyggie.FileManager.Runtime.Services;
using Cyggie.Main.Runtime.Services;
using System.Collections;
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
        testObject.Save();

        Debug.Log("Go to the save folder path and notice the test object save file.");
    }
}
