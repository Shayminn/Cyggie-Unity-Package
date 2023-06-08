using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyggie.Main.Runtime
{
    /// <summary>
    /// Event arguments for a log of type <see cref="LogTypes.Error"/>
    /// </summary>
    public class ErrorLogEventArgs : LogEventArgs
    {
        internal ErrorLogEventArgs(string log, string tag) : base(log, tag)
        {

        }
    }
}
