using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어의 이동, 점프, 카메라 회전, 등반, 인벤토리 등 주요 컨트롤을 담당하는 클래스
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed; // 이동 속도
    public float JumpForce; // 점프 힘
    private Vector2 _curMovementInput; // 현재 이동 입력값
    private Rigidbody _rigidbody; // 플레이어의 Rigidbody
    [HideInInspector]
    public bool IsFast = false; // 일시적으로 빠른 상태 여부

    [SerializeField] private LayerMask groundLayerMask; // 바닥 판정 레이어
    [Header("Look")]
    public Transform cameraContainer; // 카메라 컨테이너(회전용)
    public float minXLook; // 카메라 X축 최소 각도
    public float maxXLook; // 카메라 X축 최대 각도
    private float camCurXRot; // 카메라 현재 X축 회전값
    public float lookSensitivity = 1f; // 마우스 감도
    private Vector2 mouseDelta; // 마우스 이동값
    private PlayerCondition _condition; // 플레이어 상태(체력, 스태미나 등)
    public bool canLook = true; // 카메라 회전 가능 여부

    public Action inventory; // 인벤토리 열기 액션

    [Header("Stamina usage")]
    [SerializeField] private float runStaminaUsage; // 달리기 시 소모 스태미나
    [SerializeField] private float jumpStaminaUsage; // 점프 시 소모 스태미나

    private bool isRunning = false; // 달리기 상태
    [Header("Climbing")]
    public LayerMask climbableLayerMask; // 등반 가능 레이어
    private Camera _camera; // 메인 카메라 참조
    [SerializeField] private Transform climbCheckPosition; // 등반 판정 위치
    private bool _isExhaustedWhileClimb = false; // 등반 중 탈진 상태

    //Launch
    private bool isLaunching = false; // 플랫폼 점프 중 상태

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 캐싱
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
        _condition = GetComponent<PlayerCondition>(); // PlayerCondition 컴포넌트 캐싱
        _camera = Camera.main; // 메인 카메라 캐싱
    }
    private void FixedUpdate()
    {
        Move(); // 이동 처리
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook(); // 카메라 회전 처리
        }
    }
    // 플레이어 이동 처리
    private void Move()
    {
        if (isLaunching)
        {
            return;
        }
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
                StartCoroutine(ExhaustedWhileClimb()); // 등반 중 탈진 코루틴 시작
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

    // 등반 중 탈진 코루틴
    private IEnumerator ExhaustedWhileClimb()
    {
        _isExhaustedWhileClimb = true;
        _rigidbody.useGravity = true;
        yield return new WaitForSeconds(5.0f);
        _isExhaustedWhileClimb = false;
    }
    // 등반 가능 여부 판정
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
    // 카메라 회전 처리
    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    // 이동 입력 처리
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
    // 마우스 입력 처리
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    // 점프 입력 처리
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
    // 바닥에 닿아있는지 판정
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
    // 인벤토리 입력 처리
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }
    // 커서 잠금/해제 및 카메라 회전 가능 여부 토글
    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    // 버섯 슈퍼 점프(특수 점프)
    public void MushroomSuperJump(float forceMultiplier)
    {
        _rigidbody.AddForce(Vector3.up * JumpForce * forceMultiplier, ForceMode.Impulse);
    }
    // 달리기 입력 처리
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
    // 플랫폼 점프(특수 이동) 처리
    public void LaunchPlatformJump(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(LaunchCoroutine());
    }
    // 플랫폼 점프 후 착지까지 상태 관리 코루틴
    private IEnumerator LaunchCoroutine()
    {
        isLaunching = true;
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (IsGrounded())
            {
                isLaunching = false;
                yield break;
            }
            yield return null;
        }
    }
    // 일시적으로 빠른 상태 토글
    public void ToggleIsFast(float duration)
    {
        StartCoroutine(SetIsFast(duration));
    }
    // 빠른 상태 지속 시간 관리 코루틴
    private IEnumerator SetIsFast(float duration)
    {
        IsFast = true;
        yield return new WaitForSeconds(duration);
        IsFast = false;
    }
}