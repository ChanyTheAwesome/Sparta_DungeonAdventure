using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// �÷��̾��� �̵�, ����, ī�޶� ȸ��, ���, �κ��丮 �� �ֿ� ��Ʈ���� ����ϴ� Ŭ����
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed; // �̵� �ӵ�
    public float JumpForce; // ���� ��
    private Vector2 _curMovementInput; // ���� �̵� �Է°�
    private Rigidbody _rigidbody; // �÷��̾��� Rigidbody
    [HideInInspector]
    public bool IsFast = false; // �Ͻ������� ���� ���� ����

    [SerializeField] private LayerMask groundLayerMask; // �ٴ� ���� ���̾�
    [Header("Look")]
    public Transform cameraContainer; // ī�޶� �����̳�(ȸ����)
    public float minXLook; // ī�޶� X�� �ּ� ����
    public float maxXLook; // ī�޶� X�� �ִ� ����
    private float camCurXRot; // ī�޶� ���� X�� ȸ����
    public float lookSensitivity = 1f; // ���콺 ����
    private Vector2 mouseDelta; // ���콺 �̵���
    private PlayerCondition _condition; // �÷��̾� ����(ü��, ���¹̳� ��)
    public bool canLook = true; // ī�޶� ȸ�� ���� ����

    public Action inventory; // �κ��丮 ���� �׼�

    [Header("Stamina usage")]
    [SerializeField] private float runStaminaUsage; // �޸��� �� �Ҹ� ���¹̳�
    [SerializeField] private float jumpStaminaUsage; // ���� �� �Ҹ� ���¹̳�

    private bool isRunning = false; // �޸��� ����
    [Header("Climbing")]
    public LayerMask climbableLayerMask; // ��� ���� ���̾�
    private Camera _camera; // ���� ī�޶� ����
    [SerializeField] private Transform climbCheckPosition; // ��� ���� ��ġ
    private bool _isExhaustedWhileClimb = false; // ��� �� Ż�� ����

    //Launch
    private bool isLaunching = false; // �÷��� ���� �� ����

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ ĳ��
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
        _condition = GetComponent<PlayerCondition>(); // PlayerCondition ������Ʈ ĳ��
        _camera = Camera.main; // ���� ī�޶� ĳ��
    }
    private void FixedUpdate()
    {
        Move(); // �̵� ó��
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook(); // ī�޶� ȸ�� ó��
        }
    }
    // �÷��̾� �̵� ó��
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
                // ��� �� Ż�� ���¸� �߷¸� �����ϰ� �ƹ��͵� ���� ����
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
                StartCoroutine(ExhaustedWhileClimb()); // ��� �� Ż�� �ڷ�ƾ ����
            }
            return;
        }
        else
        {
            _rigidbody.useGravity = true;
            _isExhaustedWhileClimb = false; // ����� ������ Ż�� ���� ����
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

    // ��� �� Ż�� �ڷ�ƾ
    private IEnumerator ExhaustedWhileClimb()
    {
        _isExhaustedWhileClimb = true;
        _rigidbody.useGravity = true;
        yield return new WaitForSeconds(5.0f);
        _isExhaustedWhileClimb = false;
    }
    // ��� ���� ���� ����
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
    // ī�޶� ȸ�� ó��
    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    // �̵� �Է� ó��
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
    // ���콺 �Է� ó��
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    // ���� �Է� ó��
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
    // �ٴڿ� ����ִ��� ����
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
    // �κ��丮 �Է� ó��
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }
    // Ŀ�� ���/���� �� ī�޶� ȸ�� ���� ���� ���
    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    // ���� ���� ����(Ư�� ����)
    public void MushroomSuperJump(float forceMultiplier)
    {
        _rigidbody.AddForce(Vector3.up * JumpForce * forceMultiplier, ForceMode.Impulse);
    }
    // �޸��� �Է� ó��
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
    // �÷��� ����(Ư�� �̵�) ó��
    public void LaunchPlatformJump(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(LaunchCoroutine());
    }
    // �÷��� ���� �� �������� ���� ���� �ڷ�ƾ
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
    // �Ͻ������� ���� ���� ���
    public void ToggleIsFast(float duration)
    {
        StartCoroutine(SetIsFast(duration));
    }
    // ���� ���� ���� �ð� ���� �ڷ�ƾ
    private IEnumerator SetIsFast(float duration)
    {
        IsFast = true;
        yield return new WaitForSeconds(duration);
        IsFast = false;
    }
}