using System;

namespace FiberFramework
{
    /// <summary>
    /// Sets custom description name from FiberObject controller list 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerDescription: Attribute
    {
        public readonly string Description;

        public ControllerDescription(string description)
        {
            Description = description;
        }
    }
}