using UnityEngine;
using System.Collections;
using Rewired;

/* Mac Key Bindings for xbox controller */
//D-pad up: joystick button 5
//D-pad down: joystick button 6
//D-pad left: joystick button 7
//D-pad right: joystick button 8
//start: joystick button 9
//back: joystick button 10
//left stick(click): joystick button 11
//right stick(click): joystick button 12
//left bumper: joystick button 13
//right bumper: joystick button 14
//center("x") button: joystick button 15
//A: joystick button 16
//B: joystick button 17
//X: joystick button 18
//Y: joystick button 19

public class PlayerController : MonoBehaviour {

	Player playerInput;
	public int playerId = 0;

	public float maxSpeed;
	public float airMoveForce;
	bool facingRight = true;
	
	Animator anim;
	new Rigidbody2D rigidbody2D;

	public GameObject grenadePrefab;
	public GameObject throwingStarPrefab;
	Vector2 grenadeDistance = new Vector2(.4f, .7f);
	public float throwingForce;

	//targeting
	public GameObject directionalTarget;
	Vector2 targetDistance = new Vector2(2.5f, 3f);

	//ground
	public bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.02f;
	public LayerMask whatIsGround;

	//walls
	public bool canGrabLedge = false;
	public bool grabbingLedge = false;
	public bool touchingWall = false;
	public bool touchingRightWall = false;
	public bool touchingLeftWall = false;
	public bool wallSliding = false;
	public bool wallJumping = false;
	public Transform wallCheckLeft;
	public Transform wallCheckRight;
	public Transform ledgeCheck;
	public float wallTouchWidth;
	float timeSinceWallJump;
	public float wallJumpDuration;
	public LayerMask whatIsWall;

	//jumping/air
	public bool ableToWallJump = false;//tracking var for ghost jumping
	public float timeSinceUnableToWallJump;
	public float ghostJumpInterval;
	public float airDragMultiplier;
	public float jumpForce;
	public float jumpPushForce;
	
	//double jump
	bool doubleJumpAllowed = false;
	bool doubleJump = false;

