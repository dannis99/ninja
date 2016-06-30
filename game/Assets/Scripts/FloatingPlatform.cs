using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingPlatform : MonoBehaviour {

	public Vector2 startingPosition;
    public Vector2 endPosition;
    public float speed;

	bool movingToEnd = true;
	
	// Update is called once per frame
	void FixedUpdate () {
		if(movingToEnd && (transform.position.x != endPosition.x || transform.position.y != endPosition.y))
		{
			transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);//new Vector3(Mathf.Lerp(startingPosition.x, endPosition.x, t/movementDuration), Mathf.Lerp(startingPosition.y, endPosition.y, t/movementDuration), transform.position.z);			
		}
		else if(!movingToEnd && (transform.position.x != startingPosition.x || transform.position.y != startingPosition.y))
        {
			transform.position = Vector3.MoveTowards(transform.position, startingPosition, speed * Time.deltaTime);//transform.position = new Vector3(Mathf.Lerp(endPosition.x, startingPosition.x, t/movementDuration), Mathf.Lerp(endPosition.y, startingPosition.y, t/movementDuration), transform.position.z);
        }

		if((transform.position.x == startingPosition.x && transform.position.y == startingPosition.y) ||
           (transform.position.x == endPosition.x && transform.position.y == endPosition.y))
		{
			movingToEnd = (movingToEnd) ? false : true;
		}
	}
}
