using Cyggie.Plugins.Logs;
using System;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for serialization
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Verify the Inspector Reference of one or many <paramref name="objs"/> <br/>
        /// Objects must be <see cref="SerializableAttribute"/>
        /// </summary>
        /// <param name="objs">Array of objects to check</param>
        /// <returns>True if all <paramref name="objs"/> pass the null reference check</returns>
        public static bool CheckInspectorReference(params object[] objs)
        {
            foreach (object obj in objs)
            {
                if (!obj.GetType().IsSerializable)
                {
                    Log.Error($"Trying to check for Inspector reference for object {obj}, but it is not serializable.", nameof(SerializationHelper));
                    return false;
                }

                if (obj == null)
                {
                    Log.Error($"Object {obj} has a missing reference, assign it in the Inspector.", nameof(SerializationHelper));
                    return false;
                }
            }

            return true;
        }
    }
}
