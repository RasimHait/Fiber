using System.Collections.Generic;
using UnityEngine;

namespace FiberFramework
{
    [AddComponentMenu("")]
    internal sealed class FiberMonoBridge : MonoBehaviour
    {
        private readonly List<IFiberHandler> _handlers = new();


        public void StoreHandlers(object target)
        {
            AddHandlers(target);
        }


        public void PurgeHandlers(object target)
        {
            RemoveHandlers(target);
        }


        private void AddHandlers(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj == null) continue;

                RegisterHandler<IUpdateHandler>(obj);
                RegisterHandler<IFixedUpdateHandler>(obj);
                RegisterHandler<ILateUpdateHandler>(obj);
            }
        }


        private void RemoveHandlers(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj == null) continue;

                UnregisterHandler<IUpdateHandler>(obj);
                UnregisterHandler<IFixedUpdateHandler>(obj);
                UnregisterHandler<ILateUpdateHandler>(obj);
            }
        }


        private void RegisterHandler<T>(object target) where T : IFiberHandler
        {
            if (target is T handler && !_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }


        private void UnregisterHandler<T>(object target) where T : IFiberHandler
        {
            if (target is T handler && _handlers.Contains(handler))
            {
                _handlers.Remove(handler);
            }
        }


        private void Update()      => _handlers.ForEach(x => (x as IUpdateHandler)?.Update());
        private void FixedUpdate() => _handlers.ForEach(x => (x as IFixedUpdateHandler)?.FixedUpdate());
        private void LateUpdate()  => _handlers.ForEach(x => (x as ILateUpdateHandler)?.LateUpdate());
    }
}