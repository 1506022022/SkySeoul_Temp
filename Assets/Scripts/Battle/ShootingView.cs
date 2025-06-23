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
        public float MouseSensitivity = 1.5f;
        public float VerticalRange;
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
            CloseAllCamera();
            SetCamera(CamType.Wide);
        }
        public void SetCamera(CamType type)
        {
            if (CamType == type) return;
            CamType = type;

            if (_currentView != null)
            {
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
            _currentView.LookAt = _body;
        }
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        public void UnLockMouse()
        {
            Cursor.lockState -= CursorLockMode.Locked;
        }

        public float distance = 5.0f;
        public float minDistance = 2.0f;
        public float maxDistance = 10.0f;
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;
        private float x = 0.0f;
        private float y = 0.0f;

        void Update()
        {
            x -= Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * MouseSensitivity;
            y += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * MouseSensitivity;
            distance -= Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -1, 1) * MouseSensitivity;

            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            x = Mathf.Clamp(x, -360f, 360f);
            x = x == -360f || x == 360f ? 0f : x;

            Debug.Log(x);
            float radianX = x * Mathf.Deg2Rad;
            float radianY = (y + 3) * Mathf.Deg2Rad;

            float posX = _body.position.x + distance * Mathf.Sin(radianY) * Mathf.Cos(radianX);
            float posY = _body.position.y + distance * Mathf.Cos(radianY);
            float posZ = _body.position.z + distance * Mathf.Sin(radianY) * Mathf.Sin(radianX);

            _currentView.transform.position = new Vector3(posX, posY, posZ);

            Vector3 dir = (_body.position - _currentView.transform.position).normalized;
            dir.y = 0;
            _body.forward = dir;
        }
        public void UpdateView()
        {
            Update();
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
