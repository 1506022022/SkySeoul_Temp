using UnityEngine;
namespace Battle
{
    public static class CommanderHelper
    {
        public static readonly Vector2Int[] DIRS =
        {
        new(0, -1), new(1, -1), new(-1, -1),
        new(1, 0), new(-1, 0),
        new(1, 1), new(-1, 1), new(0, 1)
    };

        public static readonly int[] View180 = { 0, -1, 1, -2, 2 };

        public static void CommandMove(Henchmen henchmen, Vector3 position)
        {
            position.y = henchmen.Position.y;
            henchmen.MoveTo(position);
        }

        public static bool IsOutOfRange(Vector3 position, int radius)
        {
            return radius < Mathf.Abs(position.x) ||
                radius < Mathf.Abs(position.z);
        }

        public static bool IsOutOfRange(Vector2Int index, int radius)
        {
            return radius * 2 < index.x ||
                radius * 2 < index.y ||
                index.x < 0 ||
                index.y < 0;
        }

        public static Vector2Int ConvertToInedx<T>(Vector3 position, FieldBase<T> field)
        {
            Vector2Int index = new((int)(position.x - field.Pivot.x), (int)(position.z - field.Pivot.z));
            return index;
        }

        public static Vector3 ConvertToPosition<T>(Vector2Int index, FieldBase<T> field)
        {
            return field.Pivot + new Vector3(index.x, 0, index.y);
        }

        public static int GetDirIndex(int a, int b)
        {
            var index = (a + b) % 8;
            var dir = index < 0 ? 8 + index : index;
            return dir;
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

    }
}

