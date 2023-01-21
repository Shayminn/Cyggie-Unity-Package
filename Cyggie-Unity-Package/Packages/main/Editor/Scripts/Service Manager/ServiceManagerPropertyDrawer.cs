using Cyggie.Main.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ServiceManagerSettings))]
    public class ServiceManagerSettingsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
}
