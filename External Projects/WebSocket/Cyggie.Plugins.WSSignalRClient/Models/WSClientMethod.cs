using Cyggie.Plugins.Logs;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Cyggie.Plugins.WebSocket.Models
{
    /// <summary>
    /// Base Model for a WS Client method
    /// </summary>
    public class WSClientMethod
    {
        /// <summary>
        /// Method name that is registered
        /// </summary>
        public string MethodName { get; protected set; } = string.Empty;

        /// <summary>
        /// Array of type parameters for arguments
        /// </summary>
        public Type[] ParameterTypes { get; protected set; } = Array.Empty<Type>();

        /// <summary>
        /// Callback called when the method is called
        /// </summary>
        public Action<object?[]>? Callback { get; protected set; } = null;

        /// <summary>
        /// Create a WS Client method with no parameters
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action callback)
        {
            MethodName = callback.Method.Name;
            Callback = (object?[] objs) =>
            {
                callback?.Invoke();
            };
        }

        internal WSClientMethod()
        {
        }

        internal string PrintParams()
        {
            StringBuilder builder = new StringBuilder();

            foreach (Type type in ParameterTypes)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(", ");
                }

                builder.Append(type.Name);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Filter the type to string if it is not a primitive type (in order to use JSON serialization
        /// </summary>
        /// <typeparam name="T">Type param</typeparam>
        /// <returns>Filtered type</returns>
        protected Type FilterType<T>()
        {
            return typeof(T).IsPrimitive ? typeof(T) : typeof(string);
        }

        /// <summary>
        /// Filter object to type <typeparamref name="T"/> <br/>
        /// Auto converts it through JSON serialization if is not primitive
        /// </summary>
        /// <typeparam name="T">Type param</typeparam>
        /// <param name="obj">Object to filter</param>
        /// <returns>Filtered object</returns>
        protected T FilterObject<T>(object obj)
        {
#pragma warning disable CS8603 // Possible null reference return.
            if (obj == null) return default;
#pragma warning restore CS8603 // Possible null reference return.

#pragma warning disable CS8603 // Possible null reference return.
            return typeof(T).IsPrimitive ?
                   (T) obj :
                   JsonConvert.DeserializeObject<T>(obj.ToString());
#pragma warning restore CS8603 // Possible null reference return.
        }
    }

    /// <summary>
    /// Model for a WS Client method with 1 parameter
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    public class WSClientMethod<T1> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 1 parameter callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[] { FilterType<T1>() };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                callback?.Invoke(FilterObject<T1>(objs[0]));
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 2 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    public class WSClientMethod<T1, T2> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 2 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 2 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    public class WSClientMethod<T1, T2, T3> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 3 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 4 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    /// <typeparam name="T4">Parameter 4 of method</typeparam>
    public class WSClientMethod<T1, T2, T3, T4> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 4 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3, T4> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>(),
                FilterType<T4>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2]),
                    FilterObject<T4>(objs[3])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 5 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    /// <typeparam name="T4">Parameter 4 of method</typeparam>
    /// <typeparam name="T5">Parameter 5 of method</typeparam>
    public class WSClientMethod<T1, T2, T3, T4, T5> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 5 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3, T4, T5> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>(),
                FilterType<T4>(),
                FilterType<T5>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2]),
                    FilterObject<T4>(objs[3]),
                    FilterObject<T5>(objs[4])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 6 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    /// <typeparam name="T4">Parameter 4 of method</typeparam>
    /// <typeparam name="T5">Parameter 5 of method</typeparam>
    /// <typeparam name="T6">Parameter 6 of method</typeparam>
    public class WSClientMethod<T1, T2, T3, T4, T5, T6> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 6 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3, T4, T5, T6> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>(),
                FilterType<T4>(),
                FilterType<T5>(),
                FilterType<T6>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2]),
                    FilterObject<T4>(objs[3]),
                    FilterObject<T5>(objs[4]),
                    FilterObject<T6>(objs[5])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 7 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    /// <typeparam name="T4">Parameter 4 of method</typeparam>
    /// <typeparam name="T5">Parameter 5 of method</typeparam>
    /// <typeparam name="T6">Parameter 6 of method</typeparam>
    /// <typeparam name="T7">Parameter 7 of method</typeparam>
    public class WSClientMethod<T1, T2, T3, T4, T5, T6, T7> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 7 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3, T4, T5, T6, T7> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>(),
                FilterType<T4>(),
                FilterType<T5>(),
                FilterType<T6>(),
                FilterType<T7>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2]),
                    FilterObject<T4>(objs[3]),
                    FilterObject<T5>(objs[4]),
                    FilterObject<T6>(objs[5]),
                    FilterObject<T7>(objs[6])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }

    /// <summary>
    /// Model for a WS Client method with 8 parameters
    /// </summary>
    /// <typeparam name="T1">Parameter 1 of method</typeparam>
    /// <typeparam name="T2">Parameter 2 of method</typeparam>
    /// <typeparam name="T3">Parameter 3 of method</typeparam>
    /// <typeparam name="T4">Parameter 4 of method</typeparam>
    /// <typeparam name="T5">Parameter 5 of method</typeparam>
    /// <typeparam name="T6">Parameter 6 of method</typeparam>
    /// <typeparam name="T7">Parameter 7 of method</typeparam>
    /// <typeparam name="T8">Parameter 8 of method</typeparam>
    public class WSClientMethod<T1, T2, T3, T4, T5, T6, T7, T8> : WSClientMethod
    {
        /// <summary>
        /// Create a WS Client method with 8 parameters callback
        /// </summary>
        /// <param name="callback">Callback invoked when received from the server</param>
        public WSClientMethod(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
        {
            MethodName = callback.Method.Name;

            ParameterTypes = new Type[]
            {
                FilterType<T1>(),
                FilterType<T2>(),
                FilterType<T3>(),
                FilterType<T4>(),
                FilterType<T5>(),
                FilterType<T6>(),
                FilterType<T7>(),
                FilterType<T8>()
            };

            Callback = (object?[] objs) =>
            {
                if (objs.Length != ParameterTypes.Length)
                {
                    Log.Error($"Found more arguments than supported length. Received {objs.Length}, expected {ParameterTypes.Length}", nameof(WSClientMethod<T1>));
                    return;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                // Suppressed cause sometimes we might want null values to be passed
                callback?.Invoke(
                    FilterObject<T1>(objs[0]),
                    FilterObject<T2>(objs[1]),
                    FilterObject<T3>(objs[2]),
                    FilterObject<T4>(objs[3]),
                    FilterObject<T5>(objs[4]),
                    FilterObject<T6>(objs[5]),
                    FilterObject<T7>(objs[6]),
                    FilterObject<T8>(objs[7])
                );
#pragma warning restore CS8604 // Possible null reference argument.
            };
        }
    }
}
