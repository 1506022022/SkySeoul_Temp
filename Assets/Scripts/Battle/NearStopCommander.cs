using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Battle.CommanderHelper;
namespace Battle
{
    public class NearStopCommander : NearCommander
    {
        public NearStopCommander(int radius,Vector3 pivot) : base(radius, pivot)
        {
        }

        protected override void CalculateNextPosition(in Field<byte> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex)
        {
            var me = currentIndex;
            var list = goTo.OrderBy(x => Vector2Int.Distance(x, me));
            foreach (var item in list)
            {
                var pos = item;//GetDirectionIndex(me, item);

                foreach (var intt in DIRS)//View180)
                {
                    //var dir = DIRS[GetDirIndex(pos, intt)];
                    //if (IsOutOfRange(currentIndex, field.Radius))
                    if (IsOutOfRange(pos + intt, field.Radius))
                    {
                        continue;
                    }

                    var nextX = pos.x + intt.x;//currentIndex.x + dir.x;
                    var nextY = pos.y + intt.y;// currentIndex.y + dir.y;
                    if (field.Array[nextX, nextY] != 0)
                    {
                        continue;
                    }

                    var temp = new Vector2Int(nextX, nextY);
                    if (temp == item)
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
