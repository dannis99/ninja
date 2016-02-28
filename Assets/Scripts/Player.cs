using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed = 10f;
	public float airSpeedMultiplier = .3f;
	public Vector2 maxVelocity = new Vector2(3,5);
	public bool standing;
	public float jumpSpeed = 300f;

	private bool possiblyStanding;
	private Vector3 initialLocalScale;

	// Use this for initialization
	void Start () {
		initialLocalScale = transform.localScale;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float forceX = 0f;
		float forceY = 0f;

		float absVelX = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x);
		float absVelY = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
		
		if (absVelY == 0f) 
		{
			if(possiblyStanding)
			{
				standing = true;
			}
			else
			{
				possiblyStanding = true;
			}
		} 
		else 
		{
			possiblyStanding = false;
			standing = false;
		}

		if (Input.GetKey ("right") || Input.GetAxis ("Horizontal") > 0) 
		{
			if(absVelX < maxVelocity.x)
			{
				forceX = standing ? speed : (speed * airSpeedMultiplier);
			}
			transform.localScale = new Vector3(initialLocalScale.x,initialLocalScale.y,initialLocalScale.z);
		}
		else if(Input.GetKey("left") || Input.GetAxis ("Horizontal") < 0)
		{
			if(absVelX < maxVelocity.x)
			{
				forceX = standing ? -speed : (-speed * airSpeedMultiplier);
			}
			transform.localScale = new Vector3(-initialLocalScale.x,initialLocalScale.y,initialLocalScale.z);
		}

		if (Input.GetKey ("space") || Input.GetButton("Jump")) 
		{
			if(standing)
			{
				forceY = jumpSpeed;
			}
		}

		GetComponent<Rigidbody2D>().AddForce(new Vector2(forceX, forceY));
		if(forceX != 0 || forceY != 0)
			Debug.Log ("applying force: " + forceX + ", " + forceY);
	}
}
