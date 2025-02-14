using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManagerScript : MonoBehaviour
{
    [Header("Stats")]
    public float damage;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

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
        air,
        walking,
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
        SpeedControl();
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

        // calculate the direction and force the player wants to move in and record it 
        moveDirection = orientation.forward * vInput + orientation.right * hInput;

        // reset character position if you press R
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = new Vector3(0, 2, 0);
            rb.linearVelocity = Vector3.zero;
        }
        // jump if ready to jump again
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
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
        // check if the player gave any movement inputs 
        if (moveDirection.magnitude != 0)
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
        }
        // if they are not on the ground, they are idling in the air
        else if (!grounded) 
        {
            state = MovementState.air;
        }
        // if they are on the ground, they are idling on the ground lol
        else
        {
            state = MovementState.idle;
            // slow player's veloctity if they are idling on the gorund
            rb.linearVelocity *= 0.99f;
        }


        // update the animation float accordingly
        switch (state) {
            case MovementState.idle:
                anim.SetFloat("Speed", 0);
                break;
            case MovementState.air:
                anim.SetFloat("Speed", 0);
                break;
            case MovementState.walking:
                anim.SetFloat("Speed", 1);
                break;
            case MovementState.sprinting:
                anim.SetFloat("Speed", 2);
                break;
            default:
                anim.SetFloat("Speed", 0);
                break;
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

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    void MovePlayer() {

        // get angle of movement direction in order to seperate the orientation/camera rotation from the movement controls
        float moveAngle = Mathf.Atan2(hInput, vInput) * Mathf.Rad2Deg;

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
}
