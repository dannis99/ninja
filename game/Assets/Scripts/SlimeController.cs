using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour {

    public float duration;
    public GameObject groundSlimePrefab;

    private float elapsedTime;

    void Start()
    {
        Invoke("destroySlime", duration);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

	void OnCollisionEnter2D(Collision2D collision)
    {
        detectCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        detectCollision(collision);
    }

    private void detectCollision(Collision2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            collision.gameObject.GetComponent<PlayerController>().takeDamage();
        }
        else if (elapsedTime > .2f && collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            GameObject groundSlime = Instantiate<GameObject>(groundSlimePrefab);
            groundSlime.transform.position = collision.contacts[0].point;
            if(collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                if (transform.position.x < collision.gameObject.transform.position.x)
                    groundSlime.transform.eulerAngles = new Vector3(groundSlime.transform.eulerAngles.x, groundSlime.transform.eulerAngles.y, 90);
                else
                    groundSlime.transform.eulerAngles = new Vector3(groundSlime.transform.eulerAngles.x, groundSlime.transform.eulerAngles.y, -90);
            }
            else
            {
                if(transform.position.y < collision.gameObject.transform.position.y)
                    groundSlime.transform.eulerAngles = new Vector3(groundSlime.transform.eulerAngles.x, groundSlime.transform.eulerAngles.y, 180);
            }
            Destroy(gameObject);
        }
    }

    private void destroySlime()
    {
        Destroy(gameObject);
    }
}
