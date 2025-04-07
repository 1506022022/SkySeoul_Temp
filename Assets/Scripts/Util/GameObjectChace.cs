using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace Util
{
    public static class GameObjectChace<T> where T : Object
    {
        private static readonly Dictionary<T, ObjectPool<T>> _pools = new();

        public static ObjectPool<T> GetPool(T origin)
        {
            _pools.TryAdd(origin, new(() => GameObject.Instantiate(origin),
                actionOnGet: (t) => t.GameObject().SetActive(true),
                actionOnRelease: (t) => t.GameObject().SetActive(false),
                actionOnDestroy: (t) => GameObject.Destroy(t)));
            return _pools[origin];
        }
        public static void ClearAll()
        {
            _pools.Keys
                      .ToList()
                      .ForEach(ClearPool);
        }
        public static void ClearPool(T origin)
        {
            GetPool(origin).Dispose();
            GetPool(origin).Clear();
            _pools.Remove(origin);
        }
    }
}