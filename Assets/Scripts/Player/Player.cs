using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �÷��̾��� ��Ʈ�ѷ�, ����, ��� ������Ʈ
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equip;

    // ������ ���� ������ �� ������ �߰� �׼�
    public ItemData itemData;
    public Action addItem;

    // ������ ��� ��ġ �� ���� ���� ��ġ
    public Transform dropPosition;
    public Transform RayStartPosition;

    // ������Ʈ �ʱ�ȭ �� �̱��� �Ŵ����� �÷��̾� ���
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equip = GetComponent<Equipment>();
    }

    // �÷��̾ Ư�� ��ġ ���Ϸ� �������� ��ġ�� �ʱ�ȭ
    private void Update()
    {
        if (this.transform.position.y < -1.0f)
        {
            this.transform.position = new Vector3(6.41f, 0.957f, 0.0f);
        }
    }
}
