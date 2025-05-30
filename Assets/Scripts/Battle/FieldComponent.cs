using UnityEngine;
namespace Battle
{
    public class FieldComponent : MonoBehaviour
    {
        public float _interval = 3;
        public float _t = 0;
        private enum CommanderType { Near, Random, NearStop };
        [SerializeField] private CommanderType type;
        private void ChangeCommander(CommanderType type)
        {
            _commander?.Dispose();
            _commander = type switch
            {
                CommanderType.Near => new NearCommander(Width,Height, pivot),
                CommanderType.Random => new RandomCommander(Width, Height, pivot),
                CommanderType.NearStop => new NearStopCommander(Width, Height, pivot),
                _ => new NearCommander(Width, Height, pivot)
            };
        }
        private CommanderType _preType;
        private void Update()
        {
            if (_preType != type)
            {
                ChangeCommander(type);
                _preType = type;
            }
            _t += Time.deltaTime;
            if (_interval < _t)
            {
                _t = 0;
                UpdateCommand();
            }
        }

        [Min(1)] public int Width = 1;
        [Min(1)] public int Height = 1;
        [Min(1)] public int CellSize = 1;
        private Commander<byte> _commander;

        public void UpdateCommand()
        {
            _commander.UpdateCommand();
        }
        void Awake()
        {
            Initialize();
            ChangeCommander(type);
        }
        Vector3 cell;
        Vector3 pivot;
        void Initialize()
        {
            cell = (Vector3.right + Vector3.forward) * CellSize + Vector3.up * 0.2f;
            pivot = transform.position
                          + new Vector3(cell.x / 2f, cell.y / 2, cell.z / 2f);
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector3 offset = new Vector3(x * CellSize, 0, z * CellSize);
                    Vector3 cellCenter = pivot + offset;
                    Gizmos.DrawWireCube(cellCenter, cell);
                }
            }
        }
    }

}
