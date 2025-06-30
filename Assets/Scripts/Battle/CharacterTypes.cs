using UnityEngine;

namespace Battle
{
    public interface IEntity { }
    public interface IActor { }
    public interface ITransform { Transform transform { get; } }
    public interface IPlayable { }
    public interface IEnemy { }
    public interface IProp { }
}