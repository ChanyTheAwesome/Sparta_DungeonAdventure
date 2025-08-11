using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    List<IDamageable> damageables = new List<IDamageable>();
    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        foreach (IDamageable damageable in damageables)
        {
            if (damageable != null)
            {
                damageable.TakePhysicalDamage(damage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageables.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageables.Remove(damageable);
        }
    }
}