using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipTool : Equip
{

    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;
    private Transform RayStartPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
        RayStartPosition = CharacterManager.Instance.Player.RayStartPosition;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    /// <summary>
    /// 플레이어가 공격 시 호출되며, Raycast를 사용해 공격 범위 내의 오브젝트를 탐지하여
    /// 자원 채집 또는 데미지 처리를 수행합니다.
    /// </summary>
    public void OnHit()
    {
        Ray ray = new Ray(RayStartPosition.position, camera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, attackDistance);

        foreach (RaycastHit hit in hits)
        {

            GameObject player = CharacterManager.Instance.Player.gameObject;
            if (hit.collider.gameObject == player)
                continue;

            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
                break;
            }
            else if (doesDealDamage && hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakePhysicalDamage(damage);
                break;
            }
        }
    }
}
