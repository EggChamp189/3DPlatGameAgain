using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public List<GameObject> EnemiesToChoose;
    public List<Material> Materials;

    public GameObject SetupEnemy(Vector3 location) {
        GameObject newEnemy = Instantiate(EnemiesToChoose[Random.Range(0, EnemiesToChoose.Capacity)]);

        // this is the way i've figured out is the best way to change the materials of a skinned mesh renderer
        var materials = newEnemy.GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[0] = Materials[Random.Range(0, Materials.Capacity)];
        newEnemy.GetComponentInChildren<SkinnedMeshRenderer>().materials = materials;

        newEnemy.transform.position = location;
        return newEnemy;
    }
}
