using UnityEngine;
using System.Collections;

public class ShieldPod : MonoBehaviour {

	public SpriteRenderer podLight;
	public GameObject bar;

	PlayerController player;

	Vector3 startPosition = new Vector3(0f, -0.67f, 0f);
	Vector3 endPosition = new Vector3(0f, 0.67f, 0f);

	Vector3 fromPosition;
	Vector3 toPosition;

	bool movingToEnd;
	bool playerInPod;
	float t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(movingToEnd && bar.transform.localPosition != endPosition)
		{
			t += Time.deltaTime;
			Debug.Log ("t: " + t);
			bar.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
			Debug.Log ("position: " + bar.transform.localPosition);
		}
		else if(!movingToEnd && bar.transform.localPosition != startPosition)
		{
			t += Time.deltaTime;
			bar.transform.localPosition = Vector3.Lerp(endPosition, startPosition, t);
		}

		if(bar.transform.localPosition == endPosition)
		{
			movingToEnd = false;
			t = 0;
		}
		else if(bar.transform.localPosition == startPosition)
		{
			t = 0;
			if (player != null && !player.shieldController.shielded)
			{
				player.shieldController.shielded = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			player = collider.gameObject.GetComponent<PlayerController>();
			if (!player.shieldController.shielded) 
			{
				podLight.color = Color.green;
				playerInPod = true;
				movingToEnd = true;
			}
		}
	}

//	void OnTriggerStay2D(Collider2D collider)
//	{
//		if(collider.gameObject.tag == "player")
//		{
//			
//		}
//	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			playerInPod = false;
			podLight.color = Color.red;
			movingToEnd = false;
			t = 0;
		}
	}
}
