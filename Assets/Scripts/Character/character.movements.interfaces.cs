using UnityEngine;
namespace Character
{
    public interface IMove
    {
        void SetActor(Transform transform);
    }
    public interface IStrength
    {
        public void SetStrength(float strength);
        public float GetStrength();
    }
    public interface IDirection
    {
        public void SetDirection(Vector3 direction);

    }
}