using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerController : MonoBehaviour {

	public ShieldController shieldController;

	Player playerInput;
	public int playerId = 0;
	public string playerColor;
	public SpriteRenderer headRenderer;
	public SpriteRenderer bodyRenderer;
	public Sprite redHead;
	public Sprite redBody;
	public Sprite blueHead;
	public Sprite blueBody;
	public Sprite greenHead;
	public Sprite greenBody;
	public Sprite yellowHead;
	public Sprite yellowBody;

	bool dead;

	public float maxSpeed;
	public float attackThrustSpeed;
	public float timeBetweenAttacks;
	public float attackDuration;
	float timeSinceAttack = 0f;
	bool ableToAttack = true;

	bool dashing;
	bool ableToDash = true;
	public float dashSpeed;
	public float timeBetweenDashes;
	public float dashDuration;
	float timeSinceDash = 0f;
	//Vector2 preDashVelocity;
	public float airMoveForce;
	bool facingRight = true;
	float hAxis;
	float vAxis;
	
	Animator anim;
	Rigidbody2D playerRigidbody2D;

	public GameObject grenadePrefab;
	public GameObject throwingStarPrefab;
	Vector2 weaponDistance = new Vector2(.8f, .7f);
	public float grenadeThrowingForce;
	public float shurikenVelocity;

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
	int maxGrenades = 3;
	int grenadeCount;
	int maxShurikens = 3;
	int shurikenCount;

	//particleEffects
	public ParticleSystem slideSmoke;

	void Awake()
	{
		playerInput = ReInput.players.GetPlayer(playerId);

		if(playerColor == "red")
		{
			bodyRenderer.sprite = redBody;
			headRenderer.sprite = redHead;
			shieldController.firstColor = new Color (0f, .78f, .6f, .7f);
			shieldController.secondColor = new Color (0f, .78f, .6f, 0f);
		}
		else if(playerColor == "blue")
		{
			bodyRenderer.sprite = blueBody;
			headRenderer.sprite = blueHead;
			shieldController.firstColor = new Color (.78f, .6f, 0f, 1f);
			shieldController.secondColor = new Color (.78f, .6f, 0f, .3f);
		}
		else if(playerColor == "green")
		{
			bodyRenderer.sprite = greenBody;
			headRenderer.sprite = greenHead;
			shieldController.firstColor = new Color (.78f, 0f, .6f, .7f);
			shieldController.secondColor = new Color (.78f, 0f, .6f, 0f);
		}
		else if(playerColor == "yellow")
		{
			bodyRenderer.sprite = yellowBody;
			headRenderer.sprite = yellowHead;
			shieldController.firstColor = new Color (0f, .7f, .2f, .7f);
			shieldController.secondColor = new Color (0f, .7f, .2f, 0f);
		}
	}

	void Start () {
		RigidbodyConstraints2D rc2D = GetComponent<Rigidbody2D>().constraints;
		DestroyImmediate(GetComponent<Rigidbody2D>());
		playerRigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		playerRigidbody2D.constraints = rc2D;
		anim = GetComponent<Animator>();
		grenadeCount = maxGrenades;
		shurikenCount = maxShurikens;
	}
	
	void FixedUpdate () {
		if (!dead) 
		{
			// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
			touchingLeftWall = Physics2D.OverlapArea (wallCheckLeft.position, new Vector2 (wallCheckLeft.position.x - wallTouchWidth, wallCheckLeft.position.y + .1f), whatIsWall);
			touchingRightWall = Physics2D.OverlapArea (wallCheckRight.position, new Vector2 (wallCheckRight.position.x + wallTouchWidth, wallCheckRight.position.y + .1f), whatIsWall);
			touchingWall = touchingLeftWall || touchingRightWall;

			canGrabLedge = touchingRightWall && !Physics2D.OverlapArea (ledgeCheck.position, new Vector2 (ledgeCheck.position.x + wallTouchWidth, ledgeCheck.position.y + .01f), whatIsWall);
		}
	}

	void Update()
	{
		if (dead)
		{
			anim.SetTrigger ("Death");
		}
		else
		{
			hAxis = playerInput.GetAxis ("Move Horizontal");
			vAxis = playerInput.GetAxis ("Move Vertical");
			anim.SetFloat ("vSpeed", playerRigidbody2D.velocity.y);
			if (Mathf.Abs (hAxis) <= 0.1)
				anim.SetFloat ("Speed", hAxis);
			//anim.SetBool("Ground", grounded);
			// If the input is moving the player right and the player is facing left...
			if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
				Flip ();
			}

			if (anim.GetBool ("Attack"))
				anim.SetBool ("Attack", false);
			if (anim.GetBool ("AirAttack"))
				anim.SetBool ("AirAttack", false);

			if (timeSinceAttack <= (attackDuration + timeBetweenAttacks)) {
				timeSinceAttack += Time.deltaTime;
			}

			if (!ableToAttack && timeSinceAttack >= (attackDuration + timeBetweenAttacks)) {
				ableToAttack = true;
			}

			CheckAbilityToJump ();

			CheckDash ();

			//checking wall jump duration
			if (wallJumping) {
				timeSinceWallJump += Time.deltaTime;
				if (timeSinceWallJump >= wallJumpDuration) {
					wallJumping = false;
				}
			}

			// Fall faster while holding down
			if (!grounded && !wallSliding && vAxis < -0.5f) {
				playerRigidbody2D.gravityScale = 2f;
				////Debug.Log("setting gravity in fall");
				if (playerInput.GetButton ("Sword")) {
					anim.SetBool ("DownAttack", true);
				}
			} else {
				playerRigidbody2D.gravityScale = 1f;
				////Debug.Log("setting gravity back from fall");
				anim.SetBool ("DownAttack", false);
			}

			CheckLedgeGrab (hAxis);
			if (!grabbingLedge)
				CheckWallSlide (hAxis);

			//duck
			if (grounded && vAxis < -0.5f && Mathf.Abs (hAxis) < .3f) {
				anim.SetBool ("Ducking", true);
			} else {
				anim.SetBool ("Ducking", false);
			}

			//looking up
			if (grounded && vAxis > 0.5f && Mathf.Abs (hAxis) < .3f) {
				anim.SetBool ("LookingUp", true);
			} else {
				anim.SetBool ("LookingUp", false);
			}

			/* checking inputs */

			if (!playerInput.GetButton ("Shuriken") && !playerInput.GetButton ("Grenade")) {
				anim.SetBool ("PreparingThrow", false);
				directionalTarget.SetActive (false);
			}

			if (playerInput.GetButton ("Shuriken") || playerInput.GetButton ("Grenade")) {
				anim.SetBool ("PreparingThrow", true);
				if (targetDirection == Vector2.zero)
					targetDirection = new Vector2 (facingRight ? 1 : -1, .5f);

				if (grounded) {//stop sliding when targeting
					playerRigidbody2D.velocity = Vector2.zero;
				}

				if (Mathf.Abs (hAxis) + Mathf.Abs (vAxis) > 1.0f) {
					targetDirection = new Vector2 (hAxis, .5f + (vAxis / 2f));//setting y to a 0 to 1 range instead of -1 to 1
				}
				setDirectionalTarget (targetDirection, Mathf.Atan2 (vAxis, Mathf.Abs (hAxis)) * Mathf.Rad2Deg);
			} else if (playerInput.GetButtonDown ("Sword") && ableToAttack) {
				ableToAttack = false;
				timeSinceAttack = 0f;
				if (grounded) {
					if(Mathf.Abs(hAxis) > .3f)
					{
						anim.SetTrigger("Attack");
						dashing = true;
						ableToDash = false;
						timeSinceDash = 0f;
						playerRigidbody2D.AddForce (new Vector2((facingRight)?attackThrustSpeed:-attackThrustSpeed, 0), ForceMode2D.Impulse);
					}
					else
					{
						anim.SetBool ("Attack", true);
					}
				} else {
					anim.SetBool ("AirAttack", true);
				}
				//rigidbody2D.AddForce (new Vector2 (facingRight ? attackThrustSpeed : -attackThrustSpeed, 0), ForceMode2D.Impulse);
			} else if (playerInput.GetButtonDown ("Dash") && ableToDash) {
				dashing = true;
				gameObject.layer = LayerMask.NameToLayer("Dodging Character");
				if (grounded)
					anim.SetBool ("Rolling", true);
				else
					anim.SetBool ("Dashing", true);
				ableToDash = false;
				timeSinceDash = 0f;
				//preDashVelocity = rigidbody2D.velocity;
				float xDashForce = dashSpeed;
				if (!facingRight)
					xDashForce = -dashSpeed;
				if (Mathf.Abs (vAxis) > .1f && Mathf.Abs (hAxis) < .1f)
					xDashForce = 0;

				float yDashForce = 0;
				if (Mathf.Abs (vAxis) > .1f)
					yDashForce = (vAxis > .1f) ? dashSpeed : -dashSpeed;

				//gotta set the combination of the forces to to the total dash force
				if (xDashForce > 0)
					xDashForce = xDashForce / (Mathf.Abs (xDashForce) + Mathf.Abs (yDashForce)) * dashSpeed;

				if (yDashForce > 0)
					yDashForce = yDashForce / (Mathf.Abs (xDashForce) + Mathf.Abs (yDashForce)) * dashSpeed;

				Vector2 dashForce = new Vector2 (xDashForce, yDashForce);
				playerRigidbody2D.AddForce (dashForce, ForceMode2D.Impulse);
			} else if (Mathf.Abs (hAxis) > 0.3 && !wallJumping && !wallSliding && !dashing) {//checking if we are going to allow side to side force or velocity changes
				if (grounded || //walking or running
				   (!grounded && (hAxis < 0 && playerRigidbody2D.velocity.x > 0 || hAxis > 0 && playerRigidbody2D.velocity.x < 0))) {//Changing velocity when moving along the ground or when in the air and changing direction
					playerRigidbody2D.velocity = new Vector2 (hAxis * maxSpeed, playerRigidbody2D.velocity.y);
				} else { //adding force when in the air and moving in the same direction
					playerRigidbody2D.AddForce (new Vector2 (hAxis * airMoveForce, playerRigidbody2D.velocity.y));
				}
				anim.SetFloat ("Speed", Mathf.Abs (hAxis));
			}

			if (playerInput.GetButtonUp ("Shuriken")) {// Input.GetButtonDown("Shuriken"))
				if (shurikenCount > 0) {
					throwShuriken (targetDirection);
				}
				targetDirection = Vector2.zero;
			} else if (playerInput.GetButtonUp ("Grenade")) {// if(Input.GetButtonDown("Grenade"))
				if (grenadeCount > 0) {
					throwGrenade (targetDirection);
				}
				targetDirection = Vector2.zero;
			}

			if (playerInput.GetButtonDown ("Jump")) {
				tryToJump ();
			}
		}
	}

	void updateGrenadeSprites()
	{
		for(int i = 0; i < grenadeSprites.Length; i++)
		{
			if(grenadeCount > i)
			{
				grenadeSprites [i].enabled = true;
			}
			else
			{
				grenadeSprites [i].enabled = false;
			}
		}					
	}

	void updateShurikenSprites()
	{
		for(int i = 0; i < shurikenSprites.Length; i++)
		{
			if(shurikenCount > i)
			{
				shurikenSprites [i].enabled = true;
			}
			else
			{
				shurikenSprites [i].enabled = false;
			}
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
			gameObject.layer = LayerMask.NameToLayer("Character");
			playerRigidbody2D.velocity = Vector2.zero;//preDashVelocity;
		}
		if (!ableToDash && timeSinceDash >= (dashDuration + timeBetweenDashes)) {
			ableToDash = true;
		}
	}

	void CheckLedgeGrab (float hAxis)
	{
		if (canGrabLedge && ((!facingRight && hAxis < -.1) || (facingRight && hAxis > .1))) {
			grabbingLedge = true;
			anim.SetBool ("LedgeGrab", true);
			playerRigidbody2D.velocity = Vector2.zero;
			playerRigidbody2D.gravityScale = 0f;
			////Debug.Log("setting gravity in check ledge");
		}
		else
			if (grabbingLedge) {
				grabbingLedge = false;
				anim.SetBool ("LedgeGrab", false);
				playerRigidbody2D.gravityScale = 1f;
				////Debug.Log("setting gravity in check ledge 2");
			}
	}

	void CheckWallSlide (float hAxis)
	{
		if (!grounded && touchingRightWall && playerRigidbody2D.velocity.y <= 0 && (/* falling */(facingRight && hAxis > 0f) || /* holding against right wall */(!facingRight && hAxis < 0f)))/* holding against left wall */ {
			playerRigidbody2D.gravityScale = 0.15f;
			////Debug.Log("setting gravity in wall slide");
			playerRigidbody2D.velocity = new Vector2 (0f, -1f);
			wallSliding = true;

			anim.SetBool ("WallSliding", true);
			if(!slideSmoke.isPlaying)
				slideSmoke.Play();
		}
		else {
			wallSliding = false;
			anim.SetBool ("WallSliding", false);
			if(slideSmoke.isPlaying)
				slideSmoke.Stop();
		}
	}

	void tryToJump()
	{
		if((grounded || (!doubleJump && doubleJumpAllowed)))
		{
			//anim.SetBool("Ground", false);
			playerRigidbody2D.AddForce(new Vector2(0, jumpForce));

			if(!doubleJump && !grounded)
			{
				doubleJump = true;
			}
		}
		else if(ableToWallJump || (timeSinceUnableToWallJump < ghostJumpInterval)) 
		{
			playerRigidbody2D.velocity = Vector2.zero;
			Vector2 force = new Vector2 (((facingRight && touchingRightWall) || (!facingRight && touchingLeftWall)) ? -jumpPushForce : jumpPushForce, jumpForce);
			if(grabbingLedge)
			{
				force = new Vector2(force.x*1.5f, force.y*1.1f);
			}
			playerRigidbody2D.AddForce (force);
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
		grenadeCount--;
		updateGrenadeSprites();
		anim.SetTrigger ("Throwing");
		GameObject grenade = Instantiate<GameObject>(grenadePrefab);
		float xForce = 0;
		if(Mathf.Abs(hAxis) > .3f)
			xForce = (facingRight)?grenadeThrowingForce:-grenadeThrowingForce;

		float yForce = 0;
		if(Mathf.Abs(vAxis) > .3f)
		{
			yForce = grenadeThrowingForce;
		}
		Vector2 force = new Vector2(xForce, yForce);
		if(force == Vector2.zero)
			force = new Vector2((facingRight)?grenadeThrowingForce:-grenadeThrowingForce, yForce);

		grenade.transform.position = getWeaponPosition(direction);
		grenade.GetComponent<Rigidbody2D>().AddForce(force);
	}

	void throwShuriken(Vector3 direction)
	{
		shurikenCount--;
		updateShurikenSprites();
		anim.SetTrigger ("Throwing");
		GameObject shuriken = Instantiate<GameObject>(throwingStarPrefab);
		float xVelocity = 0;
		if(Mathf.Abs(hAxis) > .3f)
			xVelocity = (facingRight)?shurikenVelocity:-shurikenVelocity;

		float yVelocity = 0;
		if(Mathf.Abs(vAxis) > .3f)
		{
			yVelocity = shurikenVelocity;
		}
		Vector2 velocity = new Vector2(xVelocity, yVelocity);
		if(velocity == Vector2.zero)
			velocity = new Vector2((facingRight)?shurikenVelocity:-shurikenVelocity, yVelocity);

		shuriken.transform.position = getWeaponPosition(direction);
		////Debug.Log("player position: "+transform.position);
		////Debug.Log("shuriken position: "+shuriken.transform.position);
		////Debug.Log("shuriken velocity: "+velocity);
		shuriken.GetComponent<ShurikenController>().setVelocity(velocity);
	}

	Vector2 getWeaponPosition(Vector2 direction)
	{
		float xPosition = transform.position.x + (weaponDistance.x * direction.x);
		float yPosition = transform.position.y + (weaponDistance.y * direction.y);
		if(vAxis > 0.3f)
			yPosition += weaponDistance.y;
		else if(vAxis < -.3f)
			yPosition -= weaponDistance.y;
		
		return new Vector2(xPosition, yPosition);
	}

	public void takeDamage()
	{
		if(shieldController.shielded)
		{
			shieldController.shielded = false;
		}
		else
		{
			dead = true;
			anim.SetTrigger ("Death");
			gameObject.layer = LayerMask.NameToLayer("Dodging Character");
			foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
			{
				renderer.sortingOrder += 1000;
			}
		}
	}

	public void setItem(GameObject item)
	{
		if(item.GetComponent<GrenadeController>() != null)
		{
			grenadeCount = maxGrenades;
			updateGrenadeSprites();
		}
		else if(item.GetComponent<ShurikenController>() != null)
		{
			shurikenCount = maxShurikens;
			updateShurikenSprites();
		}
	}
}