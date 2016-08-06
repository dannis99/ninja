using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GrenadeController : MonoBehaviour, ISlowable {
    public PlayerController player;

	public CircleCollider2D grenadeCollider;
	public Rigidbody2D grenadeRigidbody;
	public SpriteRenderer grenadeRenderer;
	public ParticleSystem grenadeParticleSystem;
	public Light grenadeLight;
	public float secondsToExplosion;

    private float t;
    private Color onColor;
    private Color offColor;
    private float timeFluctuation = 2f;

	protected bool exploded;

	protected void Start () {
        onColor = grenadeLight.color;
        offColor = Color.black;
		if(secondsToExplosion > 0)
			Invoke("Explode", secondsToExplosion);
	}

	protected void Update () {
        t += Time.deltaTime;
        float timeDiff = t % timeFluctuation;
        if(timeDiff > timeFluctuation/2f)
        {
            grenadeRenderer.color = Color.Lerp(onColor, offColor, timeDiff - Mathf.Floor(timeDiff));
            grenadeLight.color = Color.Lerp(onColor, offColor, timeDiff - Mathf.Floor(timeDiff));
        }
        else
        {
            grenadeRenderer.color = Color.Lerp(offColor, onColor, timeDiff - Mathf.Floor(timeDiff));
            grenadeLight.color = Color.Lerp(offColor, onColor, timeDiff - Mathf.Floor(timeDiff));
        }
	}

	protected virtual void Explode () {
		exploded = true;
	}

	protected virtual void destroyGrenade()
	{
		Destroy(this.gameObject);
	}

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "lethalSword")
        {
            Vector3 force = transform.position - collider.gameObject.transform.position;
            grenadeRigidbody.AddForce(force * 100f);
        }
    }

    public void slowed()
    {
        grenadeRigidbody.velocity /= 2f;
    }

    public void unSlowed()
    {
        grenadeRigidbody.velocity *= 2f;
    }
}
