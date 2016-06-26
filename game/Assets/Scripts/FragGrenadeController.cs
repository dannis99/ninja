using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FragGrenadeController : GrenadeController {

	public float explosionRadius;
	public int explosionDuration;
	public int explosionForce;

	protected override void Explode () {
		base.Explode();
		grenadeParticleSystem.Play();
		grenadeRenderer.enabled = false;
		grenadeLight.color = Color.black;
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
}
