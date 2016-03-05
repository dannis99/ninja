using UnityEngine;
using System.Collections;

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
	
	public float maxSpeed = 10f;
	bool facingRight = true;
	
	Animator anim;
	Rigidbody2D rigidbody2D;

	//targeting
	public GameObject directionalTarget;
	float xTargetDistance = 2.5f;
	float yTargetDistance = 3f;

	//ground
	public bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.02f;
	public LayerMask whatIsGround;

	//walls
	public bool touchingWall = false;
	public bool touchingRightWall = false;
	public bool touchingLeftWall = false;
	public bool wallSliding = false;
	public bool wallJumping = false;
	public Transform wallCheckLeft;
	public Transform wallCheckRight;
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
		float move = Input.GetAxis ("Horizontal");
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		anim.SetFloat("Speed", Mathf.Abs (move));
		//anim.SetBool("Ground", grounded);
		
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

		if(timeSinceUnableToWallJump <= ghostJumpInterval)
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
		   (facingRight && Input.GetAxis ("Horizontal") > 0f) || /* holding against right wall */
		   (!facingRight && Input.GetAxis ("Horizontal") < 0f))) /* holding against left wall */
		{
			rigidbody2D.gravityScale = 0.15f;
			wallSliding = true;
			anim.SetBool("WallSliding", true);
		}
		// Fall faster while holding down
		else if(!grounded && Input.GetAxis("Vertical") < 0f)
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
		if((move > 0 && !facingRight) || (move < 0 && facingRight)){
			Flip ();
		}

		/* checking inputs */

		if(Input.GetAxis("Weapon") > 0.2 || Input.GetAxis("Grenade") > 0.2)
		{
			if(Mathf.Abs(move) > 0.3 || Mathf.Abs(Input.GetAxis("Vertical")) > 0.3)
			{
				setDirectionalTarget(new Vector2(move, Input.GetAxis("Vertical")));
			}
			else
			{
				directionalTarget.SetActive(false);
			}
		}
		else if (!wallJumping && !wallSliding && Mathf.Abs(move) > 0.1)//don't want to allow an immediate force back to the wall when wall jumping
		{
			directionalTarget.SetActive(false);
			rigidbody2D.velocity = new Vector2 (grounded ? (move * maxSpeed) : (move * maxSpeed * airDragMultiplier), rigidbody2D.velocity.y);
		}
		else
		{
			directionalTarget.SetActive(false);
		}

		if(Input.GetButtonDown("Jump"))
		{
			if((grounded || (!doubleJump && doubleJumpAllowed)))
			{
				Jump ();
			}
			else if(ableToWallJump || timeSinceUnableToWallJump <= ghostJumpInterval) 
			{
				WallJump ();
			}
		}
	}

	void Update()
	{   
		
		
	}

	void setDirectionalTarget(Vector2 direction)
	{
		float angle = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
		Debug.Log(angle);
		directionalTarget.transform.eulerAngles = new Vector3(directionalTarget.transform.eulerAngles.x, directionalTarget.transform.eulerAngles.y, angle);
		directionalTarget.SetActive(true);
		directionalTarget.transform.localPosition = new Vector2((facingRight ? xTargetDistance : -xTargetDistance) * direction.x, yTargetDistance * direction.y);
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "lethal")
		{
			GetComponent<SpriteRenderer>().color = Color.red;
		}
	}
}