﻿using UnityEngine;
using System.Collections;

public class ElectricGrenadeController : GrenadeController {

	public float grenadeDuration;
    public float radiusDelay;
	public CircleCollider2D electricCollider;
	float startGrenadeRadius = .1f;
	float endGrenadeRadius = 1f;
	float t = 0;
    float delay = 0;

	protected void Update()
	{
		base.Update();
		var shape = grenadeParticleSystem.shape;
        
		if(exploded && shape.radius < endGrenadeRadius)
		{
            if(delay < radiusDelay)
            {
                delay += Time.deltaTime;
            }
            else
            {
                if(!grenadeParticleSystem.isPlaying)
                    grenadeParticleSystem.Play();
                shape.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, t);
                electricCollider.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, t);
                t += Time.deltaTime;
            }			
		}
	}

	protected void Explode()
	{
		base.Explode();
		var shape = grenadeParticleSystem.shape;
		shape.radius = startGrenadeRadius;
		electricCollider.radius = startGrenadeRadius;
		electricCollider.enabled = true;
		grenadeRigidbody.isKinematic = true;
		Invoke("destroyGrenade", grenadeDuration);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			collider.gameObject.GetComponent<PlayerController>().takeDamage();
			Invoke("destroyGrenade", 1);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Explode();
	}
}