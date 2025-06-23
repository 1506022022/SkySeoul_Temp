using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class FieldComponent : MonoBehaviour
    {
        public float interval = 3;
        float t = 0;
        enum CommanderType { Near, Random, NearStop };
        [SerializeField] private CommanderType type = CommanderType.Random;
        public List<MonsterComponent> enemys = new();

        public void Add(MonsterComponent enemy)
        {
            enemys.Add(enemy);
        }
        public void Remove(MonsterComponent enemy)
        {
            enemys.Remove(enemy);
            commander.FreeHenchmen(enemy.henchmen);
        }

        void ChangeCommander(CommanderType type)
        {
            commander?.Dispose();
            commander = type switch
            {
                CommanderType.Near => new NearCommander(Size.x, Size.z, pivot),
                CommanderType.Random => new RandomCommander(Size.x, Size.z, pivot),
                CommanderType.NearStop => new NearStopCommander(Size.x, Size.z, pivot),
                _ => new NearStopCommander(Size.x, Size.z, pivot)
            };
            for (int i = 0; i < enemys.Count; i++)
            {
                commander.AddHenchmen(enemys[i].henchmen);
            }
        }
        CommanderType _preType;
        void Update()
        {
            if (_preType != type)
            {
                ChangeCommander(type);
                _preType = type;
            }
            t += Time.deltaTime;
            if (interval < t)
            {
                t = 0;
                UpdateCommand();
            }
        }

        [Min(1)] public Vector3Int Size = Vector3Int.one;
        Commander<byte> commander;
        float CellSize = 1f;
        public void UpdateCommand()
        {
            commander?.UpdateCommand();
        }
        void Awake()
        {
            Initialize();
        }
        Vector3 cell;
        Vector3 pivot;
        public void Initialize()
        {
            cell = (Vector3.right + Vector3.forward) * CellSize + Vector3.up * 0.2f;
            pivot = transform.position
                          + new Vector3(cell.x / 2f, cell.y / 2, cell.z / 2f);

            ChangeCommander(type);
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int z = 0; z < Size.z; z++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    Vector3 offset = new Vector3(x * CellSize, 0, z * CellSize);
                    Vector3 cellCenter = pivot + offset;
                    Gizmos.DrawWireCube(cellCenter, cell);
                }
            }
        }
    }

}
