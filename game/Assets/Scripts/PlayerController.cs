using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using System;

public class PlayerController : MonoBehaviour, ISlowable {
    //public static string ANIM_GROUND = "Ground";
    public static string ANIM_ATTACK = "Attack";
    public static string ANIM_AIR_ATTACK = "AirAttack";
    public static string ANIM_DOWN_ATTACK = "DownAttack";
    public static string ANIM_DUCKING = "Ducking";
    public static string ANIM_LOOKING_UP = "LookingUp";
    public static string ANIM_PREPARING_THROW = "PreparingThrow";
    public static string ANIM_ROLLING = "Rolling";
    public static string ANIM_DASHING = "Dashing";
    public static string ANIM_DASH_UPWARD = "DashUpward";
    public static string ANIM_LEDGE_GRAB = "LedgeGrab";
    public static string ANIM_WALL_SLIDING = "WallSliding";

    private List<string> animParams = new List<string> {
        //ANIM_GROUND,
        ANIM_DOWN_ATTACK,
        ANIM_DUCKING,
        ANIM_LOOKING_UP,
        ANIM_PREPARING_THROW,
        ANIM_ATTACK,
        ANIM_AIR_ATTACK,
        ANIM_ROLLING,
        ANIM_DASHING,
        ANIM_DASH_UPWARD,
        ANIM_LEDGE_GRAB,
        ANIM_WALL_SLIDING
    };


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
    Vector2 dashToward = Vector2.zero;
	//Vector2 preDashVelocity;
	public float airMoveForce;
	bool facingRight = true;
	float hAxis;
	float vAxis;
	
	Animator anim;
	Rigidbody2D playerRigidbody2D;

    public GameObject grenadePrefab;
    public GameObject shurikenPrefab;
    public List<GameObject> weaponPrefabs;
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
	public LayerMask whatIsGround;

	//walls
	public BoxCollider2D ledgeCollider;
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

    //platform stuff
    public GameObject platform;
    public Vector3 platformPreviousPosition;

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
			grounded = Physics2D.OverlapArea (new Vector2 (groundCheck.position.x - .125f, groundCheck.position.y), new Vector2 (groundCheck.position.x + .125f, groundCheck.position.y - .1f), whatIsGround);
			touchingLeftWall = Physics2D.OverlapArea (wallCheckLeft.position, new Vector2 (wallCheckLeft.position.x - wallTouchWidth, wallCheckLeft.position.y + .1f), whatIsWall);
			touchingRightWall = Physics2D.OverlapArea (wallCheckRight.position, new Vector2 (wallCheckRight.position.x + wallTouchWidth, wallCheckRight.position.y + .1f), whatIsWall);
			touchingWall = touchingLeftWall || touchingRightWall;

