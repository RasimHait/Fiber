using System;

namespace FiberFramework
{
    /// <summary>
    /// Sets custom controller name from FiberObject controller list 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerName: Attribute
    {
        public readonly string CustomName;

        public ControllerName(string customName)
        {
            CustomName = customName;
        }
    }
}