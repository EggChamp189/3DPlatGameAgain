using UnityEngine;

public class MatchTransformScript : MonoBehaviour
{
    public Transform player;
    public Transform camera;

    void Update()
    {
        transform.position = player.position;
        Quaternion cameraRot = camera.rotation;
        cameraRot.x = 0;
        cameraRot.z = 0;
        transform.rotation = cameraRot;
    }
}
