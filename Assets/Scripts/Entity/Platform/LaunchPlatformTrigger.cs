using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform PointA; // 시작 지점
    [SerializeField] private Transform PointB; // 도착 지점
    [SerializeField] private float launchForce; // 점프 힘

    private Coroutine launchCoroutine; // 점프 코루틴 참조

    // 플레이어가 트리거에 들어오면 Launch 실행
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Launch();
        }
    }

    // 플레이어가 트리거에서 나가면 코루틴 중지
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (launchCoroutine != null)
            {
                StopCoroutine(launchCoroutine);
                launchCoroutine = null;
            }
        }
    }

    // 점프 코루틴 시작
    private void Launch()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    // 1.5초 대기 후 플레이어를 점프시킴
    private IEnumerator LaunchCoroutine()
    {
        Vector3 direction = (PointB.position - PointA.position);
        yield return new WaitForSeconds(1.5f);
        CharacterManager.Instance.Player.controller.LaunchPlatformJump(direction, launchForce);
    }
}
