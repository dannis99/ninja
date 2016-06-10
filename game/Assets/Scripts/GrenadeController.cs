using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrenadeController : MonoBehaviour {

	public CircleCollider2D grenadeCollider;
	public Rigidbody2D grenadeRigidbody;
	public SpriteRenderer grenadeRenderer;
	public ParticleSystem grenadeParticleSystem;
	public Light grenadeLight;
	public int secondsToExplosion;

	protected void Start () {
		if(secondsToExplosion > 0)
			Invoke("Explode", secondsToExplosion);
	}

	protected void Explode () {
		
	}

	protected void destroyGrenade()
	{
		Destroy(this.gameObject);
	}
}
