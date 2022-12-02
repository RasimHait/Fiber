using System;
using System.Collections.Generic;
using System.Linq;

namespace FiberFramework
{
    internal class FiberControllerContainer : IDisposable
    {
        public IFiberController this[int index] => _controllers[index];
        private readonly List<IFiberController> _controllers;


        public FiberControllerContainer()
        {
            _controllers = new List<IFiberController>();
        }

        public void AddController(IFiberController controller)
        {
            if (_controllers.Contains(controller))
            {
                throw new ArgumentException($"({controller}) is already registered in container of {controller.GetType()}(s).");
            }

            _controllers.Add(controller);
        }

        public void RemoveController(IFiberController controller)
        {
            if (!_controllers.Contains(controller))
            {
                throw new ArgumentException($"Container of {controller.GetType()}(s) is not contains {controller}.");
            }

            _controllers.Remove(controller);
        }

        public bool GetFirst<T>(out T result) where T : IFiberController
        {
            result = (T)_controllers.FirstOrDefault();
            return result != null;
        }

        public bool GetLast<T>(out T result) where T : IFiberController
        {
            result = (T)_controllers.LastOrDefault();
            return result != null;
        }

        public bool GetAll<T>(out IEnumerable<T> result) where T : IFiberController
        {
            result = _controllers.Cast<T>();
            return _controllers.Count > 0;
        }

        public bool Contains(IFiberController target)
        {
            return _controllers.Contains(target);
        }

        public void Dispose()
        {
            foreach (var controller in _controllers)
            {
                controller.Destroy();
            }

            _controllers.Clear();
        }
    }
}