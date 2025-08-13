using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 플레이어의 컨트롤러, 상태, 장비 컴포넌트
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equip;

    // 아이템 관련 데이터 및 아이템 추가 액션
    public ItemData itemData;
    public Action addItem;

    // 아이템 드롭 위치 및 레이 시작 위치
    public Transform dropPosition;
    public Transform RayStartPosition;

    // 컴포넌트 초기화 및 싱글톤 매니저에 플레이어 등록
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equip = GetComponent<Equipment>();
    }

    // 플레이어가 특정 위치 이하로 떨어지면 위치를 초기화
    private void Update()
    {
        if (this.transform.position.y < -1.0f)
        {
            this.transform.position = new Vector3(6.41f, 0.957f, 0.0f);
        }
    }
}
