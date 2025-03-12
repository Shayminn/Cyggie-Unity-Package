using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace Cyggie.Main.Editor.Utils.Extensions
{
    /// <summary>
    /// Extension class for <see cref="SerializedProperty"/>
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Get the value of this serialized property
        /// </summary>
        /// <param name="property">Property to get the value from</param>
        /// <returns>Property value</returns>
        /// <exception cref="NotSupportedException">Not supported for <see cref="SerializedPropertyType.Generic"/> and <see cref="SerializedPropertyType.Gradient"/></exception>
        public static object GetValue(this SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Generic => null,
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Boolean => property.boolValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue,
                SerializedPropertyType.ObjectReference => property.objectReferenceValue,
                SerializedPropertyType.LayerMask => property.intValue,
                SerializedPropertyType.Enum => property.enumValueIndex,
                SerializedPropertyType.Vector2 => property.vector2Value,
                SerializedPropertyType.Vector3 => property.vector3Value,
                SerializedPropertyType.Vector4 => property.vector4Value,
                SerializedPropertyType.Rect => property.rectValue,
                SerializedPropertyType.ArraySize => property.arraySize,
                SerializedPropertyType.Character => property.intValue,
                SerializedPropertyType.AnimationCurve => property.animationCurveValue,
                SerializedPropertyType.Bounds => property.boundsValue,
                SerializedPropertyType.Gradient => throw new NotSupportedException(),
                SerializedPropertyType.Quaternion => property.quaternionValue,
                SerializedPropertyType.ExposedReference => property.exposedReferenceValue,
                SerializedPropertyType.FixedBufferSize => property.fixedBufferSize,
                SerializedPropertyType.Vector2Int => property.vector2IntValue,
                SerializedPropertyType.Vector3Int => property.vector3IntValue,
                SerializedPropertyType.RectInt => property.rectIntValue,
                SerializedPropertyType.BoundsInt => property.boundsIntValue,
                SerializedPropertyType.ManagedReference => property.managedReferenceValue,
                SerializedPropertyType.Hash128 => property.hash128Value,
                _ => null,
            };
        }

        /// <summary>
        /// Set the value of this serialized property
        /// </summary>
        /// <param name="property">Property to set the value to</param>
        /// <param name="value">Value to set the property</param>
        /// <exception cref="NotSupportedException">Not supported for <see cref="SerializedPropertyType.Generic"/> and <see cref="SerializedPropertyType.Gradient"/></exception>
        public static void SetValue(this SerializedProperty property, object value)
        {
            if (value == null)
            {
                property.ClearValue();
                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic: throw new NotSupportedException();
                case SerializedPropertyType.Integer:
                    property.intValue = (int) value;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool) value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float) value;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = (string) value;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = (Color) value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = (UnityEngine.Object) value;
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = (int) value;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = (int) value;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2) value;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3) value;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4) value;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = (Rect) value;
                    break;
                case SerializedPropertyType.ArraySize:
                    property.arraySize = (int) value;
                    break;
                case SerializedPropertyType.Character:
                    property.intValue = (int) value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve) value;
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds) value;
                    break;
                case SerializedPropertyType.Gradient: throw new NotSupportedException();
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (Quaternion) value;
                    break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = (UnityEngine.Object) value;
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    // read-only
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = (Vector2Int) value;
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = (Vector3Int) value;
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = (RectInt) value;
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = (BoundsInt) value;
                    break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = (UnityEngine.Object) value;
                    break;
                case SerializedPropertyType.Hash128:
                    property.hash128Value = (Hash128) value;
                    break;
            }
        }

        /// <summary>
        /// Clear the value of this serialized property
        /// </summary>
        /// <param name="property">Property to set the value to</param>
        /// <exception cref="NotSupportedException">Not supported for <see cref="SerializedPropertyType.Generic"/> and <see cref="SerializedPropertyType.Gradient"/></exception>
        public static void ClearValue(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic: throw new NotSupportedException();
                case SerializedPropertyType.Integer: property.intValue = default; break;
                case SerializedPropertyType.Boolean: property.boolValue = default; break;
                case SerializedPropertyType.Float: property.floatValue = default; break;
                case SerializedPropertyType.String: property.stringValue = default; break;
                case SerializedPropertyType.Color: property.colorValue = default; break;
                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = default; break;
                case SerializedPropertyType.LayerMask: property.intValue = default; break;
                case SerializedPropertyType.Enum: property.enumValueIndex = default; break;
                case SerializedPropertyType.Vector2: property.vector2Value = default; break;
                case SerializedPropertyType.Vector3: property.vector3Value = default; break;
                case SerializedPropertyType.Vector4: property.vector4Value = default; break;
                case SerializedPropertyType.Rect: property.rectValue = default; break;
                case SerializedPropertyType.ArraySize: property.arraySize = default; break;
                case SerializedPropertyType.Character: property.intValue = default; break;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = default; break;
                case SerializedPropertyType.Bounds: property.boundsValue = default; break;
                case SerializedPropertyType.Gradient: throw new NotSupportedException();
                case SerializedPropertyType.Quaternion: property.quaternionValue = default; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = default; break;
                case SerializedPropertyType.FixedBufferSize: /* read-only */ break;
                case SerializedPropertyType.Vector2Int: property.vector2IntValue = default; break;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = default; break;
                case SerializedPropertyType.RectInt: property.rectIntValue = default; break;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = default; break;
                case SerializedPropertyType.ManagedReference: property.managedReferenceValue = default; break;
                case SerializedPropertyType.Hash128: property.hash128Value = default; break;
                default: return;
            }
        }
    }
}
