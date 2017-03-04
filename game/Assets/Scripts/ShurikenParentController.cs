using UnityEngine;
using System.Collections;
using System;

public class ShurikenParentController : MonoBehaviour, ISlowable {

	public Rigidbody2D shurikenRigidbody2D;
	public Collider2D[] colliders;
    public PlayerController player;
    public ParticleSystem shurikenDestruction;
	protected Vector2 velocity = Vector2.zero;
    
    void Update()
    {
        shurikenRigidbody2D.velocity = velocity;
        //transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    public virtual void setVelocity(Vector2 newVelocity)
	{
		velocity = newVelocity;
	}
    
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "lethalSword" || collider.gameObject.tag == "blockingSword")
        {
            velocity = new Vector2(-1f * velocity.x, -1f * velocity.y);
        }
    }

    public virtual void collision(GameObject collidedObject)
    {
        velocity = Vector2.zero;
        shurikenRigidbody2D.gravityScale = 15f;
        if (collidedObject != null && collidedObject.tag == "surface")
        {
            shurikenRigidbody2D.isKinematic = true;
            if (collidedObject != null)
                transform.SetParent(collidedObject.transform);

            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void explodeShuriken()
    {
        shurikenRigidbody2D.gravityScale = 1f;
        shurikenDestruction.Play();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        Invoke("destroyShuriken", .25f);        
    }

    private void destroyShuriken()
    {
        Destroy(gameObject);
    }

    public void slowed()
    {
        velocity /= 3f;
    }

    public void unSlowed()
    {
        velocity *= 3f;
    }
}
