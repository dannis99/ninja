using UnityEngine;
using System.Collections;

public class ShurikenController : MonoBehaviour {

	new Rigidbody2D rigidbody2D;
	Collider2D[] colliders;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		colliders = GetComponents<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag.Contains("surface"))
		{
			rigidbody2D.isKinematic = true;
			foreach(Collider2D collider in colliders)
			{
				if(!collider.isTrigger)
				{
					collider.enabled = false;
				}
			}
		}
	}
}
