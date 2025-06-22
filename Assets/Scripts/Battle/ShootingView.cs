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
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            x += mouseX * MouseSensitivity;
            y -= mouseY * MouseSensitivity;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            distance -= scroll * MouseSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // ȸ�� ���ʹϿ� ���� (Euler �� Quaternion)
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // ��ġ ��� (ĳ���� �ֺ� �� �˵� �� ��ġ)
            Vector3 offset = rotation * new Vector3(0, 0, -distance);
            _currentView.transform.position = _body.position + offset;

            // ī�޶�� ĳ���� �ٶ󺸵���
            _currentView.transform.LookAt(_body.position + Vector3.up * 1.5f);

            // ĳ���ʹ� ī�޶� �ٶ󺸴� �������� ȸ���ϵ�, ����
            Vector3 direction = (_currentView.transform.position - _body.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(-direction);
                _body.rotation = Quaternion.Slerp(_body.rotation, targetRot, Time.deltaTime * 10f);
            }
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
