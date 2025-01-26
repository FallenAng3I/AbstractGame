using UnityEngine;

namespace PlayerSystem
{
    public class Headbob : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enable = true;
        [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
        [SerializeField, Range(0, 30)] private float frequency = 10.0f;
        [SerializeField] private Transform cam;
        [SerializeField] private Transform cameraHolder;

        private float _toggleSpeed = 3.0f;
        private Vector3 _startPos;
        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _startPos = cam.localPosition;
        }

        private void Update()
        {
            if (!enable) return;
            CheckMotion();
            ResetPosition();
            cam.LookAt(FocusTarget());
        }
        private Vector3 FootStepMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.x += Mathf.Sin(Time.time * frequency) * amplitude;
            pos.y += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
            return pos;
        }
        private void CheckMotion()
        {
            float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
            if (speed < _toggleSpeed) return;
            if (!_controller.isGrounded) return;

            PlayMotion(FootStepMotion());
        }
        private void PlayMotion(Vector3 motion)
        {
            cam.localPosition += motion;
        }

        private Vector3 FocusTarget()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
            pos += cameraHolder.forward * 15.0f;
            return pos;
        }
        private void ResetPosition()
        {
            if (cam.localPosition == _startPos) return;
            cam.localPosition = Vector3.Lerp(cam.localPosition, _startPos, 1 * Time.deltaTime);
        }
    }
}