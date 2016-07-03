using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public SpriteRenderer portalRenderer;
    public Collider2D portalCollider;
    public Portal pairedPortal;
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject currObject = collider.gameObject;
        if(currObject.tag != "Untagged")
        {
            Vector3 positionDifference = pairedPortal.transform.position - transform.position;
            currObject.transform.position += positionDifference;
        }        
    }
}