	void Awake()
	{
		playerInput = ReInput.players.GetPlayer(playerId);
	}

	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}
	
	void FixedUpdate () {

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		touchingLeftWall = Physics2D.OverlapArea(wallCheckLeft.position, new Vector2(wallCheckLeft.position.x - wallTouchWidth, wallCheckLeft.position.y + .1f), whatIsWall);
		touchingRightWall = Physics2D.OverlapArea(wallCheckRight.position, new Vector2(wallCheckRight.position.x + wallTouchWidth, wallCheckRight.position.y + .1f), whatIsWall);
		touchingWall = touchingLeftWall || touchingRightWall;

		canGrabLedge = touchingRightWall && !Physics2D.OverlapArea(ledgeCheck.position, new Vector2(ledgeCheck.position.x + wallTouchWidth, ledgeCheck.position.y + .01f), whatIsWall);
	}

	void Update()
	{	
		float hAxis = playerInput.GetAxis ("Move Horizontal");// Input.GetAxis("Horizontal");
		float vAxis = playerInput.GetAxis ("Move Vertical");// Input.GetAxis("Vertical");
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		//anim.SetBool("Ground", grounded);

		if(anim.GetBool("Attack"))
			anim.SetBool("Attack", false);
		if(anim.GetBool("AirAttack"))
			anim.SetBool("AirAttack", false);

		if (grounded || touchingWall) 
		{
			doubleJump = false;
		}

		if(!grounded && touchingWall)
		{
			ableToWallJump = true;
		}
		else if(ableToWallJump)
		{
			timeSinceUnableToWallJump = 0f;
			ableToWallJump = false;
		}

		if(timeSinceUnableToWallJump < ghostJumpInterval)
		{
			timeSinceUnableToWallJump += Time.deltaTime;
		}

		//checking wall jump duration
		if (wallJumping) 
		{
			timeSinceWallJump += Time.deltaTime;
			if(timeSinceWallJump >= wallJumpDuration)
			{
				wallJumping = false;
			}
		}

		// Wall sliding
		if(!grounded && touchingWall && 
		   rigidbody2D.velocity.y <= 0 && (/* falling */
		   (facingRight && hAxis > 0f) || /* holding against right wall */
		   (!facingRight && hAxis < 0f))) /* holding against left wall */
		{
			rigidbody2D.gravityScale = 0.15f;
			rigidbody2D.velocity = new Vector2(0f, -.6f);
			wallSliding = true;
			anim.SetBool("WallSliding", true);
		}
		// Fall faster while holding down
		else if(!grounded && vAxis < 0f)
		{
			rigidbody2D.gravityScale = 2f;
			wallSliding = false;
			anim.SetBool("WallSliding", false);
		}
		else
		{
			rigidbody2D.gravityScale = 1f;
			wallSliding = false;
			anim.SetBool("WallSliding", false);
		}

		// If the input is moving the player right and the player is facing left...
		if((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)){
			Flip ();
		}

		/* checking inputs */
		if(canGrabLedge && ((!facingRight && hAxis < -.1) || (facingRight && hAxis > .1)))
		{
			grabbingLedge = true;
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.gravityScale = 0f;
		}
		else if(grabbingLedge)
		{
			grabbingLedge = false;
			rigidbody2D.gravityScale = 1f;
		}

		if(!playerInput.GetButton ("Shuriken") && !playerInput.GetButton ("Grenade"))
		{
			anim.SetBool("PreparingThrow", false);
			directionalTarget.SetActive(false);
		}

		Vector2 direction = new Vector2(facingRight ? 1 : -1, 0);
		if(playerInput.GetButton ("Shuriken") || playerInput.GetButton ("Grenade"))
		{
			anim.SetBool("PreparingThrow", true);

			if(grounded)//stop sliding when targeting
			{
				rigidbody2D.velocity = Vector2.zero;
			}

			if(Mathf.Abs(hAxis) > 0.3 || Mathf.Abs(vAxis) > 0.3)
			{
				direction = new Vector2(hAxis, vAxis);
			}
			setDirectionalTarget(direction);
		}
		else if(playerInput.GetButtonDown("Sword"))
		{
			if(grounded)
				anim.SetBool("Attack",true);
			else
				anim.SetBool("AirAttack",true);
		}
		else if (!wallJumping && !wallSliding && Mathf.Abs(hAxis) > 0.1)//don't want to allow an immediate force back to the wall when wall jumping
		{
			if(grounded || 
			  (!grounded && (hAxis < 0 && rigidbody2D.velocity.x > 0 || hAxis > 0 && rigidbody2D.velocity.x < 0)))//Changing velocity when moving along the ground or when in the air and changing direction
			{
				rigidbody2D.velocity = new Vector2 (hAxis * maxSpeed, rigidbody2D.velocity.y);
			}
			else //adding force when in the air and moving in the same direction
			{
				rigidbody2D.AddForce(new Vector2 (hAxis * airMoveForce, rigidbody2D.velocity.y));
			}
			anim.SetFloat("Speed", Mathf.Abs (hAxis));
		}

		if(playerInput.GetButtonUp ("Shuriken"))// Input.GetButtonDown("Shuriken"))
		{
			throwShuriken(direction);
		}
		else if(playerInput.GetButtonUp ("Grenade"))// if(Input.GetButtonDown("Grenade"))
		{
			throwGrenade(direction);
		}
		
		if(Mathf.Abs(hAxis) <= 0.1)
			anim.SetFloat("Speed", hAxis);

		if(playerInput.GetButtonDown("Jump"))
		{
			if((grounded || (!doubleJump && doubleJumpAllowed)))
			{
				Jump ();
			}
			else if(ableToWallJump || (timeSinceUnableToWallJump < ghostJumpInterval)) 
			{
				WallJump ();
			}
		}
	}

	void Jump()
	{
		//anim.SetBool("Ground", false);
		rigidbody2D.AddForce(new Vector2(0, jumpForce));

		if(!doubleJump && !grounded)
		{
			doubleJump = true;
		}
	}
 
	void WallJump () 
	{
		rigidbody2D.velocity = Vector2.zero;
		Vector2 force = new Vector2 (((facingRight && touchingRightWall) || (!facingRight && touchingLeftWall)) ? -jumpPushForce : jumpPushForce, jumpForce);
		rigidbody2D.AddForce (force);
		timeSinceWallJump = 0f;
		timeSinceUnableToWallJump = ghostJumpInterval;//setting ghost jump so you can't regular jump and then ghost jump
		wallJumping = true;
		Flip();
	}
	
	
	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		//Multiply the player's x local cale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void setDirectionalTarget(Vector2 direction)
	{
		float angle = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
		directionalTarget.transform.eulerAngles = new Vector3(directionalTarget.transform.eulerAngles.x, directionalTarget.transform.eulerAngles.y, angle);
		directionalTarget.SetActive(true);
		directionalTarget.transform.localPosition = new Vector2((facingRight ? targetDistance.x : -targetDistance.x) * direction.x, targetDistance.y * direction.y);
	}

	void throwGrenade(Vector3 direction)
	{
		GameObject grenade = Instantiate<GameObject>(grenadePrefab);
		grenade.transform.position = new Vector2(transform.position.x + (grenadeDistance.x * direction.x), transform.position.y + (grenadeDistance.y * direction.y));
		grenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x * throwingForce, direction.y * throwingForce));
	}

	void throwShuriken(Vector3 direction)
	{
		GameObject shuriken = Instantiate<GameObject>(throwingStarPrefab);
		shuriken.transform.position = new Vector2(transform.position.x + (grenadeDistance.x * direction.x), transform.position.y + (grenadeDistance.y * direction.y));
		shuriken.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x * throwingForce, direction.y * throwingForce));
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "lethal")
		{
			GetComponent<SpriteRenderer>().color = Color.red;
		}
	}
}