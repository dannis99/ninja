using UnityEngine;
using System.Collections;

public class SlimeGroundController : MonoBehaviour {

    public PlayerController player;
    public float duration;
    
    void Start()
    {
        Invoke("destroySlime", duration);
    }

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "player")
		{
            collider.gameObject.GetComponent<PlayerController> ().takeDamage (player);			
		}
	}

    private void destroySlime()
    {
        Destroy(gameObject);
    }
}
