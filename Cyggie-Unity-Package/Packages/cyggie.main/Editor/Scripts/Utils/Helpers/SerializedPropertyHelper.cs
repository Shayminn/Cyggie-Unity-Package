using Cyggie.Main.Editor.Utils.Extensions;
using Cyggie.Plugins.Logs;
using UnityEditor;

namespace Cyggie.Main.Editor.Utils.Helpers
{
    /// <summary>
    /// Helper class for managing SerializedProperty
    /// </summary>
    public static class SerializedPropertyHelper
    {
        /// <summary>
        /// Copy the value from a source property to a target property
        /// </summary>
        /// <param name="source">Source property to copy from</param>
        /// <param name="target">Target property to copy to</param>
        public static void Copy(SerializedProperty source, SerializedProperty target)
        {
            if (source.propertyType != target.propertyType)
            {
                Log.Error($"Trying to copy property value from {source.name} to {target.name} but they aren't the same type ({source.propertyType} to {target.propertyType}).", nameof(SerializedPropertyHelper));
                return;
            }

            object value = source.GetValue();
            target.SetValue(value);
        }

        /// <summary>
        /// Check whether a source property's value is equal to the target property's value
        /// </summary>
        /// <param name="source">Source property to check</param>
        /// <param name="target">Target property to compare</param>
        /// <returns>Equal?</returns>
        public static bool IsEqual(SerializedProperty source, SerializedProperty target)
        {
            if (source.propertyType != target.propertyType)
            {
                Log.Error($"Trying to check for equivalent value with {source.name} and {target.name} but they aren't the same type ({source.propertyType} and {target.propertyType}).", nameof(SerializedPropertyHelper));
                return false;
            }

            object sourceValue = source.GetValue();
            object targetValue = target.GetValue();

            if (sourceValue == null || targetValue == null)
            {
                return sourceValue == targetValue;
            }

            return source.GetValue().Equals(target.GetValue());
        }
    }
}
