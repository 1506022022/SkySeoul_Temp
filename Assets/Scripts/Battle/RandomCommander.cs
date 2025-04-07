using System;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class RandomCommander : Commander<byte>
    {
        public RandomCommander(int radius, Vector3 pivot) : base(radius, pivot)
        {
        }

        protected override void GetGoToList(in Field<byte> field, out List<Vector2Int> Index)
        {
            Index = new List<Vector2Int>()
        {
            new(0, 0),
            new(field.Radius, field.Radius),
            new(field.Radius * 2, field.Radius * 2)
        };

        }

        protected override void CalculateNextPosition(in Field<byte> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex)
        {
            if (field.Radius < 1)
            {
                nextIndex = currentIndex;
                Debug.LogError($"{DM_ERROR.OUT_OF_RANGE} range : {field.Radius}");
                return;
            }

            var x = UnityEngine.Random.Range(0, field.Radius);
            var y = UnityEngine.Random.Range(0, field.Radius);
            nextIndex = new(x, y);
        }

        protected override void SetEmptyValue(out byte empty)
        {
            empty = 0;
        }

        protected override void SetGenerateTFunc(out Func<Henchmen, byte> generateFunc)
        {
            generateFunc = (x) => 1;
        }

    }

}
