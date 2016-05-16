using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour {

	public List<GameObject> possibleItems;
	public List<float> itemChances;
	public SpriteRenderer chestLight;
	public GameObject chestLidLeft;
	public GameObject chestLidRight;
	public Collider2D chestCollider;

	PlayerController player;

	float xLidStart = .185f;
	float xLidEnd = .4f;

	Vector3 fromPosition;
	Vector3 toPosition;

	bool chestActive = true;
	bool movingToEnd;
	float t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(movingToEnd && chestLidRight.transform.localPosition.x != xLidEnd)
		{
			t += Time.deltaTime;
			chestLidRight.transform.localPosition = new Vector3(Mathf.Lerp(xLidStart, xLidEnd, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			chestLidLeft.transform.localPosition = new Vector3(-Mathf.Lerp(xLidStart, xLidEnd, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
		}
		else if(!movingToEnd && chestLidRight.transform.localPosition.x != xLidStart)
		{
			t += Time.deltaTime;
			chestLidRight.transform.localPosition = new Vector3(Mathf.Lerp(xLidEnd, xLidStart, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			chestLidLeft.transform.localPosition = new Vector3(-Mathf.Lerp(xLidEnd, xLidStart, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
		}

		if(chestActive && chestLidRight.transform.localPosition.x == xLidEnd)
		{
			float random = Random.Range(0, 100);
			GameObject itemToGive = null;
			//give the player an item
			int itemIndex = 0;
			foreach(float chance in itemChances)
			{
				if(random <= chance)
				{
					itemToGive = possibleItems[itemIndex];
					break;
				}
				itemIndex++;
			}
			Debug.Log("About to give player: "+itemToGive.name);
			chestLight.color = Color.red;
			chestCollider.enabled = false;
			chestActive = false;
		}
		else if(chestLidRight.transform.localPosition.x == xLidStart)
		{
			t = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			player = collider.gameObject.GetComponent<PlayerController>();
			if (!player.shieldController.shielded) 
			{
				chestLight.color = Color.yellow;
				movingToEnd = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			chestLight.color = Color.green;
			movingToEnd = false;
			t = 1f - t;
		}
	}
}
