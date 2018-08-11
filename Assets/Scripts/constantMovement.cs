using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constantMovement : MonoBehaviour
{
    public bool is2D;
    [Range(0f, 20f)]
    public float moveSpeed;
    [Range(0f, 50f)]
    public float jumpHeight;
    [Range(0f, 2f)]
    public float doubleJumpMultiplier;
    [Range(0f, 10f)]
    public float gravityModifier;
    [Range(0f, 5f)]
    public float wallJumpModifier;
    [Range(0, 1f)]
    public float wallJumpAngle;
    [Range(0f, 1f)]
    public float wallStiction;
    [Range(1f, 2f)]
    public float highJumpModifier;
    public int maxWallJumps;
    public int maxDoubleJumps;
    [Range(0f, 1f)]
    public float airMomentum;

    private CharacterController _controller;
    private Vector3 input;
    private Vector3 jump;
    private Vector3 wallJump;
    private Vector3 moveSum = Vector3.zero;
    private int wallJumpCount = 0;
    private float referenceVel = 0;
    private Vector3 momentumReference;
    private float zPos;
    private int doubleJumps;
    private Vector3 oldInput;
    private bool jumping;
    private float highJumpGravity;
    private bool started;
    private int direction;
    private bool collided;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        zPos = _controller.transform.position.z;
        direction = 1;
        started = false;
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(IsGrounded());
        Debug.Log(_controller.bounds.extents.y);
        // Movement and turning

        if (Input.GetKeyDown("space"))
        {
            started = true;
        }

        if (started){
            input = new Vector3(direction, 0, 0);  // constantly move right to start
        }

        float planeSpeed = Mathf.Sqrt(Mathf.Pow(_controller.velocity.x, 2) + Mathf.Pow(_controller.velocity.z, 2));

        // Animations
        //

        Vector2 lookDirection = new Vector2(-_controller.velocity.x / _controller.velocity.magnitude, _controller.velocity.z / _controller.velocity.magnitude);
        if (planeSpeed > 0.1)
        {
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        // Jumping

        if (IsGrounded())
        {
            // Reset everything on grounding
            jumping = false;
            wallJumpCount = 0;
            doubleJumps = 0;
            jump = Vector3.zero;
            wallJump = Vector3.zero;

            if (Input.GetButtonDown("Jump"))
            {
                jump.y = jumpHeight;
                jumping = true;
            }
        }

        else  // Mid Air
        {
            if (Input.GetButtonUp("Jump"))  // High jump grav reset
            {
                jumping = false;
                highJumpGravity = 1f;
            }

            if (jumping)
            {
                highJumpGravity = Mathf.Clamp(highJumpGravity * Mathf.Pow(0.9f, 250f * Time.deltaTime), 1 / highJumpModifier, 1f);  // Framerate independent
            }

            if (_controller.velocity.y <= 0)
            {
                highJumpGravity = 1f;
            }

            if (Input.GetButtonDown("Jump") && doubleJumps < maxDoubleJumps)  // Double Jumping
            {
                doubleJumps++;
                jump.y = jumpHeight * doubleJumpMultiplier;
            }

            // Ledge grab
            if (Input.GetButton("Interact"))
            {
                //Debug.DrawRay(_controller.transform.position + new Vector3(-1 * _controller.transform.forward.z, 1, 0), -Vector3.up, Color.red);

                if (Physics.Raycast(_controller.transform.position + new Vector3(-1 * _controller.transform.forward.z, 1, 0), -Vector3.up, 1f))
                {
                    Debug.Log("Ledge Grab!");
                }
            }

        }

        if ((_controller.collisionFlags & CollisionFlags.Above) != 0) // If character hits ceiling, kill vertical velocity
        {
            if (_controller.velocity.y >= 0)
            {
                jump.y = 0;
            }
        }

        if ((_controller.collisionFlags & CollisionFlags.Sides) != 0) // Friction for sticking to walls
        {
            jump.y += -jump.y * wallStiction;
        }

        // Movement

        jump.x = Mathf.SmoothDamp(jump.x, 0, ref referenceVel, 0.3f);  // Damp any horizontal jump velocity from wall jumps
        jump += Physics.gravity * Time.deltaTime * gravityModifier * highJumpGravity;  // Add gravity

        moveSum = (input * moveSpeed) + jump;  // Final movement vector is sum of all contributions
        _controller.Move(moveSum * Time.deltaTime);

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Direction Change
        if ((_controller.collisionFlags & CollisionFlags.Sides) != 0)
        {
            collided = !collided;
            if (!collided)
            {
                Debug.Log("Collide");
                direction *= -1;
                
            }

        }

        // Wall Jumping

        if (!_controller.isGrounded && (_controller.collisionFlags & CollisionFlags.Sides) != 0 && Input.GetButtonDown("Jump") && wallJumpCount < maxWallJumps)
        {
            wallJumpCount++;
            doubleJumps = 0;
            input = Vector3.zero;
            wallJump = Vector3.Lerp(hit.normal, Vector3.up, wallJumpAngle) * jumpHeight * wallJumpModifier;  // Vector of wall jump

            jump = wallJump;
        }
    }

    private bool IsGrounded()  // Better grounding check (_controller.isGrounded isnt as forgiving, causing double jumps from ground)
    {
        return Physics.Raycast(_controller.transform.position, -Vector3.up, _controller.bounds.extents.y + 0.1f);
    }

    private Vector3 Damp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }
}

