using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace Effect
{
    public interface IEffectCartridge
    {
        public void Execute();
        public void SetMaterials(Material[] materials);
    }
    public abstract class CoroutineEffect : IEffectCartridge
    {
        private readonly HashSet<IEnumerator> coroutines = new();
        private Material[] materials;
        public void SetMaterials(Material[] materials)
        {
            this.materials = materials;
        }
        public void Execute()
        {
            if (coroutines.Count != 0)
            {
                Debug.LogWarning(DM_WARNING.BUSY);
                return;
            }
            Enurmerator.InvokeFor(materials,
                (m) => CoroutineRunner.instance.StartCoroutine(PlayEffectRoutineSafe(m)));
        }
        private IEnumerator PlayEffectRoutineSafe(Material material)
        {
            var routine = PlayEffectRoutine(material);
            coroutines.Add(routine);

            while (material != null && routine.MoveNext())
            {
                yield return routine.Current;
            }
            coroutines.Remove(routine);
        }
        protected abstract IEnumerator PlayEffectRoutine(Material material);
    }
    public class DebugEffect : CoroutineEffect
    {
        public static readonly DebugEffect Instance = new DebugEffect(Color.clear);
        public DebugEffect this[string message]
        {
            get
            {
                return message switch
                {
                    nameof(NULL) => NULL,
                    _ => DEFUALT
                };
            }
        }
        private DebugEffect(Color color) { this.color = color; }
        private Color color;
        private static readonly DebugEffect NULL = new DebugEffect(Color.yellow);
        private static readonly DebugEffect DEFUALT = new DebugEffect(Color.red);
        protected override IEnumerator PlayEffectRoutine(Material material)
        {
            Color originalColor = material.color;
            material.color = color;
            yield return new WaitForSeconds(1f);
            material.color = originalColor;
        }
    }
    public class SkinEffect
    {
        private Material[] skins;
        private IEffectCartridge cartridge = DebugEffect.Instance["DEFUALT"];

        public SkinEffect(Renderer[] renderers)
        {
            SetSkins(renderers);
        }
        public void SetSkins(Renderer[] renderers)
        {
            skins = renderers
                .SelectMany(r => r.materials)
                .ToArray();
            cartridge.SetMaterials(skins);
        }
        public void SetEffect(IEffectCartridge cartridge)
        {
            this.cartridge = cartridge;
            cartridge.SetMaterials(skins);
        }
        public void Play()
        {
            cartridge.Execute();
        }
    }
}