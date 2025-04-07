using Cinemachine;
using UnityEngine;

namespace Battle
{
    public enum CamType
    {
        Default = 0,
        Wide = 1,
        Zoom = 2
    }
    public class ShootingView
    {
        public CamType CamType { get; private set; }
        public float MouseSensitivity = 500f;
        public float VerticalRange;
        private Vector3 _baseFocus;
        private Transform _follow;
        private readonly Transform _body;
        private readonly CinemachineVirtualCamera _wideCam;
        private readonly CinemachineVirtualCamera _zoomCam;
        private CinemachineVirtualCamera _currentView;
        public ShootingView(Transform body, CinemachineVirtualCamera wide, CinemachineVirtualCamera zoom)
        {
            _body = body;
            _wideCam = wide;
            _zoomCam = zoom;

            _wideCam.Priority = 10;
            _zoomCam.Priority = 11;
            LockMouse();
            CloseAllCamera();
            SetCamera(CamType.Wide);
        }
        public void SetCamera(CamType type)
        {
            if (CamType == type) return;
            CamType = type;

            if (_currentView != null)
            {
                _currentView.Follow.localPosition = _baseFocus;
                _currentView.gameObject.SetActive(false);
            }

            _currentView = type switch
            {
                CamType.Default => _wideCam,
                CamType.Wide => _wideCam,
                CamType.Zoom => _zoomCam,
                _ => _wideCam
            };
            _currentView.gameObject.SetActive(true);
            _baseFocus = _currentView.Follow?.transform.localPosition ?? Vector3.zero;
            _follow = _currentView.Follow;
            _follow.parent = null;
        }
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        public void UnLockMouse()
        {
            Cursor.lockState -= CursorLockMode.Locked;
        }
        public void UpdateView()
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            _body.Rotate(Vector3.up * mouseX);

            if (_follow == null) return;
            float mouseY = Input.mousePosition.y / Screen.height - 0.5f;
            mouseY *= VerticalRange;
            var position = _body.transform.position + _baseFocus;
            position.y = _baseFocus.y + -mouseY;
            _follow.position = position;
            _follow.rotation = _body.transform.rotation;
        }
        public void SetActive(bool active)
        {
            _currentView?.gameObject.SetActive(active);
        }
        private void CloseAllCamera()
        {
            _wideCam.gameObject.SetActive(false);
            _zoomCam.gameObject.SetActive(false);
        }
    }
}
