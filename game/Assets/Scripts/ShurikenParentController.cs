using UnityEngine;
using System.Collections;
using System;

public class ShurikenParentController : MonoBehaviour, ISlowable {

	public Rigidbody2D shurikenRigidbody2D;
	public Collider2D[] colliders;
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

    public void slowed()
    {
        velocity /= 3f;
    }

    public void unSlowed()
    {
        velocity *= 3f;
    }
}
