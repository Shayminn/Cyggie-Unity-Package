using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Utils.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cyggie.Main.Runtime.Attributes
{
    /// <summary>
    /// Static handler class to handle <see cref="CustomRuntimeInitializeOnLoadMethodAttribute"/>
    /// </summary>
    internal static class CustomRuntimeInitializeOnLoadHandler
    {
        private static Dictionary<RuntimeInitializeLoadType, List<MethodInfo>> _methodDictionary = new Dictionary<RuntimeInitializeLoadType, List<MethodInfo>>();

        /// <summary>
        /// Get all methods that have <see cref="CustomRuntimeInitializeOnLoadMethodAttribute"/>
        /// </summary>
        static CustomRuntimeInitializeOnLoadHandler()
        {
            IEnumerable<MethodInfo> methodInfos = ReflectionHelper.GetAllTypesInDomain()
                                                        .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                                                        .Where(method => method.GetCustomAttributes(typeof(CustomRuntimeInitializeOnLoadMethodAttribute), false).Length > 0);

            foreach (MethodInfo info in methodInfos)
            {
                if (!info.IsStatic)
                {
                    Log.Error($"Runtime initialize on load method is not static! It needs to be marked as static.", nameof(CustomRuntimeInitializeOnLoadHandler));
                    continue;
                }

                if (info.ReturnType != typeof(void))
                {
                    Log.Error($"Runtime initialize on load method has a return parameter. It needs to be void.", nameof(CustomRuntimeInitializeOnLoadHandler));
                    continue;
                }

                if (info.ContainsGenericParameters)
                {
                    Log.Error($"Runtime initialize on load method contains generic parameters. This is not allowed.", nameof(CustomRuntimeInitializeOnLoadHandler));
                    continue;
                }

                ParameterInfo[] parameters = info.GetParameters();
                if (parameters != null && parameters.Length > 0)
                {
                    Log.Error($"Runtime initialize on load method contains parameters. This is not allowed.", nameof(CustomRuntimeInitializeOnLoadHandler));
                    continue;
                }

                CustomRuntimeInitializeOnLoadMethodAttribute customAttr = info.GetCustomAttribute<CustomRuntimeInitializeOnLoadMethodAttribute>();
                if (!_methodDictionary.ContainsKey(customAttr.LoadType))
                {
                    _methodDictionary[customAttr.LoadType] = new List<MethodInfo>();
                }

                _methodDictionary[customAttr.LoadType].Add(info);
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitializeOnLoadMethodAfterSceneLoad()
        {
            if (_methodDictionary.TryGetValue(RuntimeInitializeLoadType.AfterSceneLoad, out List<MethodInfo> methodInfos))
            {
                foreach (MethodInfo info in methodInfos)
                {
                    info.Invoke(null, null);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoadMethodBeforeSceneLoad()
        {
            if (_methodDictionary.TryGetValue(RuntimeInitializeLoadType.BeforeSceneLoad, out List<MethodInfo> methodInfos))
            {
                foreach (MethodInfo info in methodInfos)
                {
                    info.Invoke(null, null);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitializeOnLoadMethodAfterAssembliesLoaded()
        {
            if (_methodDictionary.TryGetValue(RuntimeInitializeLoadType.AfterAssembliesLoaded, out List<MethodInfo> methodInfos))
            {
                foreach (MethodInfo info in methodInfos)
                {
                    info.Invoke(null, null);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void RuntimeInitializeOnLoadMethodBeforeSplashScreen()
        {
            if (_methodDictionary.TryGetValue(RuntimeInitializeLoadType.BeforeSplashScreen, out List<MethodInfo> methodInfos))
            {
                foreach (MethodInfo info in methodInfos)
                {
                    info.Invoke(null, null);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeOnLoadMethodSubsystemRegistration()
        {
            if (_methodDictionary.TryGetValue(RuntimeInitializeLoadType.SubsystemRegistration, out List<MethodInfo> methodInfos))
            {
                foreach (MethodInfo info in methodInfos)
                {
                    info.Invoke(null, null);
                }
            }
        }
    }
}