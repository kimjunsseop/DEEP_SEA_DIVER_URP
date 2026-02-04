using UnityEngine;
using System.Collections.Generic;


public class MonsterSpawner : MonoBehaviour
{
    public MonsterSpawner instance;
    public Monster[] monsters;
    public int MaxSize;
    public Transform[] monSpawns;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        InitializeMonster();
    }
    void Update()
    {
        
    }

    void InitializeMonster()
    {
        List<int> rand = new List<int>();
        for(int i = 0; i < monSpawns.Length; i++)
        {
            rand.Add(i);
        }
        for(int i = 0; i < rand.Count; i++)
        {
            int temp = rand[i];
            int randomIndex = Random.Range(i,rand.Count);
            rand[i] = rand[randomIndex];
            rand[randomIndex] = temp;
        }
        for(int i = 0; i < MaxSize; i++)
        {
            int mr = Random.Range(0,8);
            Instantiate(monsters[mr], monSpawns[rand[i]].position, Quaternion.identity);
            
        }
    }
}
