using System;
using System.Collections.Generic;


namespace TopDown
{
    [Serializable]
    public class MapJson
    {
        public List<MapData> Maps = new();
        public List<uint> ClearedMaps = new();
    }

}
