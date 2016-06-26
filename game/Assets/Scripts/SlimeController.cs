using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour {

    public float duration;

    void Start()
    {
        Invoke("destroySlime", duration);
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "player")
		{
            collision.gameObject.GetComponent<PlayerController> ().takeDamage ();			
		}
	}

    private void destroySlime()
    {
        Destroy(gameObject);
    }
}
