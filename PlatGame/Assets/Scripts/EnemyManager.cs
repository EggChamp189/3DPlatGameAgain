using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyData data;
    public List<GameObject> enemySpawns;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject spawn in enemySpawns) {
            data.SetupEnemy(spawn.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
