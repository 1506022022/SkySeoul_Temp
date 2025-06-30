using Character;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Battle
{
    public class FieldComponent : MonoBehaviour, IInitializable, IDisposable, IActor
    {
        float t = 0;
        public float interval = 3;
        public List<IEnemy> enemys = new();

        void Update()
        {
            t += Time.deltaTime;
            if (interval <= t)
            {
                t = 0;
                var player = FindAnyObjectByType<PlayableBaseComponent>();
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (player == null) break;
                    //enemys[i].henchmen.MoveTo(player.transform.position);
                }
            }
        }

        public void Add(MonsterComponent enemy)
        {
            enemys.Add(enemy);
        }
        public void Remove(MonsterComponent enemy)
        {
            enemys.Remove(enemy);
        }

        [Min(1)] public Vector3Int Size = Vector3Int.one;
        float CellSize = 1f;

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

            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] is ITransform tActor) tActor.transform.gameObject.SetActive(true);
                if (enemys[i] is IInitializable initializable) initializable.Initialize();
            }
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

        public void Dispose()
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] is ITransform tActor) tActor.transform.gameObject.SetActive(false);
                if (enemys[i] is IDisposable disposable) disposable.Dispose();
            }
        }
    }

}
