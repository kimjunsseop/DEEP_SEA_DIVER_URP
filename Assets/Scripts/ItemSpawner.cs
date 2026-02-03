using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner instance;
    public Transform[] spawnPoints;
    public Transform[] playerItemSpawnPoints;
    public Player player;
    public GameObject[] items;
    public GameObject[] playerItems;
    public int playerItemSize = 5;
    public int itemSize = 3;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        List<int> rand = new List<int>();
        List<int> prand = new List<int>();
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            rand.Add(i);
        }
        for(int i = 0; i < playerItemSpawnPoints.Length; i++)
        {
            prand.Add(i);
        }
        for(int i = 0; i < rand.Count; i++)
        {
            int temp = rand[i];
            int randomIndex = UnityEngine.Random.Range(i,rand.Count);
            rand[i] = rand[randomIndex];
            rand[randomIndex] = temp;
        }
        for(int i = 0; i < prand.Count; i++)
        {
            int temp = prand[i];
            int randomIndex = UnityEngine.Random.Range(i, prand.Count);
            prand[i] = prand[randomIndex];
            prand[randomIndex] = temp;
        }
        List<int> r = new List<int>();
        for(int i = 0; i < items.Length; i++)
        {
            r.Add(i);
        }
        for(int i = 0; i < items.Length; i++)
        {
            int temp = r[i];
            int randomIndex = UnityEngine.Random.Range(i, r.Count);
            r[i] = r[randomIndex];
            r[randomIndex] = temp;
        }
        for(int i = 0; i < itemSize; i++)
        {
            Instantiate(items[r[i]], spawnPoints[rand[i]].position, Quaternion.identity);
            UIManager.instance.setImage(items[r[i]], i);
            UIManager.instance.itemss.Add(r[i], items[r[i]]);
            GameManager.instance.found.Add(false);
        }
        for(int i = 0; i < playerItemSize; i++)
        {
            int plr = UnityEngine.Random.Range(0,3);
            Instantiate(playerItems[plr], playerItemSpawnPoints[prand[i]].position, Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}
