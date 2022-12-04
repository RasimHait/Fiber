using System;

namespace FiberFramework
{
    /// <summary>
    /// Hides controller from FiberObject controller list 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HiddenController: Attribute
    {
        // Just an Editor mark.
    }
}