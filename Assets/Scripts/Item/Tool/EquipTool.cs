using System.Collections;
using System.Collections.Generic;
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

    public void OnHit()
    {
        Ray ray = new Ray(RayStartPosition.position, camera.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
            else if(doesDealDamage && hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakePhysicalDamage(damage);
            }
        }
    }
}
