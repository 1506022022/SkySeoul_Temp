using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Battle.FlagHelper;
namespace Battle
{
    [Flags]
    internal enum BodyState
    {
        None = 0,
        Walk = 1 << 0,
        Grounded = 1 << 1,
        Crouch = 1 << 2,
        Weapon = 1 << 3,
        Hit = 1 << 4,
        Attack = 1 << 5,
        Guard = 1 << 6,
        Down = 1 << 7,
        Interaction = 1 << 8,
        Cancel = 1 << 9,
        Land = 1 << 10,
        Idle = 1 << 11,
        Slide = 1 << 12,
        Jump = 1 << 13,
        StandUp = 1 << 14
    }
    public class Character
    {
        public event Action OnIdle;
        public event Action OnCalmDown;
        public event Action<Vector3> OnMove;
        public event Action<Vector3> OnRun;
        public event Action OnPreJump;
        public event Action OnJump;
        public event Action OnLand;
        public event Action OnPreAttack;
        public event Action OnAttack;
        public event Action OnEndAttack;
        public event Action OnHit;
        public event Action OnCancel;
        public event Action OnFall;
        public event Action OnSlide;
        public event Action OnSlideEnd;
        public event Action OnInteraction;
        public event Action OnStandup;
        internal BodyState BodyState;
        private bool _cantAction => _cantMove |
            HasFlag(BodyState, BodyState.Attack);
        private bool _cantMove => HasFlag(BodyState,
            BodyState.Hit |
            BodyState.Cancel |
            BodyState.Land |
            BodyState.Interaction) |
            !HasFlag(BodyState, BodyState.Grounded);
        private float _lastUpdateTime;

        public Character()
        {
            OnSlide += UpdateTime;
            OnJump += UpdateTime;
            OnLand += UpdateTime;
            OnAttack += UpdateTime;
            OnHit += UpdateTime;
            OnCancel += UpdateTime;
            OnFall += UpdateTime;
            CoroutineRunner.instance.StartCoroutine(UpdateIdle());
        }
        private void UpdateTime()
        {
            _lastUpdateTime = Time.time;
        }
        public bool DoMove(Vector3 dir)
        {
            if (HasFlag(BodyState, BodyState.Interaction))
            {
                DoCancel();
                return false;
            }
            else
            if (_cantMove || HasFlag(BodyState, BodyState.Slide))
            {
                return false;
            }

            AddFlag(ref BodyState, BodyState.Walk);
            OnMove?.Invoke(dir);
            return true;
        }
        public bool DoRun(Vector3 dir)
        {
            if (_cantMove || HasFlag(BodyState, BodyState.Slide))
            {
                return false;
            }

            if (HasFlag(BodyState, BodyState.Interaction))
            {
                DoCancel();
                return false;
            }

            AddFlag(ref BodyState, BodyState.Walk);
            OnRun?.Invoke(dir);
            return true;
        }
        public bool DoJump()
        {
            if (_cantAction || HasFlag(BodyState, BodyState.Slide) || !HasFlag(BodyState, BodyState.Grounded))
            {
                return false;
            }

            AddFlag(ref BodyState, BodyState.Jump);
            RemoveFlag(ref BodyState, BodyState.Grounded | BodyState.Idle);
            OnJump?.Invoke();
            return true;
        }
        public bool DoLand()
        {
            if (HasFlag(BodyState, BodyState.Grounded))
            {
                return false;
            }

            RemoveFlag(ref BodyState, BodyState.Jump);
            AddFlag(ref BodyState, BodyState.Grounded | BodyState.Land);
            OnLand?.Invoke();
            return true;
        }
        public bool DoFall()
        {
            if (!HasFlag(BodyState, BodyState.Grounded))
            {
                return false;
            }

            RemoveFlag(ref BodyState, BodyState.Grounded | BodyState.Idle);
            DoCancel();
            OnFall?.Invoke();
            return true;
        }
        public bool DoAttack()
        {
            if (_cantAction)
            {
                return false;
            }

            AddFlag(ref BodyState, BodyState.Attack);
            RemoveFlag(ref BodyState, BodyState.Idle);
            OnAttack?.Invoke();
            return true;
        }
        public bool DoInteraction()
        {
            if (_cantAction || HasFlag(BodyState, BodyState.Slide))
            {
                return false;
            }

            AddFlag(ref BodyState, BodyState.Interaction);
            RemoveFlag(ref BodyState, BodyState.Idle);
            OnInteraction?.Invoke();
            return true;
        }
        public bool CalmDown()
        {
            if (!_cantAction)
            {
                return false;
            }

            RemoveFlag(ref BodyState, BodyState.Attack |
                                      BodyState.Guard |
                                      BodyState.Land |
                                      BodyState.Cancel |
                                      BodyState.StandUp |
                                      BodyState.Hit);

            OnCalmDown?.Invoke();
            return true;
        }
        public bool DoStandUp()
        {
            if (!HasFlag(BodyState, BodyState.Grounded))
            {
                return false;
            }

            if (HasFlag(BodyState, BodyState.Slide))
            {
                RemoveFlag(ref BodyState, BodyState.Slide);
                OnSlideEnd?.Invoke();
            }
            else
            if (HasFlag(BodyState, BodyState.Interaction))
            {
                RemoveFlag(ref BodyState, BodyState.Interaction);
                // AddFlag(ref _bodyState, BodyState.StandUp);
                OnStandup?.Invoke();
            }
            return true;
        }
        public bool DoCancel()
        {
            if (HasFlag(BodyState, BodyState.Cancel))
            {
                return false;
            }
            if (!HasFlag(BodyState, BodyState.Attack | BodyState.Interaction | BodyState.Guard))
            {
                return false;
            }

            AddFlag(ref BodyState, BodyState.Cancel);
            RemoveFlag(ref BodyState, BodyState.Idle);
            OnCancel?.Invoke();
            return true;
        }
        public bool DoHit()
        {
            AddFlag(ref BodyState, BodyState.Hit);
            OnHit?.Invoke();
            return true;
        }
        public bool DoSlide()
        {
            if (EqualsFlag(BodyState, (BodyState.Grounded | BodyState.Walk | BodyState.Idle)))
            {
                AddFlag(ref BodyState, BodyState.Slide);
                RemoveFlag(ref BodyState, BodyState.Walk | BodyState.Idle);
                OnSlide?.Invoke();
                return true;
            }
            return false;
        }
        private IEnumerator UpdateIdle()
        {
            var duration = 1.5f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (HasFlag(BodyState, BodyState.Idle | BodyState.Slide))
                {
                }
                else
                if (duration < Time.time - _lastUpdateTime)
                {
                    AddFlag(ref BodyState, BodyState.Idle);
                    RemoveFlag(ref BodyState, BodyState.Walk);
                    OnIdle?.Invoke();
                }
            }
        }

    }
}

