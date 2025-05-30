using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Battle.CommanderHelper;

namespace Battle
{

    public class NearCommander : Commander<byte>
    {
        protected static Vector2Int[] DIRS =
            {
        new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(-1, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(0, 1)
    };


        public NearCommander(int width, int height, Vector3 pivot) : base(width, height, pivot)
        {
        }

        public static int GetDirectionIndex(Vector2Int a, Vector2Int b)
        {
            Vector2Int direction = b - a;

            float minAngle = float.MaxValue;
            int closestDirectionIndex = -1;

            Vector2 directionVector = direction;

            for (int i = 0; i < DIRS.Length; i++)
            {
                Vector2 dir = DIRS[i];

                float dotProduct = Vector2.Dot(directionVector.normalized, dir.normalized);

                float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

                if (angle < minAngle)
                {
                    minAngle = angle;
                    closestDirectionIndex = i;
                }
            }

            return closestDirectionIndex;
        }


        protected override void GetGoToList(in FieldBase<byte> field, out List<Vector2Int> Index)
        {
            Index = new List<Vector2Int>();

            var player = GameObject.FindAnyObjectByType<ZoomCharacterComponent>();
            if (player != null)
            {
                var playerIndex = ConvertToInedx(player.transform.position, field);
                Index.Add(playerIndex);
                return;
            }

            Index.Add(new Vector2Int(0, 0));
            Index.Add(new Vector2Int(field.Height, field.Height));
            Index.Add(new Vector2Int(field.Height * 2, field.Height * 2));
        }

        protected override void CalculateNextPosition(in FieldBase<byte> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex)
        {
            var me = currentIndex;
            var list = goTo.OrderBy(x => Vector2Int.Distance(x, me));
            foreach (var item in list)
            {
                var pos = GetDirectionIndex(me, item);

                foreach (var intt in View180)
                {
                    var dir = DIRS[GetDirIndex(pos, intt)];
                    if (IsOutOfRange(new Vector3(currentIndex.x - field.Height + dir.x, 0, currentIndex.y - field.Height + dir.y), field.Height))
                    {
                        continue;
                    }
                    var ind = currentIndex.x + dir.x;
                    var inc = currentIndex.y + dir.y;
                    if (field.Array[ind + inc] != 0)
                    {
                        continue;
                    }
                    nextIndex = new Vector2Int(ind, inc);
                    return;
                }
            }
            nextIndex = currentIndex;
        }

        protected override void SetEmptyValue(out byte empty)
        {
            empty = 0;
        }

        protected override void SetGenerateTFunc(out Func<Henchmen, byte> generateFunc)
        {
            generateFunc = GenerateT;
        }

        private byte GenerateT(Henchmen henchmen)
        {
            return (byte)henchmen.Team;
        }
    }

}
