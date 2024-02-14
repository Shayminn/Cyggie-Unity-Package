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
        /// <returns>True if any <paramref name="objs"/> has a null reference</returns>
        public static bool HasMissingReference(params object[] objs)
        {
            foreach (object obj in objs)
            {
                if (!obj.GetType().IsSerializable)
                {
                    Log.Error($"Trying to check for Inspector reference for object {obj}, but it is not serializable.", nameof(SerializationHelper));
                    return true;
                }

                if (obj == null)
                {
                    Log.Error($"Object {obj} has a missing reference, assign it in the Inspector.", nameof(SerializationHelper));
                    return true;
                }
            }

            return false;
        }
    }
}
