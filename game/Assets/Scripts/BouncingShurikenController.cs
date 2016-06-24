using UnityEngine;
using System.Collections;

public class BouncingShurikenController : ShurikenParentController
{
    public int bounces;
    bool active = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (active)
        {
            if (collision.gameObject.tag == "player")
            {
                active = false;
                collision.gameObject.GetComponent<PlayerController>().takeDamage();
                Destroy(gameObject);
            }
            else if(bounces > 0)
            {
                bounces--;
                Debug.Log("bounces");
                var hit = collision.contacts[0];
                Debug.Log("old velocity: " + velocity);
                velocity = Vector2.Reflect(velocity, hit.normal);
                Debug.Log("new velocity: " + velocity);
            }
            else
            {
                active = false;
                velocity = Vector2.zero;
                shurikenRigidbody2D.isKinematic = true;

                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
    }
}
