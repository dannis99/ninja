using UnityEngine;
using System.Collections;

public class BouncingShurikenController : ShurikenParentController
{
    public int bounces;
    bool active = true;
    float timeInactiveAfterBlockHit = .1f;
    bool canHitPlayerAndBounce = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        checkCollisionAndTrigger(collision.gameObject, collision.contacts[0].normal);
    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "lethalSword" || collider.gameObject.tag == "blockingSword")
        {
            checkCollisionAndTrigger(collider.gameObject, Vector2.zero);
        }
    }

    void checkCollisionAndTrigger(GameObject collidedObject, Vector2 reflectNormal)
    {
        if (active)
        {
            Debug.Log("bouncing hit: " + collidedObject.tag);
            if (collidedObject.tag == "player" && canHitPlayerAndBounce)
            {
                active = false;
                collidedObject.GetComponent<PlayerController>().takeDamage();
                Destroy(gameObject);
                Debug.Log("killing player with bouncing shurken");
            }
            else if (bounces > 0 && canHitPlayerAndBounce)
            {
                if (collidedObject.tag == "lethalSword" || collidedObject.tag == "blockingSword")
                {
                    canHitPlayerAndBounce = false;
                    Debug.Log("setting canHitPlayerFalse");
                    Invoke("canNowHitPlayerAndBounce", timeInactiveAfterBlockHit);
                }

                bounces--;
                if (reflectNormal != Vector2.zero)
                {
                    velocity = Vector2.Reflect(velocity, reflectNormal);
                }
                else
                {
                    velocity = new Vector2(-1f * velocity.x, -1f * velocity.y);
                }                
                Debug.Log("setting new velocity for bouncing shuriken: " + velocity);
            }
            else if (bounces <= 0)
            {
                active = false;
                velocity = Vector2.zero;
                shurikenRigidbody2D.isKinematic = true;
                transform.SetParent(collidedObject.transform, true);

                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
    }

    private void canNowHitPlayerAndBounce()
    {
        canHitPlayerAndBounce = true;
    }    
}
