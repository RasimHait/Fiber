using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiberFramework
{
    [AddComponentMenu("")]
    internal class FiberMonoBridge : MonoBehaviour
    {
        private readonly List<IUpdateHandler>      _updateHandlers      = new List<IUpdateHandler>();
        private readonly List<IFixedUpdateHandler> _fixedUpdateHandlers = new List<IFixedUpdateHandler>();
        private readonly List<ILateUpdateHandler>  _lateUpdateHandlers  = new List<ILateUpdateHandler>();


        public void StoreHandlers(object handler)
        {
            _updateHandlers.Add(handler as IUpdateHandler);
            _fixedUpdateHandlers.Add(handler as IFixedUpdateHandler);
            _lateUpdateHandlers.Add(handler as ILateUpdateHandler);
        }

        public void PurgeHandlers(object handler)
        {
            _updateHandlers.Remove(handler as IUpdateHandler);
            _fixedUpdateHandlers.Remove(handler as IFixedUpdateHandler);
            _lateUpdateHandlers.Remove(handler as ILateUpdateHandler);
        }

        private void Update()
        {
            foreach (var handler in _updateHandlers)
            {
                handler?.Update();
            }
        }

        private void FixedUpdate()
        {
            foreach (var handler in _fixedUpdateHandlers)
            {
                handler?.FixedUpdate();
            }
        }

        private void LateUpdate()
        {
            foreach (var handler in _lateUpdateHandlers)
            {
                handler?.LateUpdate();
            }
        }
    }
}