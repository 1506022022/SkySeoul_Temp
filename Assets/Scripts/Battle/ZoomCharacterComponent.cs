using Cinemachine;
using UnityEngine;

namespace Battle
{
    public class ZoomCharacterComponent : CharacterComponent 
    {
        private ShootingView view;
        [Header("View")]
        [SerializeField, Range(0, 180f)] private float verticalRange;
        [SerializeField, Range(0, 1000)] private float mouseSensitivity = 500;
        [SerializeField] private CinemachineVirtualCamera wideCam;
        [SerializeField] private CinemachineVirtualCamera zoomInCam;
        public override void Initialize()
        {
            base.Initialize();
            view = new ShootingView(transform,wideCam,zoomInCam);
        }
        public void OnZoomOut()
        {
            view.SetCamera(CamType.Wide);
        }
        public void OnZoomIn()
        {
            view.SetCamera(CamType.Zoom);
        }
        void LateUpdate()
        {
            view.UpdateView();
        }
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (view != null)
            {
                view.VerticalRange = verticalRange;
                view.MouseSensitivity = mouseSensitivity;
            }
        }
    }
}