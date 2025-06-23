using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public static class GameModeFactory
    {
        public static IGameMode CreateMode(MapType type)
        {
            return type switch
            {
                MapType.Lobby => new LobbyMode(),
                MapType.BattleMap => new BattleMode(),
                _ => null
            };
        }
    }
    public static class LoaderFactory
    {
        static readonly Dictionary<string, GameObject> Instances = new();

        public static T Load<T>(string path, string name) where T : class
        {
            if (!Instances.TryGetValue(path, out var instance))
            {
                var prefab = Resources.Load<GameObject>(path);

                if (prefab == null)
                {
                    Debug.Log($"{DM_ERROR.NOT_FOUND} : {path}");
                    return null;
                }

                instance = GameObject.Instantiate(prefab);
                instance.name = instance.name.Replace("(Clone)", "");
                Instances.Add(path, instance);
                GameObject.DontDestroyOnLoad(instance);
            }


            T result = null;
            if (instance.name == name) result = instance.transform.GetComponent<T>();
            if (result == null) result = instance.transform.Find(name)?.GetComponent<T>();
            if (result == null) Debug.Log($"{DM_ERROR.NOT_FOUND} : {path} at in child.");
            return result;
        }
        public static void Unload()
        {
            foreach (var obj in Instances.Values)
            {
                GameObject.Destroy(obj);
            }
            Instances.Clear();
        }
    }
    public static class ControllerManagerFactory
    {
        public static IControllerManager CreateControllerManager(MapType type)
        {
            return type switch
            {
                _ => null
            };
        }
    }
}