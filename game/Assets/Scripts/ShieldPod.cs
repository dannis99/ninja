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
		if(movingToEnd && bar.transform.position != endPosition)
		{
			bar.transform.position = Vector3.Lerp(startPosition, endPosition, t);
		}
		else if(!movingToEnd && bar.transform.position != startPosition)
		{
			bar.transform.position = Vector3.Lerp(endPosition, startPosition, t);
		}

		if(bar.transform.position == endPosition)
		{
			movingToEnd = false;
			t = 0;
		}
		else if(bar.transform.position == startPosition)
		{
			t = 0;
		}
		else
		{
			t += Time.deltaTime;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player" && !player.shieldController.shielded)
		{
			podLight.color = Color.green;
			playerInPod = true;
			player = collider.gameObject.GetComponent<PlayerController>();
			movingToEnd = true;
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
