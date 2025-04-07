using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;
using static Battle.CommanderHelper;

namespace Battle
{
    public abstract class Commander<T> : IDisposable
    {
        private readonly T _EMPTY;
        private readonly Field<T> _field;
        private readonly List<Henchmen> _henchmens;
        private readonly Func<Henchmen, T> _generateElement;

        protected abstract void CalculateNextPosition(in Field<T> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex);
        protected abstract void SetGenerateTFunc(out Func<Henchmen, T> generateFunc);
        protected abstract void SetEmptyValue(out T empty);
        protected abstract void GetGoToList(in Field<T> field, out List<Vector2Int> Index);

        public Commander(int radius, Vector3 pivot)
        {
            _field = new Field<T>(radius, pivot);
            _henchmens = new List<Henchmen>();

            Henchmen.OnSpawnEvent += AddHenchmen;
            Henchmen.OnDestroyEvent += FreeHenchmen;

            SetGenerateTFunc(out _generateElement);
            SetEmptyValue(out _EMPTY);
        }

        ~Commander()
        {
            Henchmen.OnSpawnEvent -= AddHenchmen;
            Henchmen.OnDestroyEvent -= FreeHenchmen;

            FreeHenchmenAll();
        }

        public void UpdateCommand()
        {
            SetField();
            GiveCommand();
        }

        private void AddHenchmen(Henchmen henchmen)
        {
            if (henchmen.HasDependency)
            {
                return;
            }

            _henchmens.Add(henchmen);
            henchmen.HasDependency = true;
        }

        private void FreeHenchmen(Henchmen henchmen)
        {
            if (!_henchmens.Contains(henchmen))
            {
                return;
            }

            _henchmens.Remove(henchmen);
            henchmen.HasDependency = false;
        }

        private void FreeHenchmenAll()
        {
            foreach (var henchmen in _henchmens.ToList())
            {
                FreeHenchmen(henchmen);
            }
        }

        private void SetField()
        {
            _field.Reset();
            foreach (var henchmen in _henchmens.ToList())
            {
                if (IsOutOfRange(henchmen.Position - _field.Pivot, _field.Radius))
                {
                    FreeHenchmen(henchmen);
                    continue;
                }

                var index = ConvertToInedx(henchmen.Position-_field.Pivot, _field.Radius);
                _field.Array[index.y, index.x] = _generateElement(henchmen);
            }
        }

        List<GameObject> PlayerPoint = new();
        List<GameObject> MonsterPoint = new();
        private void GiveCommand()
        {
            _henchmens.OrderBy(x => ConvertToInedx(x.Position-_field.Pivot, _field.Radius));
            GetGoToList(in _field, out var goTo);
            //
            foreach (var a in PlayerPoint)
            {
                GameObjectChace<GameObject>.GetPool(Resources.Load<GameObject>("Player Point")).Release(a);
            }
            PlayerPoint.Clear();
            foreach (var a in MonsterPoint)
            {
                GameObjectChace<GameObject>.GetPool(Resources.Load<GameObject>("Monster Point")).Release(a);
            }
            MonsterPoint.Clear();
            foreach (var target in goTo)
            {
                var arrow = GameObjectChace<GameObject>.GetPool(Resources.Load<GameObject>("Player Point")).Get();
                arrow.transform.position = ConvertToPosition(target, _field.Radius, 1.5f) + _field.Pivot;
                PlayerPoint.Add(arrow);
            }
            //

            foreach (var henchmen in _henchmens)
            {
                var currentIndex = ConvertToInedx(henchmen.Position - _field.Pivot, _field.Radius);
                CalculateNextPosition(in _field, in goTo, currentIndex, out var nextIndex);

                _field.Array[currentIndex.y, currentIndex.x] = _EMPTY;
                _field.Array[nextIndex.y, nextIndex.x] = _generateElement(henchmen);

                CommandMove(henchmen, ConvertToPosition(nextIndex, _field.Radius, henchmen.Position.y) + _field.Pivot);
                // 
                if (henchmen.Team is not Team.Monster)
                {
                    continue;
                }
                var arrow = GameObjectChace<GameObject>.GetPool(Resources.Load<GameObject>("Monster Point")).Get();
                arrow.transform.position = ConvertToPosition(nextIndex, _field.Radius, 1.5f) + _field.Pivot;
                MonsterPoint.Add(arrow);
                //
            }
        }
        public void Dispose()
        {
            Henchmen.OnSpawnEvent -= AddHenchmen;
            Henchmen.OnDestroyEvent -= FreeHenchmen;

            FreeHenchmenAll();
        }

    }

}
