using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public SpriteRenderer portalRenderer;
    public Collider2D portalCollider;
    public Portal pairedPortal;
    public GameObject portaledObject;

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject currObject = collider.gameObject;
        if (currObject == portaledObject)
        {
            portaledObject = null;
        }
        else
        {
            pairedPortal.portaledObject = currObject;
            currObject.transform.position = pairedPortal.transform.position;
        }
    }
}