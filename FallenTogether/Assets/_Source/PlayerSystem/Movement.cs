using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class Movement : MonoBehaviour
    {
        [Header("Movement settings")]
        [SerializeField] private float speed = 3.0f;
        [SerializeField] private float sprintMultiplier = 2.0f;
    
        [Header("Jump settings")]
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = 9.81f;

        [Header("Sensitivity settings")]
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float upDownRange = 88.0f;
    
        [Header("Input Actions")]
        [SerializeField] private InputActionAsset playerControls;

        private bool _isMoving;
        private Camera mainCamera;
        private float verticalRotation;
        private Vector3 currentMovement = Vector3.zero;
        private Vector3 jumpDirection = Vector3.zero;
        private CharacterController characterController;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private Vector2 moveInput;
        private Vector2 lookInput;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            moveAction = playerControls.FindActionMap("Player").FindAction("Move");
            lookAction = playerControls.FindActionMap("Player").FindAction("Look");
            jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");
            sprintAction = playerControls.FindActionMap("Player").FindAction("Sprint");

            moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
            moveAction.canceled += context => moveInput = Vector2.zero;
        
            lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
            lookAction.canceled += context => lookInput = Vector2.zero;
        }   

        private void OnEnable()
        {
            moveAction.Enable();
            lookAction.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            lookAction.Disable();
            jumpAction.Disable();
            sprintAction.Disable();
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

            if (characterController.isGrounded)
            {
                float verticalSpeed = moveInput.y * speed * speedMultiplier;
                float horizontalSpeed = moveInput.x * speed * speedMultiplier;

                Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
                horizontalMovement = transform.rotation * horizontalMovement;

                jumpDirection = horizontalMovement;

                currentMovement.x = horizontalMovement.x;
                currentMovement.z = horizontalMovement.z;
            }
            else
            {
                currentMovement.x = jumpDirection.x;
                currentMovement.z = jumpDirection.z;
            }

            HandleGravityAndJumping();
            characterController.Move(currentMovement * Time.deltaTime);

            _isMoving = moveInput.y != 0 || moveInput.x != 0;
        }

        private void HandleGravityAndJumping()
        {
            if (characterController.isGrounded)
            {
                currentMovement.y = -0.5f;

                if (jumpAction.triggered)
                {
                    currentMovement.y = jumpForce;
                }
            }
            else
            {
                currentMovement.y -= gravity * Time.deltaTime;
            }
        }

        private void HandleRotation()
        {
            float mouseXRotation = lookInput.x * mouseSensitivity;
            transform.Rotate(0, mouseXRotation, 0);

            verticalRotation -= lookInput.y * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
            mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }
}
