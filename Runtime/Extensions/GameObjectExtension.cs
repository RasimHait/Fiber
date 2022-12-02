using UnityEngine;

namespace FiberFramework
{
    public static class GameObjectExtension
    {
        public static bool IsFiberObject(this GameObject gameObject, out FiberObject fiberObject)
        {
            return gameObject.TryGetComponent(out fiberObject);
        }
        

        public static FiberObject AsFiberObject(this GameObject gameObject)
        {
            return gameObject.GetComponent<FiberObject>();
        }
    }
}