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

            StoreHandlers(_controller, _view);
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
            return _controller.TryGetModel(out result);
        }


        public bool TryGetView<T>(out T result) where T : FiberView
        {
            return _controller.TryGetView(out result);
        }


        public void RefreshModel()
        {
            if (_controller != null)
            {
                var modelType = _controller.GetType().GetProperty("Model")?.PropertyType;

                if (modelType?.GetConstructor(Type.EmptyTypes) != null)
                {
                    _model = (FiberModel)Activator.CreateInstance(modelType);
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
                    _view = (FiberView)Activator.CreateInstance(viewType);

                    _view.Construct(this);

                    StoreHandlers(_view);
                }
            }
        }


        public void RefreshConfigurations()
        {
            if (_controller != null)
            {
                _configurations = (FiberControllerConfigurations)Activator.CreateInstance(typeof(FiberControllerConfigurations));
            }
        }


        private void Awake()
        {
            if (_controller != null)
            {
                _view.Initialize(this);


                _model?.OnInitialize();
                _controller?.Initialize(_model, _view, _configurations);

                if (_view != null)
                {
                    StoreHandlers(_controller, _view);
                    return;
                }

                StoreHandlers(_controller);
            }
        }


        private void Start()
        {
            _controller?.SetReady();
        }


        private void StoreHandlers(params object[] objects)
        {
            _handlers = new List<IFiberHandler>();

            foreach (var obj in objects)
            {
                if (obj == null) continue;

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


        private void OnEnable()                                    => _handlers.ForEach(x => (x as IEnableHandler)?.OnEnable());
        private void OnDisable()                                   => _handlers.ForEach(x => (x as IDisableHandler)?.OnDisable());
        private void OnMouseDown()                                 => _handlers.ForEach(x => (x as IMouseDownHandler)?.OnMouseDown());
        private void OnMouseDrag()                                 => _handlers.ForEach(x => (x as IMouseDragHandler)?.OnMouseDrag());
        private void OnMouseEnter()                                => _handlers.ForEach(x => (x as IMouseEnterHandler)?.OnMouseEnter());
        private void OnMouseExit()                                 => _handlers.ForEach(x => (x as IMouseExitHandler)?.OnMouseExit());
        private void OnMouseOver()                                 => _handlers.ForEach(x => (x as IMouseOverHandler)?.OnMouseOver());
        private void OnMouseUp()                                   => _handlers.ForEach(x => (x as IMouseUpHandler)?.OnMouseUp());
        private void OnBecameInvisible()                           => _handlers.ForEach(x => (x as IBecameInvisibleHandler)?.OnBecameInvisible());
        private void OnBecameVisible()                             => _handlers.ForEach(x => (x as IBecameVisibleHandler)?.OnBecameVisible());
        private void OnTriggerEnter(Collider        other)         => _handlers.ForEach(x => (x as ITriggerEnterHandler)?.OnTriggerEnter(other));
        private void OnTriggerExit(Collider         other)         => _handlers.ForEach(x => (x as ITriggerExitHandler)?.OnTriggerExit(other));
        private void OnTriggerStay(Collider         other)         => _handlers.ForEach(x => (x as ITriggerStayHandler)?.OnTriggerStay(other));
        private void OnTriggerEnter2D(Collider2D    col)           => _handlers.ForEach(x => (x as ITriggerEnter2DHandler)?.OnTriggerEnter2D(col));
        private void OnTriggerExit2D(Collider2D     other)         => _handlers.ForEach(x => (x as ITriggerExit2DHandler)?.OnTriggerExit2D(other));
        private void OnTriggerStay2D(Collider2D     other)         => _handlers.ForEach(x => (x as ITriggerStay2DHandler)?.OnTriggerStay2D(other));
        private void OnCollisionEnter(Collision     collision)     => _handlers.ForEach(x => (x as ICollisionEnterHandler)?.OnCollisionEnter(collision));
        private void OnCollisionExit(Collision      other)         => _handlers.ForEach(x => (x as ICollisionExitHandler)?.OnCollisionExit(other));
        private void OnCollisionStay(Collision      collisionInfo) => _handlers.ForEach(x => (x as ICollisionStayHandler)?.OnCollisionStay(collisionInfo));
        private void OnCollisionEnter2D(Collision2D col)           => _handlers.ForEach(x => (x as ICollisionEnter2DHandler)?.OnCollisionEnter2D(col));
        private void OnCollisionExit2D(Collision2D  other)         => _handlers.ForEach(x => (x as ICollisionExit2DHandler)?.OnCollisionExit2D(other));
        private void OnCollisionStay2D(Collision2D  collision)     => _handlers.ForEach(x => (x as ICollisionStay2DHandler)?.OnCollisionStay2D(collision));

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