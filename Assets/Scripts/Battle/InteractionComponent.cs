using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{

    public class InteractionComponent : MonoBehaviour
    {
        private Interaction _interaction;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private UnityEvent _onInteraction;
        private void Awake()
        {
            SetInteraction();
        }
        private void SetInteraction()
        {
            Bounds bounds;
            if (_boxCollider != null)
            {
                bounds = _boxCollider.bounds;
            }
            else
            {
                Debug.LogWarning($"{DM_WARNING.SERIALIZE_FIELD_EMPTY} : {name}");
                bounds = new Bounds()
                {
                    center = transform.position,
                    size = Vector3.one
                };
            }
            _interaction = new Interaction(_boxCollider.bounds);
            _interaction.OnStart += _onInteraction.Invoke;

             
        }

        private void OnDestroy()
        {
            _interaction.OnDestroy();
        }
    }
}
