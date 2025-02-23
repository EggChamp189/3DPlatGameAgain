using UnityEngine;
using UnityEngine.EventSystems;

public class SpringScript : MonoBehaviour
{
    public float springForce = 18;
    float timer = 0;
    float timerReset = 0.1f;

    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.CompareTag("Player") && timer > timerReset)
        {
            timer = 0;
            PlayerManagerScript playerScript = FindFirstObjectByType<PlayerManagerScript>();
            Rigidbody player = playerScript.gameObject.GetComponent<Rigidbody>();

            if (playerScript.state == PlayerManagerScript.MovementState.fallAttacking || playerScript.state == PlayerManagerScript.MovementState.falling || playerScript.state == PlayerManagerScript.MovementState.jumping)
            {
                player.AddForce((transform.up) * springForce, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && timer > timerReset)
        {
            timer = 0;
            PlayerManagerScript playerScript = FindFirstObjectByType<PlayerManagerScript>();
            Rigidbody player = playerScript.gameObject.GetComponent<Rigidbody>();

            if (playerScript.state == PlayerManagerScript.MovementState.fallAttacking || playerScript.state == PlayerManagerScript.MovementState.falling || playerScript.state == PlayerManagerScript.MovementState.jumping)
            {
                player.AddForce((transform.up) * springForce, ForceMode.Impulse);
            }
        }
    }
}
