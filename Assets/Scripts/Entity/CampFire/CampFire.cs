using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float damageRate;

    private List<IDamageable> things = new List<IDamageable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    private void DealDamage()
    {
        for(int i = 0; i<things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            things.Add(damageable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            things.Remove(damageable);
        }
    }
}