            if(grounded)
            {
                RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector3.down, .1f);
                if(hit.collider != null && hit.collider.gameObject.GetComponent<FloatingPlatform>() != null)
                {
                    if (platform == null)
                    {
                        platform = hit.collider.gameObject;
                        platformPreviousPosition = Vector3.zero;
                    }                        
                }
                else
                {
                    platform = null;
                }
            }
            else
            {
                platform = null;
            }

            if(platform != null)
            {
                Vector3 platformDelta = Vector3.zero;
                if (platformPreviousPosition != Vector3.zero)
                    platformDelta = platform.transform.position - platformPreviousPosition;
                transform.position += platformDelta;
                platformPreviousPosition = platform.transform.position;
            }

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
			//anim.SetBool(ANIM_GROUND, grounded);
			// If the input is moving the player right and the player is facing left...
			if ((hAxis > 0 && !facingRight) || (hAxis < 0 && facingRight)) {
				Flip ();
			}

			if (anim.GetBool (ANIM_ATTACK))
				anim.SetBool (ANIM_ATTACK, false);
			if (anim.GetBool (ANIM_AIR_ATTACK))
				anim.SetBool (ANIM_AIR_ATTACK, false);

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
					anim.SetBool (ANIM_DOWN_ATTACK, true);
				}
			} else {
				playerRigidbody2D.gravityScale = 1f;
				////Debug.Log("setting gravity back from fall");
				anim.SetBool (ANIM_DOWN_ATTACK, false);
			}

			CheckLedgeGrab (hAxis);
			if (!grabbingLedge)
				CheckWallSlide (hAxis);

			//duck
			if (grounded && vAxis < -0.5f && Mathf.Abs (hAxis) < .3f) {
				anim.SetBool (ANIM_DUCKING, true);
			} else {
				anim.SetBool (ANIM_DUCKING, false);
			}

			//looking up
			if (grounded && vAxis > 0.5f && Mathf.Abs (hAxis) < .3f) {
				anim.SetBool (ANIM_LOOKING_UP, true);
			} else {
				anim.SetBool (ANIM_LOOKING_UP, false);
			}

			/* checking inputs */

			if (!playerInput.GetButton ("Shuriken") && !playerInput.GetButton ("Grenade")) {
				anim.SetBool (ANIM_PREPARING_THROW, false);
				directionalTarget.SetActive (false);
			}

			if (playerInput.GetButton ("Shuriken") || playerInput.GetButton ("Grenade")) {
				anim.SetBool (ANIM_PREPARING_THROW, true);
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
						anim.SetBool (ANIM_ATTACK, true);
					}
				} else {
					anim.SetBool (ANIM_AIR_ATTACK, true);
				}
				//rigidbody2D.AddForce (new Vector2 (facingRight ? attackThrustSpeed : -attackThrustSpeed, 0), ForceMode2D.Impulse);
			} else if (playerInput.GetButtonDown ("Dash") && ableToDash) {
				dashing = true;
                playerRigidbody2D.isKinematic = true;
                setSpriteOpacity(.6f);
				gameObject.layer = LayerMask.NameToLayer("Dodging Character");
				
				ableToDash = false;
				timeSinceDash = 0f;
                //preDashVelocity = rigidbody2D.velocity;
                float yDashForce = 0;
				if (Mathf.Abs (vAxis) > .4f)
					yDashForce = (vAxis > .1f) ? 1f : -1f;

                float xDashForce = 0;
                if (Mathf.Abs(hAxis) > .4f || yDashForce == 0)
                    xDashForce = (facingRight) ? 1f : -1f;

				////gotta set the combination of the forces to to the total dash force
    //            if(Mathf.Abs(xDashForce) > Mathf.Abs(yDashForce))
    //            {
    //                xDashForce *= dashSpeed;
    //            }
    //            else if (Mathf.Abs(yDashForce) > Mathf.Abs(xDashForce))
    //            {
    //                yDashForce *= dashSpeed;
    //            }
    //            else if(Mathf.Abs(xDashForce) == 1 && Mathf.Abs(yDashForce) == 1)
    //            {
    //                xDashForce *= (dashSpeed/2f);
    //                yDashForce *= (dashSpeed/2f);
    //            }
    //            else
    //            {
    //                xDashForce = (facingRight)?dashSpeed:-dashSpeed;
    //            }
                    
                if (grounded && Mathf.Abs(xDashForce) > Mathf.Abs(yDashForce))
                    anim.SetBool(ANIM_ROLLING, true);
                else if (Mathf.Abs(xDashForce) > Mathf.Abs(yDashForce))
                    anim.SetBool(ANIM_DASHING, true);
                else
                    anim.SetBool(ANIM_DASH_UPWARD, true);

                Vector3 dashForce = new Vector2 (xDashForce, yDashForce);
                dashToward = transform.position + (dashForce * 10f);
				//playerRigidbody2D.AddForce (dashForce, ForceMode2D.Impulse);
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

    void setSpriteOpacity(float opacity)
    {
        foreach(SpriteRenderer subRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            subRenderer.color = new Color(subRenderer.color.r, subRenderer.color.g, subRenderer.color.b, opacity);
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
            if(dashing)
            {
                transform.position = Vector3.MoveTowards(transform.position, dashToward, dashSpeed * Time.deltaTime);
            }
		}

		if (dashing && timeSinceDash >= dashDuration) {
			anim.SetBool(ANIM_DASHING, false);
			anim.SetBool(ANIM_ROLLING, false);
            anim.SetBool(ANIM_DASH_UPWARD, false);
            dashing = false;
            playerRigidbody2D.isKinematic = false;
            dashToward = Vector2.zero;
            setSpriteOpacity(1f);
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
			anim.SetBool (ANIM_LEDGE_GRAB, true);
			ledgeCollider.enabled = true;
		}
		else
			if (grabbingLedge) {
				grabbingLedge = false;
				anim.SetBool (ANIM_LEDGE_GRAB, false);
				ledgeCollider.enabled = false;
			}
	}

	void CheckWallSlide (float hAxis)
	{
		if (!grounded && touchingRightWall && playerRigidbody2D.velocity.y <= 0 && (/* falling */(facingRight && hAxis > 0f) || /* holding against right wall */(!facingRight && hAxis < 0f)))/* holding against left wall */ {
			playerRigidbody2D.gravityScale = 0.15f;
			////Debug.Log("setting gravity in wall slide");
			playerRigidbody2D.velocity = new Vector2 (0f, -1f);
			wallSliding = true;

			anim.SetBool (ANIM_WALL_SLIDING, true);
			if(!slideSmoke.isPlaying)
				slideSmoke.Play();
		}
		else {
			wallSliding = false;
			anim.SetBool (ANIM_WALL_SLIDING, false);
			if(slideSmoke.isPlaying)
				slideSmoke.Stop();
		}
	}

	void tryToJump()
	{
		if((grounded || (!doubleJump && doubleJumpAllowed)))
		{
			//anim.SetBool(ANIM_GROUND, false);
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
		GameObject shuriken = Instantiate<GameObject>(shurikenPrefab);
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
		shuriken.GetComponent<ShurikenParentController>().setVelocity(velocity);
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
            slideSmoke.Stop();
            playerRigidbody2D.gravityScale = 1f;
            foreach(string animParam in animParams)
            {
                anim.SetBool(animParam, false);
            }            
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
        if (item.GetComponent<FragGrenadeController>() != null)
        {
            foreach(GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<FragGrenadeController>() != null)
                    grenadePrefab = weapon;
            }
            grenadeCount = maxGrenades;
            updateGrenadeSprites();
        }
        else if (item.GetComponent<ElectricGrenadeController>() != null)
        {
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<ElectricGrenadeController>() != null)
                    grenadePrefab = weapon;
            }
            grenadeCount = maxGrenades;
            updateGrenadeSprites();
        }
        else if (item.GetComponent<SlimeGrenadeController>() != null)
        {
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<SlimeGrenadeController>() != null)
                    grenadePrefab = weapon;
            }
            grenadeCount = maxGrenades;
            updateGrenadeSprites();
        }
        else if (item.GetComponent<TimeGrenadeController>() != null)
        {
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<TimeGrenadeController>() != null)
                    grenadePrefab = weapon;
            }
            grenadeCount = maxGrenades;
            updateGrenadeSprites();
        }
        else if(item.GetComponent<ShurikenController>() != null)
		{
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<ShurikenController>() != null)
                    shurikenPrefab = weapon;
            }
            shurikenCount = maxShurikens;
			updateShurikenSprites();
		}
        else if (item.GetComponent<BouncingShurikenController>() != null)
        {
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<BouncingShurikenController>() != null)
                    shurikenPrefab = weapon;
            }
            shurikenCount = maxShurikens;
            updateShurikenSprites();
        }
        else if (item.GetComponent<PenetratingShurikenController>() != null)
        {
            foreach (GameObject weapon in weaponPrefabs)
            {
                if (weapon.GetComponent<PenetratingShurikenController>() != null)
                    shurikenPrefab = weapon;
            }
            shurikenCount = maxShurikens;
            updateShurikenSprites();
        }
    }

    float slowMultiplier = 2f;

    public void slowed()
    {
        maxSpeed /= slowMultiplier;
        dashSpeed /= slowMultiplier;
        attackThrustSpeed /= slowMultiplier;
        anim.speed /= slowMultiplier;
    }

    public void unSlowed()
    {
        maxSpeed *= slowMultiplier;
        dashSpeed *= slowMultiplier;
        attackThrustSpeed *= slowMultiplier;
        anim.speed *= slowMultiplier;
    }

    void OnTriggerEnter2D(Collider2D collider)
	{
        if(dashing && collider.gameObject.layer == LayerMask.NameToLayer("Walls") ||
                      collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            timeSinceDash = dashDuration + timeBetweenDashes + .1f;
        }        
    }

	//void OnCollisionEnter2D(Collision2D collision)
	//{
 //       if(dashing && collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
 //       {
 //           Debug.Log("player collision: " + collision.gameObject.name);
 //           playerRigidbody2D.isKinematic = false;
 //       }
	//}
}