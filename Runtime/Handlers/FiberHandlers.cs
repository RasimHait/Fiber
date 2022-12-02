using UnityEngine;

namespace FiberFramework
{
    public interface IFiberHandler
    {
    }

    /// <summary> Executes on Bridge </summary>
    public interface IUpdateHandler : IFiberHandler
    {
        void Update();
    }

    /// <summary> Executes on Bridge </summary>
    public interface IFixedUpdateHandler : IFiberHandler
    {
        void FixedUpdate();
    }

    /// <summary> Executes on Bridge </summary>
    public interface ILateUpdateHandler : IFiberHandler
    {
        void LateUpdate();
    }

    /// <summary> Executes on View </summary>
    public interface IEnableHandler : IFiberHandler
    {
        void OnEnable();
    }

    /// <summary> Executes on View </summary>
    public interface IDisableHandler : IFiberHandler
    {
        void OnDisable();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseDownHandler : IFiberHandler
    {
        void OnMouseDown();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseDragHandler : IFiberHandler
    {
        void OnMouseDrag();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseEnterHandler : IFiberHandler
    {
        void OnMouseEnter();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseExitHandler : IFiberHandler
    {
        void OnMouseExit();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseOverHandler : IFiberHandler
    {
        void OnMouseOver();
    }

    /// <summary> Executes on View </summary>
    public interface IMouseUpHandler : IFiberHandler
    {
        void OnMouseUp();
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerEnterHandler : IFiberHandler
    {
        void OnTriggerEnter(Collider trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerExitHandler : IFiberHandler
    {
        void OnTriggerExit(Collider trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerStayHandler : IFiberHandler
    {
        void OnTriggerStay(Collider trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerEnter2DHandler : IFiberHandler
    {
        void OnTriggerEnter2D(Collider2D trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerExit2DHandler : IFiberHandler
    {
        void OnTriggerExit2D(Collider2D trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ITriggerStay2DHandler : IFiberHandler
    {
        void OnTriggerStay2D(Collider2D trigger);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionEnterHandler : IFiberHandler
    {
        void OnCollisionEnter(Collision collision);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionExitHandler : IFiberHandler
    {
        void OnCollisionExit(Collision collision);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionStayHandler : IFiberHandler
    {
        void OnCollisionStay(Collision collision);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionEnter2DHandler : IFiberHandler
    {
        void OnCollisionEnter2D(Collision2D collision);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionExit2DHandler : IFiberHandler
    {
        void OnCollisionExit2D(Collision2D collision);
    }

    /// <summary> Executes on View </summary>
    public interface ICollisionStay2DHandler : IFiberHandler
    {
        void OnCollisionStay2D(Collision2D collision);
    }

    /// <summary> Executes on View </summary>
    public interface IBecameVisibleHandler : IFiberHandler
    {
        void OnBecameVisible();
    }

    /// <summary> Executes on View </summary>
    public interface IBecameInvisibleHandler : IFiberHandler
    {
        void OnBecameInvisible();
    }

    /// <summary> Executes on View </summary>
    public interface IDrawGizmosHandler : IFiberHandler
    {
        void OnDrawGizmos();
    }

    /// <summary> Executes on View </summary>
    public interface IDestroyHandler : IFiberHandler
    {
        void OnDestroy();
    }
}