using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace TopDown
{
    public static class MapManager
    {
        public static readonly string mapDataPath = Path.Combine(Application.dataPath, "MapData.text");
        private static Dictionary<uint, MapData> maps;
        public static Dictionary<uint, MapData> Maps
        {
            get
            {
                if (maps == null) Initialize();
                return maps;
            }
        }
        private static List<uint> clearedMaps;
        public static List<uint> ClearedMaps
        {
            get
            {
                if (clearedMaps == null) Initialize();
                return clearedMaps;
            }
        }
        private static void Initialize()
        {
            maps = new();
            clearedMaps = new();
            var json = File.ReadAllText(MapManager.mapDataPath);
            var mapJson = JsonUtility.FromJson<MapJson>(json);
            if (!string.IsNullOrEmpty(json) && mapJson != null)
            {
                clearedMaps = mapJson.ClearedMaps;
                foreach (var item in mapJson.Maps)
                {
                    maps.Add(item.ID, item);
                }
            }
        }
    }

}
