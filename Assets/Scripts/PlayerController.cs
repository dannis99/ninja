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
	
	public bool grounded = false;
	public bool touchingWall = false;
	public bool wallSliding = false;
	public bool wallJumping = false;
	public Transform groundCheck;
	public Transform wallCheckLeft;
	public Transform wallCheckRight;
	float groundRadius = 0.02f;
	public float wallTouchWidth = 1f;
	float timeSinceWallJump = 0f;
	public float wallJumpDuration = 1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;
	public float airDragMultiplier = .8f;
	public float jumpForce = 700f;
	public float jumpPushForce = 100f;
	
	//double jump
	bool doubleJumpAllowed = false;
	bool doubleJump = false;
	
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}
	
	void FixedUpdate () {

		if(Input.anyKeyDown)
		{
			print(Input.inputString);
		}

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		touchingWall = Physics2D.OverlapArea(wallCheckLeft.position, new Vector2(wallCheckLeft.position.x - wallTouchWidth, wallCheckLeft.position.y + .1f), whatIsWall) || 
					   Physics2D.OverlapArea(wallCheckRight.position, new Vector2(wallCheckRight.position.x + wallTouchWidth, wallCheckRight.position.y + .1f), whatIsWall);
		float move = Input.GetAxis ("Horizontal");
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		anim.SetFloat("Speed", Mathf.Abs (move));
		//anim.SetBool("Ground", grounded);
		
		if (grounded) 
		{
			doubleJump = false;
		}
 
		if (touchingWall) 
		{
			doubleJump = false; 
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

		if (!wallJumping && !wallSliding && Mathf.Abs(move) > 0)//don't want to allow an immediate force back to the wall when wall jumping
		{
			rigidbody2D.velocity = new Vector2 (grounded ? (move * maxSpeed) : (move * maxSpeed * airDragMultiplier), rigidbody2D.velocity.y);
		}
		
		// If the input is moving the player right and the player is facing left...
		if((move > 0 && !facingRight) || (move < 0 && facingRight)){
			Flip ();
		}

		// If the jump button is pressed and the player is grounded then the player should jump.
		if((grounded || (!doubleJump && doubleJumpAllowed)) && Input.GetButtonDown("Jump"))
		{
			Jump ();
		}
 
		if (!grounded && touchingWall && Input.GetButtonDown ("Jump")) 
		{
			WallJump ();
		}
	}

	void Update()
	{   
		
		
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
		rigidbody2D.AddForce (new Vector2 (facingRight? -jumpPushForce : jumpPushForce, jumpForce));
		timeSinceWallJump = 0f;
		wallJumping = true;
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