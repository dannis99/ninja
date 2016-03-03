using UnityEngine;
using System.Collections;
 
public class PlayerController : MonoBehaviour {
	
	public float maxSpeed = 10f;
	bool facingRight = true;
	
	Animator anim;
	Rigidbody2D rigidbody2D;
	
	//sets up the grounded stuff
	public bool grounded = false;
	public bool touchingWall = false; 
	public Transform groundCheck;
	public Transform wallCheck;
	float groundRadius = 0.02f;
	float wallTouchRadius = 0.3f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;
	public float airDragMultiplier = .8f;
	public float jumpForce = 700f;
	public float jumpPushForce = 100f;
	
	//double jump
	bool doubleJumpAllowed = false;
	bool doubleJump = false;
	
	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		touchingWall = Physics2D.OverlapCircle(wallCheck.position, wallTouchRadius, whatIsWall);
		//anim.SetBool("Ground", grounded);
		
		if (grounded) 
		{
			doubleJump = false;
		}
 
		if (touchingWall) 
		{
			doubleJump = false; 
		}
		
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		
		
		
		float move = Input.GetAxis ("Horizontal");
		
		anim.SetFloat("Speed", Mathf.Abs (move));
		
		rigidbody2D.velocity = new Vector2(grounded ? (move * maxSpeed):(move * maxSpeed * airDragMultiplier), rigidbody2D.velocity.y);
		
		// If the input is moving the player right and the player is facing left...
		if(move > 0 &&!facingRight){
			Flip ();
		}// Otherwise if the input is moving the player left and the player is facing right...
		else if(move < 0 && facingRight){
			Flip ();
		}



		// Wall sliding
		if(!grounded && touchingWall && 
		   rigidbody2D.velocity.y < 0 && (/* falling */
		   (facingRight && Input.GetAxis ("Horizontal") > 0f) || /* holding against right wall */
		   (!facingRight && Input.GetAxis ("Horizontal") < 0f))) /* holding against left wall */
		{
			rigidbody2D.gravityScale = 0.15f;
		}
		// Fall faster while holding down
		else if(!grounded && Input.GetAxis("Vertical") < 0f)
		{
			rigidbody2D.gravityScale = 2f;
		}
		else
		{
			rigidbody2D.gravityScale = 1f;
		}

		// If the jump button is pressed and the player is grounded then the player should jump.
		if((grounded || (!doubleJump && doubleJumpAllowed)) && Input.GetButtonDown("Jump"))
		{
			//anim.SetBool("Ground", false);
			rigidbody2D.AddForce(new Vector2(0, jumpForce));

			if(!doubleJump && !grounded)
			{
				doubleJump = true;
			}
		}
 
		if (!grounded && touchingWall && Input.GetButtonDown ("Jump")) 
		{
			WallJump ();
		}
	}

	void Update()
	{   
		
		
	}
 
	void WallJump () 
	{
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.AddForce (new Vector2 (facingRight? -jumpPushForce : jumpPushForce, jumpForce));
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
}