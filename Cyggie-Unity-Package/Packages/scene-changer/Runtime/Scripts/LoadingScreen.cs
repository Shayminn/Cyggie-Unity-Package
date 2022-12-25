using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.SceneChanger.Runtime
{
    public class LoadingScreen : MonoBehaviour
    {
        public void SetProgress(float progress)
        {

        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
