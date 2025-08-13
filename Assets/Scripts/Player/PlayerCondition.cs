using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 데미지를 받을 수 있는 객체를 위한 인터페이스
public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}
// 플레이어의 상태(체력, 허기, 에너지)를 관리하는 클래스
public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition UICondition; // UI와 연결된 상태 정보
    private Condition health { get { return UICondition.Health; } } // 체력
    private Condition hunger { get { return UICondition.Hunger; } } // 허기
    private Condition energy { get { return UICondition.Energy; } } // 에너지

    public float noHungerHealthLossRate; // 허기가 0일 때 체력 감소 속도

    public event Action OnTakeDamage; // 데미지 이벤트
    [HideInInspector]
    public bool IsInfiniteStamina; // 무한 스태미나 여부
    private Coroutine _coroutine; // 무한 스태미나 코루틴 참조

    // 매 프레임마다 상태를 갱신
    void Update()
    {
        energy.Add(energy.passiveValue * Time.deltaTime); // 에너지 자연 회복
        health.Add(health.passiveValue * Time.deltaTime); // 체력 자연 회복
        if (hunger.curValue <= 0)
        {
            health.Subtract(noHungerHealthLossRate * Time.deltaTime); // 허기가 없으면 체력 감소
        }
        else
        {
            hunger.Subtract(hunger.passiveValue * Time.deltaTime); // 허기 자연 감소
        }
        if (health.curValue <= 0)
        {
            Die(); // 체력이 0 이하이면 사망 처리
        }
    }

    // 체력 회복
    public void Heal(float amount)
    {
        health.Add(amount);
    }

    // 물리적 데미지 처리
    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    // 음식 섭취(허기 회복)
    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    // 사망 처리 및 상태 초기화
    public void Die()
    {
        health.curValue = 0;
        health.Add(health.maxValue);
        hunger.Add(hunger.maxValue);
        energy.Add(energy.maxValue);
        this.gameObject.transform.position = new Vector3(6.41f, 0.957f, 0.0f); // 리스폰 위치
    }

    // 스태미나 사용
    public bool UseStamina(float amount)
    {
        if (energy.curValue - amount < 0)
        {
            return false; // 스태미나 부족
        }
        if (!IsInfiniteStamina)
        {
            energy.Subtract(amount);
        }
        return true;
    }

    // 일정 시간 동안 무한 스태미나 적용
    public void InfiniteStamina(float time)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CoroutineInfiniteStamina(time));
    }

    // 무한 스태미나 코루틴
    private IEnumerator CoroutineInfiniteStamina(float time)
    {
        IsInfiniteStamina = true;
        yield return new WaitForSeconds(time);
        IsInfiniteStamina = false;
    }

    // 허기 자연 감소 속도 감소
    public void ReduceHungerPassiveValue(float value)
    {
        hunger.passiveValue = (1 - value) * hunger.passiveValue;
        if (hunger.passiveValue < 0)
        {
            hunger.passiveValue = 0;
        }
    }

    // 최대 체력 증가
    public void AddMaxHealth(float value)
    {
        health.maxValue += value;
        health.startValue += value;
        health.curValue = health.startValue;
    }

    // 체력 자연 회복 속도 증가
    public void IncreaseHealthPassiveValue(float value)
    {
        health.passiveValue += value;
        if (health.passiveValue < 0)
        {
            health.passiveValue = 0;
        }
    }
}
