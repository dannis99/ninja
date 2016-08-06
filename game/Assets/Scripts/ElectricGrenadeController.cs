using UnityEngine;
using System.Collections;

public class ElectricGrenadeController : GrenadeController {

	public float grenadeDuration;
    public float radiusDelay;
	public CircleCollider2D electricCollider;
	float startGrenadeRadius = .1f;
	float endGrenadeRadius = 1f;
	float deltaTime = 0;
    float delay = 0;
    ParticleSystem.ShapeModule shape;

    protected void Start()
    {
        base.Start();
        shape = grenadeParticleSystem.shape;
    }

	protected void Update()
	{
		base.Update();
		
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
                shape.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, deltaTime);
                electricCollider.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, deltaTime);
                deltaTime += Time.deltaTime;
            }			
		}
	}

	protected void Explode()
	{
        if(!exploded)
        {
            base.Explode();
            var shape = grenadeParticleSystem.shape;
            shape.radius = startGrenadeRadius;
            electricCollider.radius = startGrenadeRadius;
            electricCollider.enabled = true;
            grenadeCollider.enabled = false;
            Invoke("destroyGrenade", grenadeDuration);
        }		
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			collider.gameObject.GetComponent<PlayerController>().takeDamage(player);
			Invoke("destroyGrenade", 1);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
        this.transform.SetParent(collision.gameObject.transform);
        grenadeRigidbody.isKinematic = true;
        Explode();
	}
}
