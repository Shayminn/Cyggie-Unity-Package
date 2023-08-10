using Cyggie.Plugins.Logs;
using System;

namespace Cyggie.Plugins.SQLite.Utils.Helpers
{
    /// <summary>
    /// Helper class for SQLite
    /// </summary>
    internal static class SQLiteHelper
    {
        /// <summary>
        /// Util method for converting an row-column's value to its appropriate data type
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="t">The system type of the object</param>
        /// <returns>The object value converted to its appropriate type</returns>
        internal static object ConvertValue(object value, Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                    return Convert.ToByte(value);

                case TypeCode.SByte:
                    return Convert.ToSByte(value);

                case TypeCode.UInt16:
                    return Convert.ToUInt16(value);

                case TypeCode.UInt32:
                    return Convert.ToUInt32(value);

                case TypeCode.UInt64:
                    return Convert.ToUInt64(value);

                case TypeCode.Int16:
                    return Convert.ToInt16(value);

                case TypeCode.Int32:
                    return Convert.ToInt32(value);

                case TypeCode.Int64:
                    return Convert.ToInt64(value);

                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);

                case TypeCode.Double:
                    return Convert.ToDouble(value);

                case TypeCode.Single:
                    return Convert.ToSingle(value);

                case TypeCode.DateTime:
                    {
                        if (!DateTime.TryParse((string)value, out DateTime result))
                        {
                            Log.Error($"Couldn't convert {value} to DateTime.", nameof(SQLiteHelper));
                            return value;
                        }

                        return result;
                    }

                default:
                    return value;
            }
        }
    }
}
