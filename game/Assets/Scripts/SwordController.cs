using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

	public GameObject currentPlayer;
    Rigidbody2D currentPlayerRigidbody;
	public GameObject swordClashPrefab;
    public GameObject alternateSwordHitBox;
    bool canHitPlayer = true;
    float timeSinceSwordClash;
    float timeToWaitAfterClash = .25f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    if(!canHitPlayer)
        {
            timeSinceSwordClash += Time.deltaTime;
            if(timeSinceSwordClash > timeToWaitAfterClash)
            {
                canHitPlayer = true;
                timeSinceSwordClash = 0f;
            }
        }
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "lethalSword" && !alternateSwordHitBox.Equals(collider.gameObject))
        {
            Debug.Log("sword on sword collision");
            Vector3 force = currentPlayer.transform.position - collider.gameObject.transform.position;
            currentPlayer.GetComponent<Rigidbody2D>().AddForce(force * 100f);
            canHitPlayer = false;
        }
        else if (canHitPlayer && collider.gameObject.tag == "player" && !currentPlayer.Equals(collider.gameObject))
		{
            collider.gameObject.GetComponent<PlayerController> ().takeDamage ();
			GameObject swordClash = Instantiate<GameObject>(swordClashPrefab);
			swordClash.transform.position = (collider.transform.position+transform.position)/2f;
		}
	}
}
