using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class MyLoader<T> where T : class
    {
        private readonly Dictionary<string, GameObject> Instances = new();

        public T Load(string path, string name)
        {
            if (!Instances.TryGetValue(path, out var instance) || instance == null)
            {
                var prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.Log($"{DM_ERROR.NOT_FOUND} : {path}");
                    return null;
                }

                instance = GameObject.Instantiate(prefab);
                instance.name = instance.name.Replace("(Clone)", "");
                Instances.TryAdd(path, instance);
            }

            T result = null;
            if (instance.name == name) result = instance.transform.GetComponent<T>();
            if (result == null) result = instance.transform.Find(name)?.GetComponent<T>();
            if (result == null) Debug.Log($"{DM_ERROR.NOT_FOUND} : {path} at in child.");
            return result;
        }

        public void Unload()
        {
            foreach (var instance in Instances.Values)
            {
                if (instance == null) continue;
                GameObject.Destroy(instance);
            }
        }
    }
}
