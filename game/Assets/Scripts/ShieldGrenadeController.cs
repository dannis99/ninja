using UnityEngine;
using System.Collections;

public class ShieldGrenadeController : GrenadeController {

	public float grenadeDuration;
    public CircleCollider2D shieldCollider;
    public SpriteRenderer shieldRenderer;
	
	protected override void Explode()
	{
        if(!exploded)
        {
            base.Explode();
            grenadeParticleSystem.Play();
            shieldCollider.enabled = true;
            shieldRenderer.enabled = true;
            grenadeCollider.enabled = false;
            Invoke("collapseCollider", grenadeDuration - .1f);
            Invoke("destroyGrenade", grenadeDuration);
        }		
	}

    void collapseCollider()
    {
        shieldCollider.radius = 0f;
    }
    
    public override void OnTriggerEnter2D(Collider2D collider)
	{
        ShurikenParentController shuriken = collider.gameObject.GetComponent<ShurikenParentController>();
        if(shuriken != null)
        {
            shuriken.collision(null);
            shuriken.explodeShuriken();
        }
	}

    void OnTriggerExit2D(Collider2D collider)
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
        this.transform.SetParent(collision.gameObject.transform);
        grenadeRigidbody.isKinematic = true;
        Explode();
	}
}
