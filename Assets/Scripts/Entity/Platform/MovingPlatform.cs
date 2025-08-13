using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����̴� �÷����� �����ϴ� Ŭ����, ���� �������̽� ����
public class MovingPlatform : MonoBehaviour, IRewardable
{
    [SerializeField] private Transform _pointA; // �̵� ���� ����
    [SerializeField] private Transform _pointB; // �̵� �� ����
    [SerializeField] private GameObject _platform; // ���� �̵��� �÷��� ������Ʈ

    [SerializeField] private float speed = 1.0f; // �̵� �ӵ�

    private bool _isMovingToB; // B �������� �̵� ������ ����

    private void Start()
    {
        // ���� �� �÷��� ��Ȱ��ȭ
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        MovePlatform(); // �÷��� �̵� ó��
        SetDirection(); // �̵� ���� ����
    }

    // �÷����� ��ǥ �������� �̵�
    private void MovePlatform()
    {
        Transform Target = _isMovingToB ? _pointB : _pointA;
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, Target.position, speed * Time.deltaTime);
    }

    // �÷��� ��ġ�� ���� �̵� ���� ��ȯ
    private void SetDirection()
    {
        if (_platform.transform.position == _pointA.position)
        {
            _isMovingToB = true;
        }
        else if (_platform.transform.position == _pointB.position)
        {
            _isMovingToB = false;
        }
    }

    // ���� �� �÷��� Ȱ��ȭ �� �ʱ�ȭ
    public void Reward()
    {
        this.gameObject.SetActive(true);
        _platform.transform.position = _pointA.position;
        _isMovingToB = true;
    }
}
