using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrenadeController : MonoBehaviour {

	public CircleCollider2D grenadeCollider;
	public SpriteRenderer renderer;
	public ParticleSystem particleSystem;
	public int secondsToExplosion;
	public float explosionRadius;
	public int explosionDuration;
	public int explosionForce;

	// Use this for initialization
	void Start () {
		Invoke("Explode", secondsToExplosion);
	}

	void Explode () {
		particleSystem.Play();
		renderer.enabled = false;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		Vector3 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius, 1 << LayerMask.NameToLayer("Character"));
        List<PlayerController> playersAlreadyHit = new List<PlayerController>();
        foreach (Collider2D hit in colliders) {
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null && !playersAlreadyHit.Contains(player))
            {
				Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
	            if(rb != null)
	            {
					Vector3 newVector = rb.transform.position - transform.position;
					newVector.Normalize();
					rb.AddForce(newVector*explosionForce,ForceMode2D.Impulse);
	            }

            	playersAlreadyHit.Add(player);
            	player.takeDamage();
            }
        }
		//grenadeCollider.radius = explosionRadius;
		Invoke("destroyGrenade", explosionDuration);
	}

	void destroyGrenade()
	{
		Destroy(this.gameObject);
	}
}
