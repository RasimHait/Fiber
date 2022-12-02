using System;

namespace FiberFramework
{
    [Serializable]
    public abstract class FiberModel
    {
        public abstract void OnInitialize();
    }
}