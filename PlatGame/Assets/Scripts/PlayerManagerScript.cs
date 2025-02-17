using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerManagerScript : MonoBehaviour
{
    [Header("General")]
    public float damage;
    bool isDoingSomething = false;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    private int speedNum;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump = true;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Animation")]
    public Animator anim;
    // add all attack animations in this. 0 should be the non-moving grounded attack, 1 should be the moving grounded attack, and 2 should be the falling attack.
    public List<AnimationClip> attackAnimations;

    [Header("Dependencies")]
    public Transform orientation;
    public Transform cam;
    public ObligatoryScriptableObject scriptableObject;

    float hInput;
    float vInput;
    Vector3 moveDirection;
    Rigidbody rb; 
    
    public MovementState state = MovementState.idle;
    public enum MovementState
    {
        idle,
        jumping,
        falling,
        walking,
        attacking,
        moveAttacking,
        fallAttacking,
        sprinting
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        moveSpeed = walkSpeed;
        readyToJump = true;
        damage = scriptableObject.damage;
    }

    // Update is called once per frame
    void Update()
    {
        // check slightly below the player if they are close enough to a ground layer object
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        UpdateAllInputs();
        StateHandler();
        if (!isDoingSomething)
        {
            SpeedControl();
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    void UpdateAllInputs()
    {
        // record the movement of the player 
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        // if the player is attacking, keep the last movement vector so wonky movement doesn't happen? might change in the future
        if (!isDoingSomething)
            // calculate the direction and force the player wants to move in and record it 
            moveDirection = orientation.forward * vInput + orientation.right * hInput;

        // reset character position if you press R
        if (Input.GetKey(KeyCode.R) && !isDoingSomething)
        {
            transform.position = new Vector3(0, 2, 0);
            rb.linearVelocity = Vector3.zero;
        }
        // jump if ready to jump again
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded && !isDoingSomething)
        {
            readyToJump = false;

            Jump();

            // anything that happens after jumping happens in this function, which is called after jump cooldown
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    // update the movement state based on what the player is doing.
    private void StateHandler()
    {
        // make sure the player isn't in the middle of another action
        if (!isDoingSomething)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) 
            {
                isDoingSomething = true;
                // this won't completely halt movement, just make it so movement can't be added while the attack happens
                if (grounded && state == MovementState.idle)
                {
                    moveSpeed = 0;
                    state = MovementState.attacking;
                    // get the exact length of the attack animation in case I end up changing it
                    Invoke(nameof(DoneAttacking), attackAnimations[0].length);
                }
                else if (grounded && (state == MovementState.walking || state == MovementState.sprinting))
                {
                    // this will give the illusion of a lunge when the attack happens
                    state = MovementState.moveAttacking;
                    // get the exact length of the attack animation in case I end up changing it
                    Invoke(nameof(DoneAttacking), attackAnimations[1].length);
                }
                else
                {
                    // allow some movement, but will be very small
                    moveSpeed /= 2;
                    state = MovementState.fallAttacking;
                    // get the exact length of the attack animation in case I end up changing it
                    Invoke(nameof(DoneAttacking), attackAnimations[2].length);
                }
            }
            // check if the player gave any movement inputs 
            else if (moveDirection.magnitude != 0)
            {
                // if pressing the run key, change the move speed
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveSpeed = sprintSpeed;
                    state = MovementState.sprinting;
                }
                else
                {
                    moveSpeed = walkSpeed;
                    state = MovementState.walking;
                }
                // need a air check in here too to make sure that you stay falling even if moving in the air
                if (!grounded)
                {
                    state = MovementState.falling;
                    anim.SetBool("Falling", true);
                }
            }
            // if they are not on the ground, they are falling in the air
            else if (!grounded)
            {
                state = MovementState.falling;
                anim.SetBool("Falling", true);
            }
            // if they are on the ground, they are idling on the ground lol
            else
            {
                state = MovementState.idle;
                // slow player's veloctity if they are idling on the gorund
                rb.linearVelocity *= 0.99f;
                anim.SetBool("Falling", false);
            }

            if (!grounded)
            {
                anim.SetBool("Falling", true);
            }
            // if they are on the ground, they are idling on the ground lol
            else
            {
                // slow player's veloctity if they are idling on the gorund
                rb.linearVelocity *= 0.99f;
                anim.SetBool("Falling", false);
            }

            // update the animation float accordingly
            switch (state)
            {
                case MovementState.idle:
                    speedNum = 0;
                    break;
                case MovementState.falling:
                    speedNum = 0;
                    break;
                case MovementState.walking:
                    speedNum = 1;
                    break;
                case MovementState.sprinting:
                    speedNum = 2;
                    break;
                    // reguardless of the attack type, the attack trigger should only go through one time since the isDoingSomething variable should be set to true
                case MovementState.attacking:
                    anim.SetTrigger("AttackStand");
                    break;
                case MovementState.moveAttacking:
                    anim.SetTrigger("AttackMove");
                    break;
                case MovementState.fallAttacking:
                    anim.SetTrigger("AttackFall");
                    break;
                default:
                    break;
            }
            anim.SetFloat("Speed", speedNum);
        }
    }

    private void SpeedControl()
    {
        // limiting speed on ground or in air
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit the velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        

    }

    // this function gets invoked when the player is finished the attack animation to reset the state
    private void DoneAttacking() {
        isDoingSomething = false;
        // immediately update once done so that the state can be changed to normal before the next frame starts
        StateHandler();
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        anim.SetTrigger("Jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    void MovePlayer() {

        // get angle of movement direction in order to seperate the orientation/camera rotation from the movement controls
        float moveAngle = Mathf.Atan2(hInput, vInput) * Mathf.Rad2Deg;

        if (isDoingSomething)
            moveSpeed *= 0.95f;

        // different movement based on whether or not the player is grounded
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        }
        if (moveDirection.magnitude != 0)
        {
            // this part of the script also was interpretted from this: https://discussions.unity.com/t/third-person-camera-movement-script/783511/1

            // should rotate the player in the direction they want to move when the orientation rotates, while not needing to face forward to move in the correct direction.
            // THIS IS IT!!!! MULTIPLYING THE ROTATION OF THE ANGLE OF THE MOVEMENT 2D VECTOR WITH THE ORIENTATION THAT MATCHES THE CAMERA'S ROTATION ALLOWS ME TO GET THE HAT IN TIME 3RD PERSON CAMERA!!!!!!! HAHAHAHAHHAVIHEROVGIU[EWHBOV;ERW i WIN!!!!!!1
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, moveAngle, 0) * orientation.rotation, 0.15f);
        }

    }

    public void GetHit()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy")) {
            GetHit();
        }
    }
}
