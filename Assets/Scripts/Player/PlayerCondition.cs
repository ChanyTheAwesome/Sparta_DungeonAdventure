using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������� ���� �� �ִ� ��ü�� ���� �������̽�
public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}
// �÷��̾��� ����(ü��, ���, ������)�� �����ϴ� Ŭ����
public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition UICondition; // UI�� ����� ���� ����
    private Condition health { get { return UICondition.Health; } } // ü��
    private Condition hunger { get { return UICondition.Hunger; } } // ���
    private Condition energy { get { return UICondition.Energy; } } // ������

    public float noHungerHealthLossRate; // ��Ⱑ 0�� �� ü�� ���� �ӵ�

    public event Action OnTakeDamage; // ������ �̺�Ʈ
    [HideInInspector]
    public bool IsInfiniteStamina; // ���� ���¹̳� ����
    private Coroutine _coroutine; // ���� ���¹̳� �ڷ�ƾ ����

    // �� �����Ӹ��� ���¸� ����
    void Update()
    {
        energy.Add(energy.passiveValue * Time.deltaTime); // ������ �ڿ� ȸ��
        health.Add(health.passiveValue * Time.deltaTime); // ü�� �ڿ� ȸ��
        if (hunger.curValue <= 0)
        {
            health.Subtract(noHungerHealthLossRate * Time.deltaTime); // ��Ⱑ ������ ü�� ����
        }
        else
        {
            hunger.Subtract(hunger.passiveValue * Time.deltaTime); // ��� �ڿ� ����
        }
        if (health.curValue <= 0)
        {
            Die(); // ü���� 0 �����̸� ��� ó��
        }
    }

    // ü�� ȸ��
    public void Heal(float amount)
    {
        health.Add(amount);
    }

    // ������ ������ ó��
    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    // ���� ����(��� ȸ��)
    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    // ��� ó�� �� ���� �ʱ�ȭ
    public void Die()
    {
        health.curValue = 0;
        health.Add(health.maxValue);
        hunger.Add(hunger.maxValue);
        energy.Add(energy.maxValue);
        this.gameObject.transform.position = new Vector3(6.41f, 0.957f, 0.0f); // ������ ��ġ
    }

    // ���¹̳� ���
    public bool UseStamina(float amount)
    {
        if (energy.curValue - amount < 0)
        {
            return false; // ���¹̳� ����
        }
        if (!IsInfiniteStamina)
        {
            energy.Subtract(amount);
        }
        return true;
    }

    // ���� �ð� ���� ���� ���¹̳� ����
    public void InfiniteStamina(float time)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CoroutineInfiniteStamina(time));
    }

    // ���� ���¹̳� �ڷ�ƾ
    private IEnumerator CoroutineInfiniteStamina(float time)
    {
        IsInfiniteStamina = true;
        yield return new WaitForSeconds(time);
        IsInfiniteStamina = false;
    }

    // ��� �ڿ� ���� �ӵ� ����
    public void ReduceHungerPassiveValue(float value)
    {
        hunger.passiveValue = (1 - value) * hunger.passiveValue;
        if (hunger.passiveValue < 0)
        {
            hunger.passiveValue = 0;
        }
    }

    // �ִ� ü�� ����
    public void AddMaxHealth(float value)
    {
        health.maxValue += value;
        health.startValue += value;
        health.curValue = health.startValue;
    }

    // ü�� �ڿ� ȸ�� �ӵ� ����
    public void IncreaseHealthPassiveValue(float value)
    {
        health.passiveValue += value;
        if (health.passiveValue < 0)
        {
            health.passiveValue = 0;
        }
    }
}
