using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Battle.CommanderHelper;

namespace Battle
{
    public abstract class Commander<T> : IDisposable
    {
        private readonly T _EMPTY;
        private readonly FieldBase<T> _field;
        private readonly List<Henchmen> henchmens;
        private readonly Func<Henchmen, T> _generateElement;

        protected abstract void CalculateNextPosition(in FieldBase<T> field, in List<Vector2Int> goTo, Vector2Int currentIndex, out Vector2Int nextIndex);
        protected abstract void SetGenerateTFunc(out Func<Henchmen, T> generateFunc);
        protected abstract void SetEmptyValue(out T empty);
        protected abstract void GetGoToList(in FieldBase<T> field, out List<Vector2Int> Index);

        public Commander(int width, int height, Vector3 pivot)
        {
            _field = new FieldBase<T>(width, height, pivot);
            henchmens = new List<Henchmen>();

            SetGenerateTFunc(out _generateElement);
            SetEmptyValue(out _EMPTY);
        }

        ~Commander()
        {
            FreeHenchmenAll();
        }

        public void UpdateCommand()
        {
            SetField();
            GiveCommand();
        }

        public void AddHenchmen(Henchmen henchmen)
        {
            if (henchmen.HasDependency) return;
            henchmens.Add(henchmen);
            henchmen.HasDependency = true;
        }

        public void FreeHenchmen(Henchmen henchmen)
        {
            if (!henchmens.Contains(henchmen)) return;
            henchmens.Remove(henchmen);
            henchmen.HasDependency = false;
        }

        private void FreeHenchmenAll()
        {
            foreach (var henchmen in henchmens.ToList())
            {
                FreeHenchmen(henchmen);
            }
        }

        private void SetField()
        {
            _field.Reset();
            foreach (var henchmen in henchmens.ToList())
            {
                if (IsOutOfRange(henchmen.Position - _field.Pivot, _field.Height))
                {
                    FreeHenchmen(henchmen);
                    continue;
                }

                var index = ConvertToInedx(henchmen.Position, _field);
                _field.Array[index.y + index.x] = _generateElement(henchmen);
            }
        }

        private void GiveCommand()
        {
            henchmens.OrderBy(x => ConvertToInedx(x.Position, _field));
            GetGoToList(in _field, out var goTo);

            foreach (var henchmen in henchmens)
            {
                if (henchmen.Team is not Team.Monster) continue;

                var currentIndex = ConvertToInedx(henchmen.Position, _field);
                CalculateNextPosition(in _field, in goTo, currentIndex, out var nextIndex);

                _field.Array[currentIndex.y + currentIndex.x] = _EMPTY;
                _field.Array[nextIndex.y + nextIndex.x] = _generateElement(henchmen);

                CommandMove(henchmen, ConvertToPosition(nextIndex, _field));
            }
        }
        public void Dispose()
        {
            FreeHenchmenAll();
        }

    }

}
