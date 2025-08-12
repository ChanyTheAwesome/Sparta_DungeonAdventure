using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private GameObject _platform;

    [SerializeField] private float speed = 1.0f;

    private bool _isMovingToB;
    private void Start()
    {
        _platform.transform.position = _pointA.position;
        _isMovingToB = true;
    }
    private void Update()
    {
        MovePlatform();
        SetDirection();
    }

    private void MovePlatform()
    {
        Transform Target = _isMovingToB ? _pointB : _pointA;
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, Target.position, speed * Time.deltaTime);
    }

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
}
