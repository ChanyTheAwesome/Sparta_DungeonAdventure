using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRewardable
{
    public void Reward();
}
public class Arena : MonoBehaviour
{
    [SerializeField] private GameObject SpawnNPCPrefab; // 스폰할 NPC 프리팹
    [SerializeField] private int npcCount; // 스폰할 NPC 수
    [SerializeField] private Transform laserPointA; // 레이저 시작 위치
    [SerializeField] private Transform laserPointB; // 레이저 끝 위치
    [SerializeField] private Transform[] spawnArea; // 몬스터 스폰 영역(2개 Transform 필요)
    [SerializeField] private GameObject door; // 아레나 문 오브젝트
    public GameObject Door { get { return door; } } // 문 오브젝트 접근용 프로퍼티
    [SerializeField] public GameObject rewardObject; // 보상 오브젝트
    public GameObject RewardObject { get { return rewardObject; } } // 보상 오브젝트 접근용 프로퍼티
    [SerializeField] private Transform itemDropPosition; // 아이템 드랍 위치
    public Transform ItemDropPosition { get { return itemDropPosition; } } // 아이템 드랍 위치 접근용 프로퍼티

    [HideInInspector] public bool MonsterSpawned = false; // 몬스터가 스폰되었는지 여부
    [HideInInspector] public bool IsActive = false; // 아레나 활성화 여부
    [HideInInspector] public bool IsRewarded = false; // 보상 지급 여부

    private void Update()
    {
        if (IsActive)
        {
            LaserCheck(); // 아레나 활성화 시 레이저 체크
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArenaManager.Instance.ActivateThisArena(this); // 플레이어가 진입하면 아레나 활성화
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArenaManager.Instance.DeactivateArena(); // 플레이어가 나가면 아레나 비활성화
        }
    }

    private void LaserCheck()
    {
        Ray ray = new Ray(laserPointA.position, laserPointB.position - laserPointA.position); // 레이저 생성
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!MonsterSpawned)
                {
                    MonsterSpawned = true;
                    SpawnMonsters(); // 플레이어가 레이저에 닿으면 몬스터 스폰
                }
            }
        }
    }
    private void SpawnMonsters()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(); // 스폰 위치 계산
            GameObject go = Instantiate(SpawnNPCPrefab, spawnPosition, Quaternion.identity, this.transform); // 몬스터 생성
            if (go != null)
            {
                ArenaManager.Instance.spawnedMonsters.Add(go); // 생성된 몬스터 리스트에 추가
            }
        }
    }
    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        if (spawnArea.Length > 0)
        {
            float minX = spawnArea[0].transform.position.x;
            float minZ = spawnArea[0].transform.position.z;
            float maxX = spawnArea[1].transform.position.x;
            float maxZ = spawnArea[1].transform.position.z;
            spawnPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ)); // 랜덤 위치 계산
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 20.0f, Vector3.down, out hit, 100f))
            {
                spawnPosition = hit.point; // 지면에 맞게 위치 조정
            }
        }
        else
        {
            spawnPosition = transform.position; // 스폰 영역이 없으면 아레나 위치에서 스폰
        }
        return spawnPosition;
    }
}
