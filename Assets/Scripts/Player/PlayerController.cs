using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;
    public float JumpForce;
    private Vector2 _curMovementInput;
    private Rigidbody _rigidbody;
    [HideInInspector]
    public bool IsFast = false;

    [SerializeField] private LayerMask groundLayerMask;
    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity = 1f;
    private Vector2 mouseDelta;
    private PlayerCondition _condition;
    public bool canLook = true;

    public Action inventory;

    [Header("Stamina usage")]
    [SerializeField] private float runStaminaUsage;
    [SerializeField] private float jumpStaminaUsage;

    private bool isRunning = false;
    [Header("Climbing")]
    public LayerMask climbableLayerMask;
    private Camera _camera;
    [SerializeField] private Transform climbCheckPosition;
    private bool _isExhaustedWhileClimb = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _condition = GetComponent<PlayerCondition>();
        _camera = Camera.main;
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }
    private void Move()
    {
        if (IsClimbing())
        {
            if (_isExhaustedWhileClimb)
            {
                // 등반 중 탈진 상태면 중력만 적용하고 아무것도 하지 않음
                _rigidbody.useGravity = true;
                return;
            }

            if (_condition.UseStamina(runStaminaUsage))
            {
                _rigidbody.useGravity = false;
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.AddForce(Vector3.up * MoveSpeed * _curMovementInput.y * 0.5f, ForceMode.VelocityChange);
            }
            else
            {
                StartCoroutine(ExhaustedWhileClimb());
            }
            return;
        }
        else
        {
            _rigidbody.useGravity = true;
            _isExhaustedWhileClimb = false; // 등반이 끝나면 탈진 상태 해제
        }
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= MoveSpeed;
        if (isRunning)
        {
            if (_condition.UseStamina(runStaminaUsage))
            {
                dir *= 2f;
            }
            else
            {
                isRunning = false;
            }
        }
        if (IsFast)
        {
            dir *= 1.5f;
        }
        dir.y = _rigidbody.velocity.y;
        _rigidbody.velocity = dir;
    }

    private IEnumerator ExhaustedWhileClimb()
    {
        _isExhaustedWhileClimb = true;
        _rigidbody.useGravity = true;
        yield return new WaitForSeconds(5.0f);
        _isExhaustedWhileClimb = false;
    }
    private bool IsClimbing()
    {
        Ray ray = new Ray(climbCheckPosition.position, _camera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.3f, climbableLayerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if ((context.phase == InputActionPhase.Canceled))
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            if (!_condition.UseStamina(jumpStaminaUsage))
            {
                return;
            }
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            _condition.UseStamina(jumpStaminaUsage);
        }
    }
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward*0.2f) + (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (-transform.forward*0.2f) + (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (transform.right*0.2f) + (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (-transform.right*0.2f) + (transform.up*0.01f),Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    public void MushroomSuperJump()
    {
        _rigidbody.AddForce(Vector3.up * JumpForce * 10, ForceMode.Impulse);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isRunning = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }
    public void ToggleIsFast(float duration)
    {
        StartCoroutine(SetIsFast(duration));
    }
    private IEnumerator SetIsFast(float duration)
    {
        IsFast = true;
        yield return new WaitForSeconds(duration);
        IsFast = false;
    }
}