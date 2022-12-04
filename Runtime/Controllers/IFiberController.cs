using System;

namespace FiberFramework
{
    public interface IFiberController
    {
        void Initialize(FiberControllerConfigurations configurations);
        void Destroy();
    }

    internal interface IModelContainer
    {
        FiberModel GetModel();
        void       SetModel<T>(T model) where T : FiberModel;
    }

    internal interface IViewContainer
    {
        FiberView GetView();
        void      SetView<T>(T view) where T : FiberView;
    }
}