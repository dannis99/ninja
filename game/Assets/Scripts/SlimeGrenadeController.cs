using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeGrenadeController : GrenadeController {

    public GameObject slimePrefab;
    public int slimeCount;
    public float maxForce;

	protected override void Explode () {
		base.Explode();
		grenadeParticleSystem.Play();
		grenadeRenderer.enabled = false;
		grenadeLight.color = Color.black;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		Vector3 explosionPos = transform.position;
        for(int i = 0; i < slimeCount; i++)
        {
            GameObject slime = Instantiate<GameObject>(slimePrefab);
            slime.transform.position = transform.position;
            Rigidbody2D slimeBody = slime.GetComponent<Rigidbody2D>();
            float xForce = Random.Range(.5f, maxForce);
            xForce = (Random.Range(1, 3) == 2) ? -xForce : xForce;
            float yForce = Random.Range(.5f, maxForce);
            slimeBody.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);
        }
        destroyGrenade();
	}
}
