using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceDoor : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt()
    {
        return "EŰ�� ���� �� ����";
    }
    public void OnInteract()
    {
        this.gameObject.SetActive(false);
    }
}