using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아레나 관리 클래스
public class ArenaManager : MonoBehaviour
{
    // 싱글톤 인스턴스
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

    // 현재 활성화된 아레나
    private Arena currentArena;
    // 스폰된 몬스터 목록
    public List<GameObject> spawnedMonsters = new();
    // 드랍 아이템 목록
    public List<ItemData> drops = new();

    // 아레나 활성화
    public void ActivateThisArena(Arena arena)
    {
        if (arena != null)
        {
            currentArena = arena;
            arena.IsActive = true;
        }
    }

    // 아레나 비활성화
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

    // 모든 몬스터 제거
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

    // 적 제거 및 보상 처리
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
        // 모든 적이 제거되면 아이템 드랍 및 보상
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

    // 보상 지급
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