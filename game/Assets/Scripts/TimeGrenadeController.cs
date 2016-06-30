using UnityEngine;
using System.Collections;

public class TimeGrenadeController : GrenadeController {

	public float grenadeDuration;
    public CircleCollider2D timeCollider;
    public SpriteRenderer timeRenderer;
	
	protected void Explode()
	{
		base.Explode();
        grenadeParticleSystem.Play();
        timeCollider.enabled = true;
        timeRenderer.enabled = true;
		grenadeRigidbody.isKinematic = true;
		Invoke("destroyGrenade", grenadeDuration);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
        Rigidbody2D colliderBody = collider.gameObject.GetComponent<Rigidbody2D>();
        if(colliderBody != null)
        {
            colliderBody.velocity /= 2;
        }
	}

    void OnTriggerExit2D(Collider2D collider)
    {
        Rigidbody2D colliderBody = collider.gameObject.GetComponent<Rigidbody2D>();
        if (colliderBody != null)
        {
            colliderBody.velocity *= 2;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        this.transform.SetParent(collision.gameObject.transform);
		Explode();
	}
}
