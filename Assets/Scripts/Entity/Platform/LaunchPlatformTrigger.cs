using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform PointA; // ���� ����
    [SerializeField] private Transform PointB; // ���� ����
    [SerializeField] private float launchForce; // ���� ��

    private Coroutine launchCoroutine; // ���� �ڷ�ƾ ����

    // �÷��̾ Ʈ���ſ� ������ Launch ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Launch();
        }
    }

    // �÷��̾ Ʈ���ſ��� ������ �ڷ�ƾ ����
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

    // ���� �ڷ�ƾ ����
    private void Launch()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }

    // 1.5�� ��� �� �÷��̾ ������Ŵ
    private IEnumerator LaunchCoroutine()
    {
        Vector3 direction = (PointB.position - PointA.position);
        yield return new WaitForSeconds(1.5f);
        CharacterManager.Instance.Player.controller.LaunchPlatformJump(direction, launchForce);
    }
}
