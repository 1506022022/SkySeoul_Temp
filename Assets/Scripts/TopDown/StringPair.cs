using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    [Serializable]
    public struct String2
    {
        public string Key;
        public string Value;
    }

    [CreateAssetMenu(menuName ="Data/StringPair")]
    public class StringPair : ScriptableObject
    {
        [SerializeField] List<String2> data = new();
        private Dictionary<string, string> map;
        public Dictionary<string, string> Map
        {
            get
            {
                if (map == null)
                {
                    map = new();
                    for (int i = 0; i < data.Count; i++)
                    {
                        var key = data[i].Key;
                        var value = data[i].Value;
                        map.Add(key, value);
                    }
                }
                return map;
            }
        }
    }

}
