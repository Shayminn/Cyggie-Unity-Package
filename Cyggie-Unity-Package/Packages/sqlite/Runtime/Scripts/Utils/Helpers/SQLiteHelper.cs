using Cyggie.Main.Runtime;
using System;
using UnityEngine;

namespace Cyggie.Runtime.SQLite.Utils.Helpers
{
    /// <summary>
    /// Utils for Custom SQL
    /// </summary>
    internal static class SQLiteHelper
    {
        /// <summary>
        /// Util method for converting values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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
                        if (!DateTime.TryParse((string) value, out DateTime result))
                        {
                            Log.Error($"[Cyggie.SQLite] Couldn't convert {value} to DateTime.", nameof(SQLiteHelper));
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
