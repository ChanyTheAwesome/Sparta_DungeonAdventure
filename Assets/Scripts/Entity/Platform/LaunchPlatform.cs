using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatform : MonoBehaviour, IRewardable
{
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Reward()
    {
        this.gameObject.SetActive(true);
    }
}
