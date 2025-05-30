using UnityEngine;

namespace Effect
{
    public class SkinEffectComponent : MonoBehaviour
    {
        private SkinEffect skinEffect;
        [SerializeField] private Renderer[] renderers;

        private void Awake()
        {
            skinEffect = new SkinEffect(renderers);
        }
        public void SetEffect(IEffectCartridge effect)
        {
            skinEffect.SetEffect(effect);
        }
        [ContextMenu("Play")]
        public void Play()
        {
            skinEffect.Play();
        }
    }
}