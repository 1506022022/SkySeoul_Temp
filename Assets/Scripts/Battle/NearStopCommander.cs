using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Battle.CommanderHelper;
namespace Battle
{
    public class NearStopCommander : NearCommander
    {
        public NearStopCommander(int width, int height, Vector3 pivot) : base(width, height, pivot)
        {
        }

        protected override void CalculateNextPosition(in FieldBase<byte> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex)
        {
            var me = currentIndex;
            var list = goTo.OrderBy(x => Vector2Int.Distance(x, me));
            foreach (var pos in list)
            {
                foreach (var intt in DIRS)
                {
                    if (IsOutOfRange(pos + intt, field.Height))
                    {
                        continue;
                    }

                    var nextX = pos.x + intt.x;
                    var nextY = pos.y + intt.y;
                    if (field.Array[nextX + nextY] != 0)
                    {
                        continue;
                    }

                    var temp = new Vector2Int(nextX, nextY);
                    if (temp == pos)
                    {
                        nextIndex = currentIndex;
                        return;
                    }

                    nextIndex = temp;
                    return;
                }
            }
            nextIndex = currentIndex;
        }


    }

}
