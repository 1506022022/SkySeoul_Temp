using Cinemachine;
using TMPro;
using UnityEngine;

namespace Battle
{
    public class CharacterComponent : MonoBehaviour
    {
        private Character _character;
        private CharacterJoycon _usedJoycon;
        private CharacterJoycon _zoomInJoycon;
        private CharacterJoycon _zoomOutJoycon;
        private CharacterAnimator _characterAnimator;
        private CharacterAnimator _zoomInAnim;
        private CharacterAnimator _zoomOutAnim;
        private CharacterMovement _characterMovement;
        private HandIK _handIK;
        private ShootingView _view;
        [Header("References")]
        public bool _owner;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterController _controller;
        [Header("View")]
        [SerializeField, Range(0, 180f)] private float _verticalRange;
        [SerializeField, Range(0, 1000)] private float _mouseSensitivity = 500;
        [SerializeField] private CinemachineVirtualCamera _wideCam;
        [SerializeField] private CinemachineVirtualCamera _zoomInCam;
        [Header("Battle")]
        public float ArmLength;
        public float MoveSpeed = 3f;
        public float JumpPower = 5f;
        public float SlidPower = 3f;
        [SerializeField] private WeaponComponent _weapon;
        [SerializeField] private HitBoxComponent _hitBox;
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _ui;

        private void Awake()
        {
            _character = new();

            _zoomInAnim = new HanZoomInAnimator(_character, _animator);
            _zoomOutAnim = new HanZoomOutAnimator(_character, _animator);
            _characterAnimator = _zoomOutAnim;
            _characterAnimator.Use();

            _hitBox.HitBox.OnCollision += (h) => _character.DoHit();
            _weapon.SetOwner(_character, actor: transform);

            _view = new(body: transform, _wideCam, _zoomInCam)
            {
                VerticalRange = _verticalRange,
                MouseSensitivity = _mouseSensitivity
            };
            _view.SetActive(_owner);

            if (!_owner)
            {
                return;
            }

            _zoomInJoycon = new HanZoomInJoycon(_character, _controller);
            _zoomOutJoycon = new HanZoomOutJoycon(_character, _controller);
            _usedJoycon = _zoomOutJoycon;

            _characterMovement = new CharacterMovement(_character, _controller)
            {
                JumpPower = JumpPower,
                MoveSpeed = MoveSpeed,
                SlidPower = SlidPower
            };

            DoZoomOut();
        }
        private void LateUpdate()
        {
            if (!_owner) return;
            UpdateJoycon();
            UpdateZoomIn();
            UpdateView();
        }
        private void UpdateKeyLookForward()
        {
            if (!Input.GetKeyDown(KeyCode.LeftControl))
            {
                return;
            }
            _controller.transform.rotation = _animator.transform.rotation;
            _animator.transform.localRotation = Quaternion.identity;
        }
        private void FixedUpdate()
        {
            _ui?.SetText(_character.BodyState.ToString());

            if (!_owner) return;
            _characterMovement.UpdateGravity();
        }
        private void UpdateJoycon()
        {
            _usedJoycon?.UpdateInput();
        }
        private void UpdateView()
        {
            if (FlagHelper.HasFlag(_character.BodyState, BodyState.Jump)) return;
            _view.UpdateView();
        }
        private void UpdateZoomIn()
        {
            if (_view.CamType is not CamType.Zoom && Input.GetKey(KeyCode.Mouse1)) DoZoomIn();
            else
            if (_view.CamType is CamType.Zoom && !Input.GetKey(KeyCode.Mouse1)) DoZoomOut();
        }
        private void DoZoomOut()
        {
            _view.SetCamera(CamType.Wide);
            _usedJoycon = _zoomOutJoycon;
            _characterAnimator.Unuse();
            _characterAnimator = _zoomOutAnim;
            _characterAnimator.Use();
        }
        private void DoZoomIn()
        {
            _view.SetCamera(CamType.Zoom);
            _usedJoycon = _zoomInJoycon;
            _characterAnimator.Unuse();
            _characterAnimator = _zoomInAnim;
            _characterAnimator.Use();
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_characterMovement != null)
            {
                _characterMovement.SlidPower = SlidPower;
                _characterMovement.JumpPower = JumpPower;
                _characterMovement.MoveSpeed = MoveSpeed;
            }
            if (_view != null)
            {
                _view.VerticalRange = _verticalRange;
                _view.MouseSensitivity = _mouseSensitivity;
            }
            if (_handIK != null)
            {
                _handIK.ARM_LENGTH = ArmLength;
            }
        }
#endif
    }
}
