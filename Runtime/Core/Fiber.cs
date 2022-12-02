using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiberFramework
{
    public static class Fiber
    {
        private static FiberMonoBridge                            _monoBridge;
        private static Dictionary<Type, FiberControllerContainer> _containers = new();
        private static bool                                       _initialized;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_initialized) return;

            Application.quitting += Clear;
            PrepareBridge();
        }


        private static void Clear()
        {
            _initialized          =  false;
            _monoBridge           =  null;
            _containers           =  new Dictionary<Type, FiberControllerContainer>();
            Application.quitting -= Clear;
        }


        private static void PrepareBridge()
        {
            _monoBridge ??= UnityEngine.Object.FindObjectOfType<FiberMonoBridge>();
            _monoBridge ??= new GameObject("[FiberMonoBridge]").AddComponent<FiberMonoBridge>();

            UnityEngine.Object.DontDestroyOnLoad(_monoBridge);
        }


        public static void Register<T>(T controller) where T : IFiberController
        {
            var type = controller.GetType();

            if (!_containers.ContainsKey(type))
            {
                var container = new FiberControllerContainer();
                _containers.Add(type, container);
            }

            var targetContainer = _containers[type];

            if (targetContainer.Contains(controller)) return;

            targetContainer.AddController(controller);

            _monoBridge.StoreHandlers(controller);

            if (controller.TryGetView(out FiberView view))
            {
                _monoBridge.StoreHandlers(view);
            }
        }


        public static void UnRegister<T>(T controller) where T : IFiberController
        {
            var type = controller.GetType();

            if (!_containers.ContainsKey(type)) return;

            var targetContainer = _containers[type];

            if (!targetContainer.Contains(controller)) return;

            targetContainer.RemoveController(controller);

            _monoBridge.PurgeHandlers(controller);

            if (controller.TryGetView(out FiberView view))
            {
                _monoBridge.PurgeHandlers(view);
            }
        }


        public static bool GetFirst<T>(out T result) where T : IFiberController
        {
            var type = typeof(T);

            if (_containers.ContainsKey(type))
            {
                if (_containers[type].GetFirst(out result))
                {
                    return true;
                }
            }

            result = default;
            return false;
        }


        public static bool GetLast<T>(out T result) where T : IFiberController
        {
            var type = typeof(T);

            if (_containers.ContainsKey(type))
            {
                if (_containers[type].GetLast(out result))
                {
                    return true;
                }
            }

            result = default;
            return false;
        }


        public static bool GetAll<T>(out IEnumerable<T> result) where T : IFiberController
        {
            var type = typeof(T);

            if (_containers.ContainsKey(type))
            {
                if (_containers[type].GetAll(out result))
                {
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}