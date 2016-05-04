using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

	public GameObject currentPlayer;
	public GameObject swordClashPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("sword collision: "+collider.gameObject.tag);
		if (collider.gameObject.tag == "player" && !currentPlayer.Equals(collider.gameObject))
		{
			collider.gameObject.GetComponent<PlayerController> ().takeDamage ();
			GameObject swordClash = Instantiate<GameObject>(swordClashPrefab);
			swordClash.transform.position = (collider.transform.position+transform.position)/2f;
//			foreach (SpriteRenderer renderer in collider.gameObject.GetComponentsInChildren<SpriteRenderer>())
//			{
//				renderer.color = Color.red;
//			}
		}
	}
}
