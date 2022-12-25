using Cyggie.SceneChanger.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangerTest : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private bool _onStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if (_onStart)
        {
            SceneChanger.ChangeScene(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneChanger.ChangeScene(1);
        }
    }
}
