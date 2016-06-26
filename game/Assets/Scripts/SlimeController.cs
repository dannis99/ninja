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
            Destroy(gameObject);
        }
    }

    private void destroySlime()
    {
        Destroy(gameObject);
    }
}
