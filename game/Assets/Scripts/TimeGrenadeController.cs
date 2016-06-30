using UnityEngine;
using System.Collections;

public class TimeGrenadeController : GrenadeController {

	public float grenadeDuration;
    public CircleCollider2D timeCollider;
    public SpriteRenderer timeRenderer;
	
	protected override void Explode()
	{
		base.Explode();
        grenadeParticleSystem.Play();
        timeCollider.enabled = true;
        timeRenderer.enabled = true;
		grenadeRigidbody.isKinematic = true;
        grenadeCollider.enabled = false;
        Invoke("collapseCollider", grenadeDuration-.1f);
        Invoke("destroyGrenade", grenadeDuration);
	}

    void collapseCollider()
    {
        timeCollider.radius = 0f;
    }
    
    void OnTriggerEnter2D(Collider2D collider)
	{
        ISlowable slowable = collider.gameObject.GetComponent<ISlowable>();
        if(slowable != null)
        {
            slowable.slowed();
        }
	}

    void OnTriggerExit2D(Collider2D collider)
    {
        ISlowable slowable = collider.gameObject.GetComponent<ISlowable>();
        if (slowable != null)
        {
            slowable.unSlowed();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        this.transform.SetParent(collision.gameObject.transform);
		Explode();
	}
}
