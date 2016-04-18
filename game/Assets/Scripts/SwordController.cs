using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

	public GameObject currentPlayer;

	// Use this for initialization
	void Start () {
		Debug.Log("creating sword");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("sword collision: "+collider.gameObject.tag);
		if (collider.gameObject.tag == "player" && !currentPlayer.Equals(collider.gameObject))
		{
			foreach (SpriteRenderer renderer in collider.gameObject.GetComponentsInChildren<SpriteRenderer>())
			{
				renderer.color = Color.red;
			}
		}
	}
}
