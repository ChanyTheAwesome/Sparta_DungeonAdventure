using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition Health;
    public Condition Hunger;
    public Condition Energy;

    void Start()
    {
        CharacterManager.Instance.Player.condition.UICondition = this;
    }

    void Update()
    {
        
    }
}
