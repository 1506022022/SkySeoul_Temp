using Unity.Cinemachine;
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
        private readonly CinemachineCamera _wideCam;
        private readonly CinemachineCamera _zoomCam;
        private CinemachineCamera _currentView;
        public ShootingView(Transform body, CinemachineCamera wide, CinemachineCamera zoom)
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
        public float sensitivity = 5f;      // ���콺 ����
        public Vector3 offset = new Vector3(0, 1.7f, -5);  // �ʱ� �Ÿ�, ����
        public float smoothSpeed = 10f;     // ī�޶� �̵� �ε巯��
        private float yaw = 0f;
        private float pitch = 10f;
        void Update()
        {

            // ���콺 �Է�
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, -20f, 60f);  // ���� ȸ�� ����

            // ȸ�� ���
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            // ��ġ ���
            Vector3 desiredPosition = _body.position + rotation * offset.normalized * distance;

            // �ε巯�� �̵�
            _currentView.transform.position = desiredPosition;
            _currentView.transform.LookAt(_body.position + Vector3.up * 1.5f);  // ĳ���� �Ӹ� �� ����

            var forward = _currentView.transform.forward;
            forward.y = 0;
            _body.forward = forward;
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
