using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRewardable
{
    public void Reward();
}
public class Arena : MonoBehaviour
{
    [SerializeField] private GameObject SpawnNPCPrefab; // ������ NPC ������
    [SerializeField] private int npcCount; // ������ NPC ��
    [SerializeField] private Transform laserPointA; // ������ ���� ��ġ
    [SerializeField] private Transform laserPointB; // ������ �� ��ġ
    [SerializeField] private Transform[] spawnArea; // ���� ���� ����(2�� Transform �ʿ�)
    [SerializeField] private GameObject door; // �Ʒ��� �� ������Ʈ
    public GameObject Door { get { return door; } } // �� ������Ʈ ���ٿ� ������Ƽ
    [SerializeField] public GameObject rewardObject; // ���� ������Ʈ
    public GameObject RewardObject { get { return rewardObject; } } // ���� ������Ʈ ���ٿ� ������Ƽ
    [SerializeField] private Transform itemDropPosition; // ������ ��� ��ġ
    public Transform ItemDropPosition { get { return itemDropPosition; } } // ������ ��� ��ġ ���ٿ� ������Ƽ

    [HideInInspector] public bool MonsterSpawned = false; // ���Ͱ� �����Ǿ����� ����
    [HideInInspector] public bool IsActive = false; // �Ʒ��� Ȱ��ȭ ����
    [HideInInspector] public bool IsRewarded = false; // ���� ���� ����

    private void Update()
    {
        if (IsActive)
        {
            LaserCheck(); // �Ʒ��� Ȱ��ȭ �� ������ üũ
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArenaManager.Instance.ActivateThisArena(this); // �÷��̾ �����ϸ� �Ʒ��� Ȱ��ȭ
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArenaManager.Instance.DeactivateArena(); // �÷��̾ ������ �Ʒ��� ��Ȱ��ȭ
        }
    }

    private void LaserCheck()
    {
        Ray ray = new Ray(laserPointA.position, laserPointB.position - laserPointA.position); // ������ ����
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!MonsterSpawned)
                {
                    MonsterSpawned = true;
                    SpawnMonsters(); // �÷��̾ �������� ������ ���� ����
                }
            }
        }
    }
    private void SpawnMonsters()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition(); // ���� ��ġ ���
            GameObject go = Instantiate(SpawnNPCPrefab, spawnPosition, Quaternion.identity, this.transform); // ���� ����
            if (go != null)
            {
                ArenaManager.Instance.spawnedMonsters.Add(go); // ������ ���� ����Ʈ�� �߰�
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
            spawnPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ)); // ���� ��ġ ���
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 20.0f, Vector3.down, out hit, 100f))
            {
                spawnPosition = hit.point; // ���鿡 �°� ��ġ ����
            }
        }
        else
        {
            spawnPosition = transform.position; // ���� ������ ������ �Ʒ��� ��ġ���� ����
        }
        return spawnPosition;
    }
}
