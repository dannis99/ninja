﻿using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {

	public GameObject currentPlayer;
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
		if ((collider.gameObject.tag == "lethalSword" || collider.gameObject.tag == "blockingSword") && !alternateSwordHitBox.Equals(collider.gameObject))
        {
            Debug.Log("sword on sword collision");
            Vector3 otherPlayerPosition = Vector3.zero;
            SwordController swordController = collider.gameObject.GetComponent<SwordController>();
            if (swordController != null)
                otherPlayerPosition = swordController.currentPlayer.transform.position;
            else
                otherPlayerPosition = collider.gameObject.transform.position;

            Vector3 force = currentPlayer.transform.position - otherPlayerPosition;
            Debug.Log("Adding force to player: " + currentPlayer.name + " force: " + force * 100f);
            currentPlayer.GetComponent<Rigidbody2D>().AddForce(force * 250f);
            Debug.Log("can't hit player");
            canHitPlayer = false;
        }
        else if (canHitPlayer && collider.gameObject.tag == "player" && !currentPlayer.Equals(collider.gameObject))
		{
            Debug.Log("can hit player");
            collider.gameObject.GetComponent<PlayerController> ().takeDamage ();
			GameObject swordClash = Instantiate<GameObject>(swordClashPrefab);
			swordClash.transform.position = (collider.transform.position+transform.position)/2f;
		}
	}
}
