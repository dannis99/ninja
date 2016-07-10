using UnityEngine;
using System.Collections;

public class spaceshipFloor : MonoBehaviour {

	public Sprite[] floorSprites;
	public SpriteRenderer floorRenderer;

	Color originalColor;
    Color clearColor;
	float t;
    bool playerStanding = false;
	
	// Use this for initialization
	void Start () {
		floorRenderer.sprite = floorSprites[Random.Range(0, floorSprites.Length)];
		originalColor = floorRenderer.color;
        clearColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        floorRenderer.color = clearColor;
        floorRenderer.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if(playerStanding && floorRenderer.color != originalColor)
        {
            t += Time.deltaTime;
            floorRenderer.color = Color.Lerp(clearColor, originalColor, t);
        }
        else if(!playerStanding && floorRenderer.color != clearColor)
        {
            t += Time.deltaTime;
            floorRenderer.color = Color.Lerp(originalColor, clearColor, t);
        }
        else if((playerStanding && floorRenderer.color == originalColor) || (!playerStanding && floorRenderer.color == clearColor))
        {
            t = 0;
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
            if (t != 0)
                t = 1f - t;
            playerStanding = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
            if (t != 0)
                t = 1f - t;
            playerStanding = false;
		}
	}
}
