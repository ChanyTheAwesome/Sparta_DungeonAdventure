using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    [SerializeField] private float maxValue;
    [SerializeField] private float startValue;
    [SerializeField] private Image uiBar;

    public float passiveValue;
    public float curValue;

    private void Start()
    {
        curValue = startValue;
    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage();
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0);
    }
}
