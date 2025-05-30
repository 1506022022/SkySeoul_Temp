using UnityEngine;

namespace TopDown
{
    public enum MapType
    {
        None, Boot, Exit, Lobby, BattleMap, RailMap
    }
    [CreateAssetMenu(menuName = "DataSet/ModeSet", order = -1)]
    public class ModeSet : ScriptableObject
    {
        public MapType MapType;
        public uint MapID;
        public uint UserID;
        public uint PlayableCharacterID;
    }

}
