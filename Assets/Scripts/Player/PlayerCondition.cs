using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}
public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition UICondition;
    private Condition health { get { return UICondition.Health; } }
    private Condition hunger { get { return UICondition.Hunger; } }
    private Condition energy { get { return UICondition.Energy; } }

    public float noHungerHealthLossRate;

    public event Action OnTakeDamage;
    [HideInInspector]
    public bool IsInfiniteStamina;
    private Coroutine _coroutine;
    void Update()
    {
        energy.Add(energy.passiveValue * Time.deltaTime);
        health.Add(health.passiveValue * Time.deltaTime);
        if (hunger.curValue <= 0)
        {
            health.Subtract(noHungerHealthLossRate * Time.deltaTime);
        }
        else
        {
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        }
        if(health.curValue <= 0)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        health.Add(amount);
    }
    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }
    public void Die()
    {
        health.Add(health.maxValue);
        hunger.Add(hunger.maxValue);
        energy.Add(energy.maxValue);
        this.gameObject.transform.position = new Vector3(6.41f, 0.957f, 0.0f);
    }

    public bool UseStamina(float amount)
    {
        if(energy.curValue - amount < 0)
        {
            return false;
        }
        if (!IsInfiniteStamina)
        {
            energy.Subtract(amount);
        }
        return true;
    }
    public void InfiniteStamina(float time)
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CoroutineInfiniteStamina(time));
    }
    private IEnumerator CoroutineInfiniteStamina(float time)
    {
        IsInfiniteStamina = true;
        yield return new WaitForSeconds(time);
        IsInfiniteStamina = false;
    }
    public void ReduceHungerPassiveValue(float value)
    {
        hunger.passiveValue = (1 - value) * hunger.passiveValue;
        if (hunger.passiveValue < 0)
        {
            hunger.passiveValue = 0;
        }
    }
    public void AddMaxHealth(float value)
    {
        health.maxValue += value;
        health.startValue += value;
        health.curValue = health.startValue;
    }
}
