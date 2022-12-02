using UnityEngine;

namespace FiberFramework
{
    public sealed class FiberController
    {
        public abstract class Default<TModel, TView> : IFiberController where TModel : FiberModel where TView : FiberView
        {
            public TModel                        Model  { get; private set; }
            public TView                         View   { get; private set; }
            public FiberControllerConfigurations Config { get; private set; }


            public void Initialize(FiberModel targetModel, FiberView targetView, FiberControllerConfigurations configurations)
            {
                Model  = (TModel)targetModel;
                View   = (TView)targetView;
                Config = configurations;

                if (View != null)
                {
                    if (Config.DestroyOnLoad)
                    {
                        View.DontDestroyOnLoad();
                    }

                    View.OnDestroy += Destroy;
                }

                Fiber.Register(this);
                OnInitialized();
            }

            public void SetReady()
            {
                OnReady();
            }

            private void DestroySelf()
            {
                OnDestroy();
                Fiber.UnRegister(this);
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

            bool IFiberController.TryGetModel<T>(out T model)
            {
                model = Model as T;
                return Model is T;
            }

            bool IFiberController.TryGetView<T>(out T view)
            {
                view = View as T;
                return View is T;
            }

            protected abstract void OnInitialized();

            protected abstract void OnReady();

            protected abstract void OnDestroy();
        }

        public abstract class NoView<TModel> : Default<TModel, FiberView> where TModel : FiberModel
        {
        }

        public abstract class NoModel<TView> : Default<FiberModel, TView> where TView : FiberView
        {
        }

        public abstract class Single : Default<FiberModel, FiberView>
        {
        }
    }
}