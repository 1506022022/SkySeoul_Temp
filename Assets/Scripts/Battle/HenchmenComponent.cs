using UnityEngine;
using UnityEngine.AI;

namespace Battle
{
    public class HenchmenComponent : MonoBehaviour
    {
        private Henchmen _henchmen;
        [SerializeField] private Team Team;
        [SerializeField] private NavMeshAgent Agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private TriggerEventHandler _triggerEvent;
        private bool _attack;
        private void Awake()
        {
            Debug.Assert(Agent, $"{DM_WARNING.SERIALIZE_FIELD_EMPTY} {name}");
            if (Agent)
            {
                _henchmen = new(transform, Agent);
                _henchmen.Team = Team;
            }
            _triggerEvent.OnEnter.AddListener((s) => { _attack = true; });
            _triggerEvent.OnExit.AddListener((s) => { _attack = false; });
        }
        private void Update()
        {
            if (_attack)
            {
                _animator.SetTrigger("Attack");
            }
            else if (Agent.velocity.magnitude < 0.1f)
            {
                _animator.SetTrigger("Idle");
            }
            else
            {
                _animator.SetTrigger("Move");
            }
            if (_henchmen?.HasDependency ?? true)
            {
                return;
            }
            _henchmen.LookForJob();

        }
        private void OnDisable()
        {
            _henchmen?.LeaveJob();
        }
    }
}
