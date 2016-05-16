using UnityEngine;
using System.Collections;

public class spaceshipFloor : MonoBehaviour {

	public Sprite[] floorSprites;
	public SpriteRenderer renderer;

	Color originalColor;
	int playerCount = 0;
	
	// Use this for initialization
	void Start () {
		renderer.sprite = floorSprites[Random.Range(0, floorSprites.Length)];
		originalColor = renderer.color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "player")
		{
			if(playerCount == 0)
			{
				renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
			}
			playerCount++;
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "player")
		{
			playerCount--;
			if(playerCount == 0)
			{
				renderer.color = originalColor;
			}
		}
	}
}
