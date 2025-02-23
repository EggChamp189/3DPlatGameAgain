using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    public EnemyData data;
    public List<GameObject> enemySpawns;
    public static int enemiesLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemiesLeft = 0;
        foreach (GameObject spawn in enemySpawns)
        {
            data.SetupEnemy(spawn.transform.position);
            enemiesLeft++;
        }
        if (enemiesLeft <= 0)
            FindFirstObjectByType<GoalScript>().EnableGoal();
    }

    public static void EnemyDeath()
    {
        enemiesLeft--;
        if (enemiesLeft <= 0)
            FindFirstObjectByType<GoalScript>().EnableGoal();
    }
}
