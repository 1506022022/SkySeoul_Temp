using Unity.Cinemachine;
using UnityEngine;

namespace Battle
{
    public class ZoomCharacterComponent : CharacterComponent, IPlayable
    {
        private ShootingView view;
        [Header("View")]
        [SerializeField, Range(0, 180f)] private float verticalRange;
        [SerializeField, Range(0, 1000)] private float mouseSensitivity = 500;
        [SerializeField] private CinemachineCamera wideCam;
        [SerializeField] private CinemachineCamera zoomInCam;
        public float SlidePower = 3f;

        CharacterMovement movement;
        public override void Initialize()
        {
            base.Initialize();
            view = new ShootingView(transform, wideCam, zoomInCam);
            SetAnimator(new HanZoomOutAnimator());
            SetController(new HanZoomOutJoycon(this));
            movement = new CharacterMovement(character, transform)
            {
                SlidePower = this.SlidePower
            };
            SetMovement(movement);

        }
        public override void Dispose()
        {
            base.Dispose();
            view = null;
        }
        public void OnZoomOut()
        {
            view?.SetCamera(CamType.Wide);
        }
        public void OnZoomIn()
        {
            view?.SetCamera(CamType.Zoom);
        }
        void LateUpdate()
        {
            view?.UpdateView();
        }
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (view != null)
            {
                view.VerticalRange = verticalRange;
                view.MouseSensitivity = mouseSensitivity;
            }

            if (movement != null)
            {
                movement.SlidePower = this.SlidePower;
            }
        }
    }
}