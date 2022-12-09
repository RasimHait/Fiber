using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FiberFramework
{
    public class FiberObject : MonoBehaviour
    {
        [SerializeReference] private IFiberController              _controller;
        [SerializeReference] private FiberModel                    _model;
        [SerializeReference] private FiberView                     _view;
        [SerializeReference] private FiberControllerConfigurations _configurations;
        [SerializeReference] private List<IFiberHandler>           _handlers;

        public bool HasController     => _controller != null;
        public bool HasModel          => _model      != null;
        public bool HasView           => _view       != null;
        public Type GetControllerType => _controller.GetType();
        public Type GetModelType      => _model.GetType();
        public Type GetViewType       => _view.GetType();


        public void Construct(Type controllerType)
        {
            Reset();

            _controller = (IFiberController)Activator.CreateInstance(controllerType);

            RefreshConfigurations();
            RefreshModel();
            RefreshView();

            UpdateHandlers();
        }


        private void UpdateHandlers()
        {
            StoreHandlers(_controller, _view, _model);
        }


        public bool TryGetController<T>(out T result) where T : IFiberController
        {
            if (HasController && _controller is T output)
            {
                result = output;
                return true;
            }

            result = default;
            return false;
        }


        public bool TryGetModel<T>(out T result) where T : FiberModel
        {
            if (_controller is IModelContainer modelContainer)
            {
                result = modelContainer.GetModel() as T;
                return true;
            }

            result = default;
            return false;
        }


        public bool TryGetView<T>(out T result) where T : FiberView
        {
            if (_controller is IViewContainer viewContainer)
            {
                result = viewContainer.GetView() as T;
                return true;
            }

            result = default;
            return false;
        }


        public void RefreshModel()
        {
            if (_controller != null)
            {
                var modelType = _controller.GetType().GetProperty("Model")?.PropertyType;

                if (modelType?.GetConstructor(Type.EmptyTypes) != null)
                {
                    var model = (FiberModel)Activator.CreateInstance(modelType);
                    SetModel(model);
                }
            }
        }


        public void RefreshView()
        {
            if (_controller != null)
            {
                var viewType = _controller.GetType().GetProperty("View")?.PropertyType;

                if (viewType?.GetConstructor(Type.EmptyTypes) != null)
                {
                    var view = (FiberView)Activator.CreateInstance(viewType);
                    SetView(view);
                }
            }
        }


        public void RefreshConfigurations()
        {
            if (_controller != null)
            {
                var config = (FiberControllerConfigurations)Activator.CreateInstance(typeof(FiberControllerConfigurations));
                SetConfigurations(config);
            }
        }


        private void SetModel(object value)
        {
            _model = (FiberModel)value;
        }


        private void SetView(object value)
        {
            _view = (FiberView)value;
            _view.Initialize(this);
        }


        private void SetConfigurations(object value)
        {
            _configurations = (FiberControllerConfigurations)value;
        }

        private void Awake()
        {
            if (_controller != null)
            {
                if (_controller is IModelContainer modelContainer)
                {
                    _model?.OnInitialize();
                    modelContainer.SetModel(_model);
                }

                if (_controller is IViewContainer viewContainer)
                {
                    _view?.Initialize(this);
                    viewContainer.SetView(_view);
                }

                _controller?.Initialize(_configurations);

                UpdateHandlers();
            }
        }


        private void Start()
        {
            _handlers.ForEach(x => (x as IStartHandler)?.Start());
        }


        private void StoreHandlers(params object[] objects)
        {
            _handlers = new List<IFiberHandler>();

            foreach (var obj in objects)
            {
                if (obj == null) continue;

                RegisterHandler<IStartHandler>(obj);
                RegisterHandler<IEnableHandler>(obj);
                RegisterHandler<IDisableHandler>(obj);
                RegisterHandler<IMouseDownHandler>(obj);
                RegisterHandler<IMouseEnterHandler>(obj);
                RegisterHandler<IMouseExitHandler>(obj);
                RegisterHandler<IMouseOverHandler>(obj);
                RegisterHandler<IMouseUpHandler>(obj);
                RegisterHandler<IBecameInvisibleHandler>(obj);
                RegisterHandler<IBecameVisibleHandler>(obj);
                RegisterHandler<IUpdateHandler>(obj);
                RegisterHandler<ITriggerEnterHandler>(obj);
                RegisterHandler<ITriggerExitHandler>(obj);
                RegisterHandler<ITriggerStayHandler>(obj);
                RegisterHandler<ITriggerEnter2DHandler>(obj);
                RegisterHandler<ITriggerExit2DHandler>(obj);
                RegisterHandler<ITriggerStay2DHandler>(obj);
                RegisterHandler<ICollisionEnterHandler>(obj);
                RegisterHandler<ICollisionExitHandler>(obj);
                RegisterHandler<ICollisionStayHandler>(obj);
                RegisterHandler<ICollisionEnter2DHandler>(obj);
                RegisterHandler<ICollisionExit2DHandler>(obj);
                RegisterHandler<ICollisionStay2DHandler>(obj);
                RegisterHandler<IDrawGizmosHandler>(obj);
                RegisterHandler<IDestroyHandler>(obj);
            }
        }


        private void RegisterHandler<T>(object target) where T : IFiberHandler
        {
            if (target is T handler && !_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }


        private void OnEnable()                               => _handlers.ForEach(x => (x as IEnableHandler)?.OnEnable());
        private void OnDisable()                              => _handlers.ForEach(x => (x as IDisableHandler)?.OnDisable());
        private void OnMouseDown()                            => _handlers.ForEach(x => (x as IMouseDownHandler)?.OnMouseDown());
        private void OnMouseDrag()                            => _handlers.ForEach(x => (x as IMouseDragHandler)?.OnMouseDrag());
        private void OnMouseEnter()                           => _handlers.ForEach(x => (x as IMouseEnterHandler)?.OnMouseEnter());
        private void OnMouseExit()                            => _handlers.ForEach(x => (x as IMouseExitHandler)?.OnMouseExit());
        private void OnMouseOver()                            => _handlers.ForEach(x => (x as IMouseOverHandler)?.OnMouseOver());
        private void OnMouseUp()                              => _handlers.ForEach(x => (x as IMouseUpHandler)?.OnMouseUp());
        private void OnBecameInvisible()                      => _handlers.ForEach(x => (x as IBecameInvisibleHandler)?.OnBecameInvisible());
        private void OnBecameVisible()                        => _handlers.ForEach(x => (x as IBecameVisibleHandler)?.OnBecameVisible());
        private void OnTriggerEnter(Collider other)           => _handlers.ForEach(x => (x as ITriggerEnterHandler)?.OnTriggerEnter(other));
        private void OnTriggerExit(Collider other)            => _handlers.ForEach(x => (x as ITriggerExitHandler)?.OnTriggerExit(other));
        private void OnTriggerStay(Collider other)            => _handlers.ForEach(x => (x as ITriggerStayHandler)?.OnTriggerStay(other));
        private void OnTriggerEnter2D(Collider2D col)         => _handlers.ForEach(x => (x as ITriggerEnter2DHandler)?.OnTriggerEnter2D(col));
        private void OnTriggerExit2D(Collider2D other)        => _handlers.ForEach(x => (x as ITriggerExit2DHandler)?.OnTriggerExit2D(other));
        private void OnTriggerStay2D(Collider2D other)        => _handlers.ForEach(x => (x as ITriggerStay2DHandler)?.OnTriggerStay2D(other));
        private void OnCollisionEnter(Collision collision)    => _handlers.ForEach(x => (x as ICollisionEnterHandler)?.OnCollisionEnter(collision));
        private void OnCollisionExit(Collision other)         => _handlers.ForEach(x => (x as ICollisionExitHandler)?.OnCollisionExit(other));
        private void OnCollisionStay(Collision collisionInfo) => _handlers.ForEach(x => (x as ICollisionStayHandler)?.OnCollisionStay(collisionInfo));
        private void OnCollisionEnter2D(Collision2D col)      => _handlers.ForEach(x => (x as ICollisionEnter2DHandler)?.OnCollisionEnter2D(col));
        private void OnCollisionExit2D(Collision2D other)     => _handlers.ForEach(x => (x as ICollisionExit2DHandler)?.OnCollisionExit2D(other));
        private void OnCollisionStay2D(Collision2D collision) => _handlers.ForEach(x => (x as ICollisionStay2DHandler)?.OnCollisionStay2D(collision));

#if UNITY_EDITOR
        private void OnDrawGizmos() => _handlers.ForEach(x => (x as IDrawGizmosHandler)?.OnDrawGizmos());

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                StoreHandlers(_controller, _view);
            }
        }
#endif

        private void OnDestroy()
        {
            _handlers.ForEach(x => (x as IDestroyHandler)?.OnDestroy());
            _view?.Destroy();
        }

        public void Reset()
        {
            _controller     = default;
            _model          = default;
            _view           = default;
            _configurations = default;
            _handlers       = new List<IFiberHandler>();
        }
    }
}