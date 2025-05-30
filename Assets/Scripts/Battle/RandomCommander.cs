using System;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class RandomCommander : Commander<byte>
    {
        public RandomCommander(int width, int height, Vector3 pivot) : base(width, height, pivot)
        {
        }

        protected override void GetGoToList(in FieldBase<byte> field, out List<Vector2Int> Index)
        {
            Index = new List<Vector2Int>()
        {
            new(0, 0),
            new(field.Height, field.Height),
            new(field.Height * 2, field.Height * 2)
        };

        }

        protected override void CalculateNextPosition(in FieldBase<byte> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex)
        {
            if (field.Height < 1)
            {
                nextIndex = currentIndex;
                Debug.LogError($"{DM_ERROR.OUT_OF_RANGE} range : {field.Height}");
                return;
            }

            var x = UnityEngine.Random.Range(0, field.Width);
            var y = UnityEngine.Random.Range(0, field.Height);
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
