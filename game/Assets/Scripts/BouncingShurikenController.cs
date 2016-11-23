using UnityEngine;
using System.Collections;

public class BouncingShurikenController : ShurikenParentController
{
    public int bounces;
    bool active = true;
    float timeInactiveAfterBlockHit = .1f;
    bool canHitPlayerAndBounce = true;
    GameObject recentlyHit;

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
                collidedObject.GetComponent<PlayerController>().takeDamage(player);
                Destroy(gameObject);
                Debug.Log("killing player with bouncing shurken");
            }
            else if (bounces > 0 && canHitPlayerAndBounce && recentlyHit != collidedObject)
            {
                if (collidedObject.tag == "lethalSword" || collidedObject.tag == "blockingSword")
                {
                    canHitPlayerAndBounce = false;
                    Invoke("canNowHitPlayerAndBounce", timeInactiveAfterBlockHit);
                }

                recentlyHit = collidedObject;
                Invoke("clearRecentlyHit", timeInactiveAfterBlockHit);

                bounces--;
                if (reflectNormal != Vector2.zero)
                {
                    velocity = Vector2.Reflect(velocity, reflectNormal);
                }
                else
                {
                    velocity = new Vector2(-1f * velocity.x, -1f * velocity.y);
                }
            }
            else if (bounces <= 0)
            {
                collision(collidedObject);
            }
        }
    }

    /**
     * gotta override this method and check for bounces for cases like shield grenades so it doesn't just stop the bouncing shuriken 
     **/
    public override void collision(GameObject collidedObject)
    {
        if (bounces <= 0)
        {            
            velocity = Vector2.zero;
            if (collidedObject != null && collidedObject.tag == "surface")
            {
                shurikenRigidbody2D.isKinematic = true;
                if (collidedObject != null)
                    transform.SetParent(collidedObject.transform);

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

    private void clearRecentlyhit()
    {
        recentlyHit = null;
    }
}
