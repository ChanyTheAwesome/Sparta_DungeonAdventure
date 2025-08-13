using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
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
    private Arena currentArena;
    public List<GameObject> spawnedMonsters = new();
    public List<ItemData> drops = new();
    public void ActivateThisArena(Arena arena)
    {
        if (arena != null)
        {
            currentArena = arena;
            arena.IsActive = true;
        }
    }
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