using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ʒ��� ���� Ŭ����
public class ArenaManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static ArenaManager instance;
    public static ArenaManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ArenaManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("ArenaManager");
                    instance = obj.AddComponent<ArenaManager>();
                }
            }
            return instance;
        }
    }

    // ���� Ȱ��ȭ�� �Ʒ���
    private Arena currentArena;
    // ������ ���� ���
    public List<GameObject> spawnedMonsters = new();
    // ��� ������ ���
    public List<ItemData> drops = new();

    // �Ʒ��� Ȱ��ȭ
    public void ActivateThisArena(Arena arena)
    {
        if (arena != null)
        {
            currentArena = arena;
            arena.IsActive = true;
        }
    }

    // �Ʒ��� ��Ȱ��ȭ
    public void DeactivateArena()
    {
        if (currentArena != null)
        {
            DestroyMonsters();
            currentArena.IsActive = false;
            currentArena.MonsterSpawned = false;
            currentArena.Door.SetActive(true);
            drops.Clear();
            currentArena = null;
        }
    }

    // ��� ���� ����
    private void DestroyMonsters()
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        spawnedMonsters.Clear();
    }

    // �� ���� �� ���� ó��
    public void RemoveEnemy(GameObject go)
    {
        if (spawnedMonsters.Contains(go))
        {
            spawnedMonsters.Remove(go);
            Destroy(go);
        }
        else
        {
            Destroy(go);
            return;
        }
        // ��� ���� ���ŵǸ� ������ ��� �� ����
        if (spawnedMonsters.Count == 0)
        {
            foreach (ItemData item in drops)
            {
                if (item != null)
                {
                    GameObject itemObject = Instantiate(item.dropPrefab, currentArena.ItemDropPosition.position, Quaternion.identity);
                }
            }
            if (!currentArena.IsRewarded)
            {
                currentArena.IsRewarded = true;
                RewardCheck();
            }
        }
    }

    // ���� ����
    private void RewardCheck()
    {
        GameObject rewardObject = currentArena.RewardObject;
        IRewardable reward = rewardObject.GetComponent<IRewardable>();
        if (reward != null)
        {
            reward.Reward();
        }
    }
}