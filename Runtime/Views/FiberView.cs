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

        protected MonoBehaviour viewRoot   => _rootObject;
        public    GameObject    GameObject => _rootObject.gameObject;
        public    Transform     Transform  => _rootObject.transform;
        public    Action        OnDestroy;


        internal void Initialize(MonoBehaviour root)
        {
            _rootObject = root;
        }

        public void DontDestroyOnLoad()
        {
            Object.DontDestroyOnLoad(viewRoot.gameObject);
        }

        public void KeepDestroyOnLoad()
        {
            SceneManager.MoveGameObjectToScene(viewRoot.gameObject, SceneManager.GetActiveScene());
        }

        public void Destroy()
        {
            OnDestroy?.Invoke();
        }

    }
}