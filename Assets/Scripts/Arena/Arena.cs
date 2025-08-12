using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private GameObject SpawnNPCPrefab;
    [SerializeField] private int npcCount;
    [SerializeField] private Transform laserPointA;
    [SerializeField] private Transform laserPointB;
    [SerializeField] private Transform[] spawnArea;
    [SerializeField] private GameObject door;
    [SerializeField] private LayerMask playerLayerMask;
    public bool MonsterSpawned = false;
    public bool PlayerInLaser = false;
    private List<GameObject> spawnedMonsters = new List<GameObject>();

    private void Update()
    { 
        LaserCheck();
    }

    private void LaserCheck()
    {
        Ray ray = new Ray(laserPointA.position, laserPointB.position - laserPointA.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!PlayerInLaser)
                {
                    PlayerInLaser = true;
                    if(!MonsterSpawned)
                    {
                        MonsterSpawned = true;
                        SpawnMonsters();
                    }
                    else
                    {
                        DestroyMonsters();
                        MonsterSpawned = false;
                        if(door != null)
                        {
                            door.SetActive(true);
                            door.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
            }
            else
            {
                PlayerInLaser = false;
                door.GetComponent<Collider>().enabled = true;
            }
        }
        
    }
    private void SpawnMonsters()
    {
        for(int i = 0; i<npcCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            GameObject go = Instantiate(SpawnNPCPrefab, spawnPosition, Quaternion.identity);
            if (go != null)
            {
                spawnedMonsters.Add(go);
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
            spawnPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
            RaycastHit hit;
            if(Physics.Raycast(spawnPosition + Vector3.up * 20.0f, Vector3.down, out hit, 100f))
            {
                spawnPosition = hit.point;
            }
        }
        else
        {
            spawnPosition = transform.position;
        }
        return spawnPosition;
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
}
