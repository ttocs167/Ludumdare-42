﻿using System.Collections;
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
    public float dashMultiplier;
    public GameObject managerObject;
    public Vector3 externalMovements;

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
    private gameManagement gameManager;
    public int direction;
    private bool collided;
    private float currentMoveSpeed;
    private bool started;
    private GameObject lava;
    private bool coyoteActive;
    private bool wasGrounded;
    private float freezeTimeLeft;

    public bool isDashing = false;
	private AudioSource CoinFX;
	public AudioSource OtherFX;
	public AudioSource BumpFX;
	public AudioClip FootSteps;
	public AudioClip Bump;

	public AudioSource Clock;

    // Use this for initialization
    void Start()
    {
        gameManager = managerObject.GetComponent<gameManagement>();
        _controller = GetComponent<CharacterController>();
        lava = GameObject.FindGameObjectWithTag("Water");
        zPos = _controller.transform.position.z;
        direction = 1;
        collided = false;

		CoinFX = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {
        started = gameManager.started;
        // Movement and turning

        if (is2D)
        {
            _controller.transform.position = new Vector3(_controller.transform.position.x, _controller.transform.position.y, zPos);
        }

        if (!started && Input.GetKeyDown("space"))
        {
            started = true;
			OtherFX.Pause ();
        }

        if (started){
            input = new Vector3(direction, 0, 0);  // constantly move right to start

        }

        float planeSpeed = Mathf.Sqrt(Mathf.Pow(_controller.velocity.x, 2) + Mathf.Pow(_controller.velocity.z, 2));

        // Dash Movement
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isDashing = true;
            currentMoveSpeed = moveSpeed * dashMultiplier;
        }
        else
        {
            isDashing = false;
            currentMoveSpeed = moveSpeed;
        }

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
            wasGrounded = true;
            coyoteActive = false;


			if ((!OtherFX.isPlaying) && started) {
				OtherFX.clip = FootSteps;
				OtherFX.loop = true;
				OtherFX.Play ();
			}

            if (Input.GetKeyDown(KeyCode.Mouse0) && started)
            {
                jump.y = jumpHeight;
                jumping = true;
				OtherFX.Pause ();

            }
        }

        else  // Mid Air
        {
            if (wasGrounded)  // Coyote time
            {
                StartCoroutine(CoyoteTime(0.2f));
            }

            if (coyoteActive && Input.GetKeyDown(KeyCode.Mouse0) && started)
            {
                print("Coyote Jump!");
                jump.y = jumpHeight;
                jumping = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))  // High jump grav reset
            {
                jumping = false;
                highJumpGravity = 1f;
            }

            if (jumping)
            {
				OtherFX.Pause ();
                highJumpGravity = Mathf.Clamp(highJumpGravity * Mathf.Pow(0.9f, 250f * Time.deltaTime), 1 / highJumpModifier, 1f);  // Framerate independent
            }

            if (_controller.velocity.y <= 0)
            {
                highJumpGravity = 1f;
				OtherFX.Pause ();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && doubleJumps < maxDoubleJumps && !coyoteActive)  // Double Jumping
            {
                doubleJumps++;
                jump.y = jumpHeight * doubleJumpMultiplier;
				OtherFX.Pause ();
            }

            wasGrounded = false;
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

        jump += externalMovements;
        jump.x = Mathf.SmoothDamp(jump.x, 0, ref referenceVel, 0.3f);  // Damp any horizontal jump velocity from wall jumps
        jump += Physics.gravity * Time.deltaTime * gravityModifier * highJumpGravity;  // Add gravity

        moveSum = (input * currentMoveSpeed) + jump;  // Final movement vector is sum of all contributions
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
                direction *= -1;
				//OtherFX.Pause ();
				//OtherFX.loop = false;
				//OtherFX.clip = Bump;
				BumpFX.PlayOneShot (Bump);
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
        if(hit.transform.tag == "Water")
        {
            gameManager.Death();
        }

        // David's shitty shit

        hit.transform.SendMessage("playerCollision", SendMessageOptions.DontRequireReceiver);

    }

    void OnTriggerEnter(Collider other)
    {  
        if (other.gameObject.tag == "Coin")
        {
			CoinFX.Play ();
            Debug.Log("COIN!");
            other.gameObject.SetActive(false);
            gameManager.coinCount++;
            gameManager.CoinUpdate();
		
        }

        if(other.gameObject.tag == "Clock")
        {
            Debug.Log("CLOCK!");
            StartCoroutine(FreezeLava(2f, lava));
            other.gameObject.SetActive(false);
            gameManager.coinCount++;
            gameManager.CoinUpdate();
        }

    }

    private bool IsGrounded()  // Better grounding check (_controller.isGrounded isnt as forgiving, causing double jumps from ground)
    {
        return Physics.Raycast(_controller.transform.position, -this.transform.up, _controller.bounds.extents.y +0.1f);
    }
    
    private Vector3 Damp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        return Vector3.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

    IEnumerator FreezeLava(float waitTime, GameObject lava)
    {
        if (lava.GetComponent<oceanMovement>().rising == false)  // if lava is frozen, add time to timer
        {
            freezeTimeLeft += waitTime;
        }

        if (lava.GetComponent<oceanMovement>().rising == true)  // if lava is not freezing, freeze it and play clock effect
        {
            lava.GetComponent<oceanMovement>().rising = false;
            Clock.Play();

            for (freezeTimeLeft = waitTime; freezeTimeLeft > 0; freezeTimeLeft -= Time.deltaTime)  // wait until timer is done to finish coroutine
            {
                yield return null;
            }
            lava.GetComponent<oceanMovement>().rising = true;
            Clock.Pause();
        }
    }

    IEnumerator CoyoteTime(float waitTime)
    {
        print("Coyote time!");
        coyoteActive = true;

        yield return new WaitForSeconds(waitTime);

        coyoteActive = false;
    }
}

