using System;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Commons.Pools
{
    public class ComponentPool<T> : ObjectPool<T> where T : Component
    {
        private int _defaultCapacity;

        public ComponentPool(T prefab,
            Transform poolParent = null,
            IInstantiator instantiator = null,
            Action<T> actionOnCreate = null,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)

            : base(createFunc: () =>
                {
                    T obj = instantiator == null
                    ? UnityEngine.Object.Instantiate(prefab, poolParent)
                    : instantiator.InstantiatePrefabForComponent<T>(prefab, poolParent);

                    actionOnCreate?.Invoke(obj);
                    obj.gameObject.SetActive(false);
                    return obj;
                },
                actionOnGet: component =>
                {
                    component.gameObject.SetActive(true);
                    actionOnGet?.Invoke(component);
                },

                actionOnRelease: component =>
                {
                    actionOnRelease?.Invoke(component);
                    component.transform.SetParent(poolParent, false);
                    component.gameObject.SetActive(false);
                },

                actionOnDestroy: component =>
                {
                    actionOnDestroy?.Invoke(component);

                    if (component != null && component.gameObject != null)
                        UnityEngine.Object.Destroy(component.gameObject);
                },

                collectionCheck,
                defaultCapacity,
                maxSize)
        {
            _defaultCapacity = defaultCapacity;
        }

        public void Prewarm()
        {
            T[] tempArray = new T[_defaultCapacity];

            for (int i = 0; i < _defaultCapacity; i++)
                tempArray[i] = Get();

            for (int i = 0; i < _defaultCapacity; i++)
                Release(tempArray[i]);
        }
    }
}
