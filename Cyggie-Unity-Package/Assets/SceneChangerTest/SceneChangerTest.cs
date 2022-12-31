using Cyggie.SceneChanger.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerTest : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private bool _onStart = true;

    [SerializeField, Tooltip("")]
    private string _sceneToChangeTo = "";

    // Start is called before the first frame update
    void Start()
    {
        if (_onStart)
        {
            SceneChanger.ChangeScene(_sceneToChangeTo, ChangeSceneSettings.EnableAll);
            //SceneChanger.ChangeScene("Scene 2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneChanger.ChangeScene(_sceneToChangeTo, ChangeSceneSettings.EnableAll);
        }
    }
}