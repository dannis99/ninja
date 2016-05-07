using UnityEngine;
using System.Collections;

public class ShieldPod : MonoBehaviour {

	public SpriteRenderer podLight;
	public GameObject bar;
	public Collider2D podCollider;
	public SpriteRenderer glassRenderer;
	public Sprite[] glassSprites;
	public ParticleSystem[] smokeParticleSystems;
	public Animator anim;

	public float timeBetweenDamage;
	float timeSinceDamage;
	bool canTakeDamage = true;

	PlayerController player;

	Vector3 startPosition = new Vector3(0f, -0.67f, 0f);
	Vector3 endPosition = new Vector3(0f, 0.67f, 0f);

	Vector3 fromPosition;
	Vector3 toPosition;

	bool movingToEnd;
	bool playerInPod;
	bool canShield;
	float t;
	int damageCount = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(movingToEnd && bar.transform.localPosition != endPosition)
		{
			t += Time.deltaTime;
			Debug.Log ("t: " + t);
			bar.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
			Debug.Log ("position: " + bar.transform.localPosition);
		}
		else if(!movingToEnd && bar.transform.localPosition != startPosition)
		{
			t += Time.deltaTime;
			bar.transform.localPosition = Vector3.Lerp(endPosition, startPosition, t);
		}

		if(bar.transform.localPosition == endPosition)
		{
			canShield = true;
			movingToEnd = false;
			t = 0;
		}
		else if(bar.transform.localPosition == startPosition)
		{
			t = 0;
			if (canShield && player != null && !player.shieldController.shielded)
			{
				player.shieldController.shielded = true;
				podLight.color = Color.green;
				canShield = false;
			}
		}

		if(!canTakeDamage)
		{
			timeSinceDamage += Time.deltaTime;
			if(timeSinceDamage > timeBetweenDamage)
			{
				canTakeDamage = true;
			}
		}
	}

	public void takeDamage()
	{
		damageCount++;
		timeSinceDamage = 0;
		glassRenderer.sprite = glassSprites[damageCount];
		if(damageCount == 2)
		{
			foreach(ParticleSystem smoke in smokeParticleSystems)
			{
				if(!smoke.isPlaying)
					smoke.Play();
			}
		}
		else if(damageCount == 3)
		{
			foreach(ParticleSystem smoke in smokeParticleSystems)
			{
				if(smoke.isPlaying)
					smoke.Stop();
			}
			podCollider.enabled = false;
			anim.SetTrigger("broken");
			podLight.color = Color.red;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			player = collider.gameObject.GetComponent<PlayerController>();
			if (!player.shieldController.shielded) 
			{
				podLight.color = Color.yellow;
				playerInPod = true;
				movingToEnd = true;
			}
		}
		else if(collider.gameObject.tag == "lethal")
		{
			if(canTakeDamage)
				takeDamage();
		}
	}

//	void OnTriggerStay2D(Collider2D collider)
//	{
//		if(collider.gameObject.tag == "player")
//		{
//			
//		}
//	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			playerInPod = false;
			podLight.color = Color.green;
			movingToEnd = false;
			t = 1f - t;
			canShield = false;
		}
	}
}
