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
            if (!_initialized)
            {
                Application.quitting += Clear;
                PrepareBridge();
            }
        }


        private static void Clear()
        {
            _initialized         =  false;
            _monoBridge          =  null;
            _containers          =  new Dictionary<Type, FiberControllerContainer>();
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
            var targetContainer = GetOrCreateContainer<T>();

            if (!targetContainer.Contains(controller))
            {
                targetContainer.AddController(controller);

                OperateHandlers(controller, HandlerStorageOperation.Store);
            }
        }


        public static void UnRegister<T>(T controller) where T : IFiberController
        {
            var targetContainer = GetOrCreateContainer<T>();

            if (targetContainer.Contains(controller))
            {
                targetContainer.RemoveController(controller);

                OperateHandlers(controller, HandlerStorageOperation.Purge);
            }
        }


        private static void OperateHandlers(IFiberController controller, HandlerStorageOperation operation)
        {
            switch (operation)
            {
                case HandlerStorageOperation.Store:
                    _monoBridge.StoreHandlers(controller);
                    break;
                case HandlerStorageOperation.Purge:
                    _monoBridge.PurgeHandlers(controller);
                    break;
            }

            if (controller is IViewContainer viewContainer)
            {
                switch (operation)
                {
                    case HandlerStorageOperation.Store:
                        _monoBridge.StoreHandlers(viewContainer.GetView());
                        break;
                    case HandlerStorageOperation.Purge:
                        _monoBridge.PurgeHandlers(viewContainer.GetView());
                        break;
                }
            }
            
            if (controller is IModelContainer modelContainer)
            {
                switch (operation)
                {
                    case HandlerStorageOperation.Store:
                        _monoBridge.StoreHandlers(modelContainer.GetModel());
                        break;
                    case HandlerStorageOperation.Purge:
                        _monoBridge.PurgeHandlers(modelContainer.GetModel());
                        break;
                }
            }
        }


        private static FiberControllerContainer GetOrCreateContainer<T>()
        {
            var type = typeof(T);

            if (!_containers.ContainsKey(type))
            {
                var container = new FiberControllerContainer();
                _containers.Add(type, container);
            }

            return _containers[type];
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


        private enum HandlerStorageOperation
        {
            Store,
            Purge
        }
    }
}