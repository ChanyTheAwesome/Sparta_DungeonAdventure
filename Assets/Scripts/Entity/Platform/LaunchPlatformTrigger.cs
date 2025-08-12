using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform PointA;
    [SerializeField] private Transform PointB;
    [SerializeField] private float launchForce;

    private Coroutine launchCoroutine;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Launch();
        }
    }
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
    private void Launch()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
        }
        launchCoroutine = StartCoroutine(LaunchCoroutine());
    }
    private IEnumerator LaunchCoroutine()
    {
        Vector3 direction = (PointB.position - PointA.position);
        yield return new WaitForSeconds(1.5f);
        CharacterManager.Instance.Player.controller.LaunchPlatformJump(direction, launchForce);
    }
}
