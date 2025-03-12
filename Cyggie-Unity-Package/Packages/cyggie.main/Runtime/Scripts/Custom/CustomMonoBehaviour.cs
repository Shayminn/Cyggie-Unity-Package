using Cyggie.Main.Runtime.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cyggie.Main.Runtime.Customs
{
    /// <summary>
    /// Custom class that extends <see cref="UnityEngine.MonoBehaviour"/> <br/>
    /// This class automatically checks for all [SerializeFields] and makes sure that it is not null / assigned
    /// </summary>
    public class CustomMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// MonoBehaviour awake
        /// </summary>
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            Type type = GetType();
            IEnumerable<FieldInfo> serializeFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                                .Where(x => x.GetCustomAttributes(typeof(SerializeField), false).Length > 0);

            foreach (FieldInfo fieldInfo in serializeFields)
            {
                object value = fieldInfo.GetValue(this);
                if (value is UnityEngine.Object unityObj)
                {
                    this.HasMissingReference(unityObj);
                }
                else
                {
                    this.HasMissingReference(value);
                }
            }
#endif
        }
    }
}
