using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerController : MonoBehaviour {

	Player playerInput;
	public int playerId = 0;

	public float maxSpeed;
	public float attackThrustSpeed;
	bool dashing;
	bool ableToDash = true;
	public float dashSpeed;
	public float timeBetweenDashes;
	public float dashDuration;
	float timeSinceDash = 0f;
	Vector2 preDashVelocity;
	public float airMoveForce;
	bool facingRight = true;
	float hAxis;
	float vAxis;
	
	Animator anim;
	new Rigidbody2D rigidbody2D;

	public GameObject grenadePrefab;
	public GameObject throwingStarPrefab;
	Vector2 grenadeDistance = new Vector2(.4f, .7f);
	public float throwingForce;

	//targeting
	public GameObject directionalTarget;
	Vector2 targetDistance = new Vector2(.5f, 1.25f);
	Vector2 targetDirection = Vector2.zero;

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

	//weapons
	public SpriteRenderer[] grenadeSprites;
	public SpriteRenderer[] shurikenSprites;
	int grenadeCount = 3;
	int shurikenCount = 3;

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
		hAxis = playerInput.GetAxis ("Move Horizontal");
		vAxis = playerInput.GetAxis ("Move Vertical");
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
		if(Mathf.Abs(hAxis) <= 0.1)
			anim.SetFloat("Speed", hAxis);
		//anim.SetBool("Ground", grounded);
		// If the input is moving the player right and the player is facing left...
		if((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)){
			Flip ();
		}

		if(anim.GetBool("Attack"))
			anim.SetBool("Attack", false);
		if(anim.GetBool("AirAttack"))
			anim.SetBool("AirAttack", false);

		CheckAbilityToJump();

		CheckDash();

		//checking wall jump duration
		if (wallJumping) 
		{
			timeSinceWallJump += Time.deltaTime;
			if(timeSinceWallJump >= wallJumpDuration)
			{
				wallJumping = false;
			}
		}

		CheckWallSlide(hAxis);

		// Fall faster while holding down
		if(!grounded && !wallSliding && vAxis < -0.5f)
		{
			rigidbody2D.gravityScale = 2f;
		}
		else
		{
			rigidbody2D.gravityScale = 1f;
		}

		/* checking inputs */
		CheckLedgeGrab (hAxis);

		if(!playerInput.GetButton ("Shuriken") && !playerInput.GetButton ("Grenade"))
		{
			anim.SetBool("PreparingThrow", false);
			directionalTarget.SetActive(false);
		}

		if(targetDirection == Vector2.zero)
			targetDirection = new Vector2(facingRight ? 1 : -1, .5f);

		if(playerInput.GetButton ("Shuriken") || playerInput.GetButton ("Grenade"))
		{
			anim.SetBool("PreparingThrow", true);

			if(grounded)//stop sliding when targeting
			{
				rigidbody2D.velocity = Vector2.zero;
			}

			if(Mathf.Abs(hAxis) + Mathf.Abs(vAxis) > 1.0f)
			{
				targetDirection = new Vector2(hAxis, .5f + (vAxis/2f));//setting y to a 0 to 1 range instead of -1 to 1
			}
			setDirectionalTarget(targetDirection, Mathf.Atan2(vAxis, Mathf.Abs(hAxis)) * Mathf.Rad2Deg);
		}
		else if(playerInput.GetButtonDown("Sword"))
		{
			if(grounded)
			{
				anim.SetBool("Attack",true);
			}
			else
			{
				anim.SetBool("Attack",true);
			}
			rigidbody2D.AddForce(new Vector2(facingRight ? attackThrustSpeed : -attackThrustSpeed, 0), ForceMode2D.Impulse);
		}
		else if(playerInput.GetButtonDown("Dash") && ableToDash)
		{
			dashing = true;
			if(grounded)
				anim.SetBool("Rolling", true);
			else
				anim.SetBool("Dashing", true);
			ableToDash = false;
			timeSinceDash = 0f;
			preDashVelocity = rigidbody2D.velocity;
			float xDashForce = dashSpeed;
			if (!facingRight)
				xDashForce = -dashSpeed;
			if (Mathf.Abs(vAxis) > .1f && Mathf.Abs (hAxis) < .1f)
				xDashForce = 0;

			float yDashForce = 0;
			if (Mathf.Abs (vAxis) > .1f)
				yDashForce = (vAxis > .1f)?dashSpeed:-dashSpeed;
			
			Vector2 dashForce = new Vector2(xDashForce, yDashForce);
			rigidbody2D.AddForce(dashForce, ForceMode2D.Impulse);
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
			if(shurikenCount > 0)
			{
				shurikenSprites[shurikenCount-1].enabled = false;
				shurikenCount--;
				throwShuriken(targetDirection);
				targetDirection = Vector2.zero;
			}
		}
		else if(playerInput.GetButtonUp ("Grenade"))// if(Input.GetButtonDown("Grenade"))
		{
			if(grenadeCount > 0)
			{
				grenadeSprites[grenadeCount-1].enabled = false;
				grenadeCount--;
				throwGrenade(targetDirection);
				targetDirection = Vector2.zero;
			}
		}

		if(playerInput.GetButtonDown("Jump"))
		{
			tryToJump();
		}
	}

	void CheckAbilityToJump ()
	{
		if (grounded || touchingWall) {
			doubleJump = false;
		}

		if (!grounded && touchingWall) {
			ableToWallJump = true;
		}
		else if (ableToWallJump) {
			timeSinceUnableToWallJump = 0f;
			ableToWallJump = false;
		}

		if(timeSinceUnableToWallJump < ghostJumpInterval)
		{
			timeSinceUnableToWallJump += Time.deltaTime;
		}
	}

	void CheckDash ()
	{
		if (timeSinceDash <= (dashDuration + timeBetweenDashes)) {
			timeSinceDash += Time.deltaTime;
		}
		if (dashing && timeSinceDash >= dashDuration) {
			anim.SetBool("Dashing", false);
			anim.SetBool("Rolling", false);
			dashing = false;
			rigidbody2D.velocity = Vector2.zero;//preDashVelocity;
		}
		if (!ableToDash && timeSinceDash >= (dashDuration + timeBetweenDashes)) {
			ableToDash = true;
		}
	}

	void CheckLedgeGrab (float hAxis)
	{
		if (canGrabLedge && ((!facingRight && hAxis < -.1) || (facingRight && hAxis > .1))) {
			grabbingLedge = true;
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.gravityScale = 0f;
		}
		else
			if (grabbingLedge) {
				grabbingLedge = false;
				rigidbody2D.gravityScale = 1f;
			}
	}

	void CheckWallSlide (float hAxis)
	{
		if (!grounded && touchingRightWall && rigidbody2D.velocity.y <= 0 && (/* falling */(facingRight && hAxis > 0f) || /* holding against right wall */(!facingRight && hAxis < 0f)))/* holding against left wall */ {
			rigidbody2D.gravityScale = 0.15f;
			rigidbody2D.velocity = new Vector2 (0f, -1f);
			wallSliding = true;
			anim.SetBool ("WallSliding", true);
		}
		else {
			wallSliding = false;
			anim.SetBool ("WallSliding", false);
		}
	}

	void tryToJump()
	{
		if((grounded || (!doubleJump && doubleJumpAllowed)))
		{
			//anim.SetBool("Ground", false);
			rigidbody2D.AddForce(new Vector2(0, jumpForce));

			if(!doubleJump && !grounded)
			{
				doubleJump = true;
			}
		}
		else if(ableToWallJump || (timeSinceUnableToWallJump < ghostJumpInterval)) 
		{
			rigidbody2D.velocity = Vector2.zero;
			Vector2 force = new Vector2 (((facingRight && touchingRightWall) || (!facingRight && touchingLeftWall)) ? -jumpPushForce : jumpPushForce, jumpForce);
			rigidbody2D.AddForce (force);
			timeSinceWallJump = 0f;
			timeSinceUnableToWallJump = ghostJumpInterval;//setting ghost jump so you can't regular jump and then ghost jump
			wallJumping = true;
			Flip();
		}
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

	void setDirectionalTarget(Vector2 direction, float angle)
	{
		directionalTarget.transform.eulerAngles = new Vector3(directionalTarget.transform.eulerAngles.x, directionalTarget.transform.eulerAngles.y, angle);
		directionalTarget.SetActive(true);
		directionalTarget.transform.localPosition = new Vector2((facingRight ? targetDistance.x : -targetDistance.x) * direction.x, targetDistance.y * direction.y);
	}

	void throwGrenade(Vector3 direction)
	{
		GameObject grenade = Instantiate<GameObject>(grenadePrefab);
		float xForce = ((Mathf.Abs(hAxis) > .1f)?hAxis:(facingRight)?1f:-1f) * throwingForce;
		float yForce = ((Mathf.Abs(vAxis) > .1f)?vAxis:0) * throwingForce; 
		grenade.transform.position = new Vector2(transform.position.x + (grenadeDistance.x * direction.x), transform.position.y + .5f + (grenadeDistance.y * direction.y));
		grenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(xForce, yForce));
	}

	void throwShuriken(Vector3 direction)
	{
		GameObject shuriken = Instantiate<GameObject>(throwingStarPrefab);
		float xForce = ((Mathf.Abs(hAxis) > .1f)?hAxis:(facingRight)?1f:-1f) * throwingForce;
		float yForce = ((Mathf.Abs(vAxis) > .1f)?vAxis:0) * throwingForce; 
		shuriken.transform.position = new Vector2(transform.position.x + (grenadeDistance.x * direction.x), transform.position.y + .5f + (grenadeDistance.y * direction.y));
		shuriken.GetComponent<Rigidbody2D>().AddForce(new Vector2(xForce, yForce));
	}

	public void takeDamage()
	{
		foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
		{
			renderer.color = Color.red;
		}
	}
//
//	void OnCollisionEnter2D(Collision2D collision)
//	{
//		if(collision.gameObject.tag == "lethal")
//		{
//			GetComponent<SpriteRenderer>().color = Color.red;
//		}
//	}
}