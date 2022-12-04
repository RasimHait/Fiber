using Object = UnityEngine.Object;

namespace FiberFramework
{
    public sealed class FiberController
    {
        private sealed class FiberControllerBehaviour<M, V> where M : FiberModel where V : FiberView
        {
            private IFiberController              _controller;
            private FiberControllerConfigurations _config;

            public M Model { get; private set; }
            public V View  { get; private set; }
            
            public void Initialize(IFiberController rootController, FiberControllerConfigurations configurations)
            {
                _controller = rootController;
                _config     = configurations;

                if (View != null)
                {
                    if (_config.DestroyOnLoad)
                    {
                        View.DontDestroyOnLoad();
                    }

                    View.OnDestroy += Destroy;
                }

                Fiber.Register(_controller);
            }

            public void SetModel<T>(T model) where T : FiberModel
            {
                Model = model as M;
            }

            public void SetView<T>(T view) where T : FiberView
            {
                View = view as V;
            }

            private void DestroySelf()
            {
                Fiber.UnRegister(_controller);
            }

            private void DestroyView()
            {
                if (View != default)
                {
                    View.OnDestroy -= Destroy;
                    View.Destroy();
                    Object.Destroy(View.GameObject);
                }
            }

            public void Destroy()
            {
                DestroySelf();
                DestroyView();
            }
        }


        public abstract class Default<TModel, TView> : IFiberController, IModelContainer, IViewContainer where TModel : FiberModel, new() where TView : FiberView, new()
        {
            private readonly FiberControllerBehaviour<TModel, TView> _coreController;

            public TModel Model => _coreController.Model;
            public TView  View  => _coreController.View;

            public Default()
            {
                _coreController = new FiberControllerBehaviour<TModel, TView>();
            }

            public void Initialize(FiberControllerConfigurations configurations)
            {
                _coreController.Initialize(this, configurations);
            }

            public void SetModel<T>(T model) where T : FiberModel
            {
                _coreController.SetModel(model);
            }

            public void SetView<T>(T view) where T : FiberView
            {
                _coreController.SetView(view);
            }

            public void Destroy()
            {
                _coreController.Destroy();
            }

            FiberModel IModelContainer.GetModel()
            {
                return Model;
            }

            FiberView IViewContainer.GetView()
            {
                return View;
            }
        }


        public abstract class NoView<TModel> : IFiberController, IModelContainer where TModel : FiberModel, new()
        {
            private readonly FiberControllerBehaviour<TModel, FiberView> _coreController;

            public TModel Model => _coreController.Model;

            public NoView()
            {
                _coreController = new FiberControllerBehaviour<TModel, FiberView>();
            }

            public void Initialize(FiberControllerConfigurations configurations)
            {
                _coreController.Initialize(this, configurations);
            }

            public void SetModel<T>(T model) where T : FiberModel
            {
                _coreController.SetModel(model);
            }

            public void Destroy()
            {
                _coreController.Destroy();
            }

            FiberModel IModelContainer.GetModel()
            {
                return Model;
            }
        }


        public abstract class NoModel<TView> : IFiberController, IViewContainer where TView : FiberView, new()
        {
            private readonly FiberControllerBehaviour<FiberModel, TView> _coreController;

            public TView View => _coreController.View;

            public NoModel()
            {
                _coreController = new FiberControllerBehaviour<FiberModel, TView>();
            }

            public void Initialize(FiberControllerConfigurations configurations)
            {
                _coreController.Initialize(this, configurations);
            }

            public void SetView<T>(T view) where T : FiberView
            {
                _coreController.SetView(view);
            }

            public void Destroy()
            {
                _coreController.Destroy();
            }

            FiberView IViewContainer.GetView()
            {
                return View;
            }
        }


        public abstract class Single : IFiberController
        {
            private readonly FiberControllerBehaviour<FiberModel, FiberView> _coreController;

            public Single()
            {
                _coreController = new FiberControllerBehaviour<FiberModel, FiberView>();
            }

            public void Initialize(FiberControllerConfigurations configurations)
            {
                _coreController.Initialize(this, configurations);
            }

            public void Destroy()
            {
                _coreController.Destroy();
            }
        }
    }
}