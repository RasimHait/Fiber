using System;

namespace FiberFramework
{
    public interface IFiberController
    {
        bool TryGetModel<T>(out T  model) where T : FiberModel;
        bool TryGetView<T>(out T  view) where T : FiberView;
        void Initialize(FiberModel model, FiberView view, FiberControllerConfigurations configurations);
        void SetReady();
        void Destroy();
    }
}