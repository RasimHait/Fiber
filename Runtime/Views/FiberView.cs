using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FiberFramework
{
    [Serializable]
    public abstract class FiberView
    {
        [SerializeReference]
        private MonoBehaviour _rootObject;
        protected MonoBehaviour root => _rootObject;
        internal  Action        OnDestroy;


        internal void Initialize(MonoBehaviour rootMonoBehaviour)
        {
            _rootObject = rootMonoBehaviour;
        }


        public void DontDestroyOnLoad()
        {
            Object.DontDestroyOnLoad(root.gameObject);
        }


        public void KeepDestroyOnLoad()
        {
            SceneManager.MoveGameObjectToScene(root.gameObject, SceneManager.GetActiveScene());
        }


        public void Destroy()
        {
            OnDestroy?.Invoke();
            Object.Destroy(_rootObject.gameObject);
        }
    }
}