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
	public SpriteRenderer itemRenderer;
	public Light itemLight;

	PlayerController player;

	GameObject itemToGive;
	float xLidStart = .185f;
	float xLidEnd = .4f;
	float yItemStart = -.6f;
	float yItemEnd = -.09f;

	Vector3 fromPosition;
	Vector3 toPosition;

	bool chestActive = true;
	bool movingToEnd;
	float t;

	// Use this for initialization
	void Start () {
		float random = Random.Range(0, 100);
		int itemIndex = 0;
        float totalChance = 0;
		foreach(float chance in itemChances)
		{
            totalChance += chance;
			if(random <= totalChance)
			{
				itemToGive = possibleItems[itemIndex];
                Debug.Log("chest item: " + itemToGive.name);
				break;
			}
			itemIndex++;
		}
		itemRenderer.sprite = itemToGive.GetComponent<SpriteRenderer>().sprite;
	}
	
	// Update is called once per frame
	void Update () {
		if(movingToEnd && chestLidRight.transform.localPosition.x != xLidEnd)
		{
			t += Time.deltaTime;
			chestLidRight.transform.localPosition = new Vector3(Mathf.Lerp(xLidStart, xLidEnd, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			chestLidLeft.transform.localPosition = new Vector3(-Mathf.Lerp(xLidStart, xLidEnd, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			itemRenderer.transform.localPosition = new Vector3(itemRenderer.transform.localPosition.x, Mathf.Lerp(yItemStart, yItemEnd, t), itemRenderer.transform.localPosition.z);
			itemLight.enabled = true;
		}
		else if(!movingToEnd && chestLidRight.transform.localPosition.x != xLidStart)
		{
			t += Time.deltaTime;
			chestLidRight.transform.localPosition = new Vector3(Mathf.Lerp(xLidEnd, xLidStart, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			chestLidLeft.transform.localPosition = new Vector3(-Mathf.Lerp(xLidEnd, xLidStart, t), chestLidRight.transform.localPosition.y, chestLidRight.transform.localPosition.z);
			itemRenderer.transform.localPosition = new Vector3(itemRenderer.transform.localPosition.x, Mathf.Lerp(yItemEnd, yItemStart, t), itemRenderer.transform.localPosition.z);
		}

		if(chestActive && chestLidRight.transform.localPosition.x == xLidEnd)
		{
			Debug.Log("About to give player: "+itemToGive.name);
			player.setItem(itemToGive);
			chestLight.color = Color.red;
			chestCollider.enabled = false;
			chestActive = false;
			itemRenderer.gameObject.SetActive(false);
		}
		else if(chestLidRight.transform.localPosition.x == xLidStart)
		{
			t = 0;
			itemLight.enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			player = collider.gameObject.GetComponent<PlayerController>();
			chestLight.color = Color.yellow;
			movingToEnd = true;
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
