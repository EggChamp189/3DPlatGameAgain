using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask whatIsGround;

    bool foundPlayer;
    float turnSpeed = 100.0f;
    float lastAttack = 0;
    float attackCooldown = 2.0f;
    bool activelyAttacking = false;
    float launchStrength = 2.0f;

    float timeSinceLeftGround = 0f;
    float timeBeforeAttackAgain = 0.5f;

    private void Start()
    {
        // so enemies don't all attack at the same time
        attackCooldown += Random.Range(-0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerDirection = FindFirstObjectByType<PlayerManagerScript>().transform.position - transform.position;

        // if the player is close to the enemy, get ready to attack them.
        if (playerDirection.magnitude < 10 && !activelyAttacking)
            foundPlayer = true;
        else
            foundPlayer = false;

        // is done their "attack" when they touch the ground
        if (activelyAttacking)
        {
            timeSinceLeftGround += Time.deltaTime;
            if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, whatIsGround) && timeSinceLeftGround >= timeBeforeAttackAgain)
            {
                activelyAttacking = false;
                timeSinceLeftGround = 0;
            }
        }

        if (foundPlayer) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDirection), Time.deltaTime * turnSpeed);

            lastAttack += Time.deltaTime;
            if (lastAttack >= attackCooldown)
            {
                // only attack in the direction they are facing, since the player might be annoyed if the enemy suddenly rotates at them while they dodge
                GetComponent<Rigidbody>().AddForce((transform.forward * 5 + Vector3.up * 3) * launchStrength, ForceMode.Impulse); 
                activelyAttacking = true;
                lastAttack = 0;
            }
        }

    }

    public void Die() {
        GeneralManager.EnemyDeath();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")) { 
            if(!collision.gameObject.GetComponent<PlayerManagerScript>().isInvulnerable)
            {
                StartCoroutine(collision.gameObject.GetComponent<PlayerManagerScript>().GetHit(collision.gameObject.GetComponent<PlayerManagerScript>().thisLevel));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Rigidbody player = FindFirstObjectByType<PlayerManagerScript>().gameObject.GetComponent<Rigidbody>();
            if (FindFirstObjectByType<PlayerManagerScript>().state == PlayerManagerScript.MovementState.fallAttacking) {
                // bounce up again if the player hit the enemy in the air
                player.linearVelocity = new Vector3(player.linearVelocity.x * 0.6f, 12f, player.linearVelocity.z * 0.6f);
            }
            else if (FindFirstObjectByType<PlayerManagerScript>().state == PlayerManagerScript.MovementState.moveAttacking)
            {
                // bounce player back if they hit
                FindFirstObjectByType<PlayerManagerScript>().moveDirection *= -1f;
                player.linearVelocity = new Vector3(player.linearVelocity.x * -0.4f, 3f, player.linearVelocity.z * -0.4f);
            }
            Die();
        }
    }
}
