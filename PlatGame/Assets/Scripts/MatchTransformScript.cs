using UnityEngine;

public class MatchTransformScript : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = player.position;
        transform.rotation = player.rotation;
    }
}
