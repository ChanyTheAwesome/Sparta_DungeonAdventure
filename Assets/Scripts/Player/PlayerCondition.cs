using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uiCondition;
    
    private Condition _health { get { return uiCondition.health; } }
    private Condition _hunger { get { return uiCondition.hunger; } }
    private Condition _stamina { get { return uiCondition.stamina; } }

    [SerializeField] private float noHungerHealthDecay;
    
    [HideInInspector]
    public event Action onTakeDamage;

    private void Update()
    {
        _hunger.Subtract(_hunger.passiveValue * Time.deltaTime);
        _stamina.Add(_stamina.passiveValue * Time.deltaTime);
        if(_hunger.curValue < 0.0f)
        {
            _health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (_health.curValue < 0.0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        _health.Add(amount);
    }

    public void Eat(float amount)
    {
        _hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("Player has died.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        _health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}
