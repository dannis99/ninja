using UnityEngine;
using System.Collections;

public class SlimeGroundController : MonoBehaviour {

    public float duration;
    
    void Start()
    {
        Invoke("destroySlime", duration);
    }

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "player")
		{
            collider.gameObject.GetComponent<PlayerController> ().takeDamage ();			
		}
	}

    private void destroySlime()
    {
        Destroy(gameObject);
    }
}
