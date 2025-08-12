using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceDoor : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return "E키를 눌러 문 열기";
    }
    public void OnInteract()
    {
        this.gameObject.SetActive(false);
    }
}