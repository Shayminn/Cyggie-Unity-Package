using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cyggie.Main.Runtime.Utils.Helpers
{
    /// <summary>
    /// Helper class for <see cref="EventSystem"/>
    /// </summary>
    public static class EventSystemHelper
    {
        /// <summary>
        /// Simulate a pointer click (pointer down then up)
        /// </summary>
        /// <param name="target">Target to perform this click on</param>
        /// <param name="onPointerDown">Action called after the pointer is down, before it gets back up</param>
        public static void SimulateClick(GameObject target, Action onPointerDown = null)
        {
            SimulatePointerDown(target);
            onPointerDown?.Invoke();

            SimulatePointerUp(target);
        }

        /// <summary>
        /// Simulate a pointer click (pointer down then up) <br/>
        /// Calls the button's onClick when the pointer is down
        /// </summary>
        /// <param name="target">Button to perform this click on</param>
        /// <param name="onPointerDown">Action called after the pointer is down, before it gets back up</param>
        public static void SimulateClick(Button target, Action onPointerDown = null)
        {
            onPointerDown += () => target.onClick?.Invoke();
            SimulateClick(target.gameObject, onPointerDown);
        }

        /// <summary>
        /// Simulate a pointer down <br/>
        /// </summary>
        /// <param name="target">Target to perform this pointer event on</param>
        public static void SimulatePointerDown(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        }

        /// <summary>
        /// Simulate a pointer up <br/>
        /// </summary>
        /// <param name="target">Target to perform this pointer event on</param>
        public static void SimulatePointerUp(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
