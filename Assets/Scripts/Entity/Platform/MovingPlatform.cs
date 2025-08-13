using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 움직이는 플랫폼을 제어하는 클래스, 보상 인터페이스 구현
public class MovingPlatform : MonoBehaviour, IRewardable
{
    [SerializeField] private Transform _pointA; // 이동 시작 지점
    [SerializeField] private Transform _pointB; // 이동 끝 지점
    [SerializeField] private GameObject _platform; // 실제 이동할 플랫폼 오브젝트

    [SerializeField] private float speed = 1.0f; // 이동 속도

    private bool _isMovingToB; // B 지점으로 이동 중인지 여부

    private void Start()
    {
        // 시작 시 플랫폼 비활성화
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        MovePlatform(); // 플랫폼 이동 처리
        SetDirection(); // 이동 방향 결정
    }

    // 플랫폼을 목표 지점으로 이동
    private void MovePlatform()
    {
        Transform Target = _isMovingToB ? _pointB : _pointA;
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, Target.position, speed * Time.deltaTime);
    }

    // 플랫폼 위치에 따라 이동 방향 전환
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

    // 보상 시 플랫폼 활성화 및 초기화
    public void Reward()
    {
        this.gameObject.SetActive(true);
        _platform.transform.position = _pointA.position;
        _isMovingToB = true;
    }
}
