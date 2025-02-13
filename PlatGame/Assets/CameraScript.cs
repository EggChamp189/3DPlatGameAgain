using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // used this thread's 3rd person camera script as a base, which I edited off of since it doesn't work for my game exactly: https://discussions.unity.com/t/third-person-camera-movement-script/783511/1

    private const float YMinimum = -50.0f;
    private const float YMaximum = 89.0f;

    public Transform lookAt;

    public Transform Player;

    public float distance = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float sensivity = 4.0f;
    bool mouseLocked = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // you have to subtract the x, not add it like the tutorial says (for my camera movement prefrences)
        currentX -= Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        currentY += Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;

        // clamp the y so the camera doesn't get fucked up. edit the min and max variables if you want a greater range of camera
        currentY = Mathf.Clamp(currentY, YMinimum, YMaximum);

        // need to figure out how to have the camera to not clip through objects, but that not the most important thing in the game right now
        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = lookAt.position + rotation * Direction;

        transform.LookAt(lookAt.position);

        // lock and unlock mouse to screen when escape is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
            mouseLocked = !mouseLocked;
        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
}
