using UnityEngine;

namespace FiberFramework
{
    public class FiberControllerConfigurations
    {
        [SerializeField]
        private bool _dontDestroyOnLoad;
        public bool DestroyOnLoad => _dontDestroyOnLoad;
    }
}