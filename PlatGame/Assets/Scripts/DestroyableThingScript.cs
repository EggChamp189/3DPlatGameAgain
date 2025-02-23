using UnityEngine;

public class DestroyableThingScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            PlayerManagerScript playerScript = FindFirstObjectByType<PlayerManagerScript>();
            Rigidbody player = playerScript.gameObject.GetComponent<Rigidbody>();

            //Debug.Log("Detected Hit");
            if (playerScript.state == PlayerManagerScript.MovementState.fallAttacking)
            {
                // bounce up again if the player hit the enemy in the air
                player.linearVelocity = new Vector3(player.linearVelocity.x * 0.8f, 12f, player.linearVelocity.z * 0.8f);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
}
