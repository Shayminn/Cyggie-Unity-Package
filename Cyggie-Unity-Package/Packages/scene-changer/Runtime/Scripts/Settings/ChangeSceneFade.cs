using System;
using UnityEngine;

namespace Cyggie.SceneChanger.Runtime.Settings
{
    /// <summary>
    /// Settings related to Fade on <see cref="SceneChanger.ChangeScene"/>
    /// </summary>
    [Serializable]
    public class ChangeSceneFade
    {
        public float Duration { get; set; } = 0.25f;

        public Color Color { get; set; } = Color.black;

        public static readonly ChangeSceneFade Default = new ChangeSceneFade()
        {
            Duration = 0.25f,
            Color = Color.black
        };
    }
}
