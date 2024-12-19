using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBulletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        FindObjectOfType<audioManager>().Play("enemyCasting");
        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2 (direction.x, direction.y).normalized * speed;
        Physics2D.IgnoreLayerCollision(8, 8, true);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        PlayerController indera = hitInfo.GetComponent<PlayerController>();
        witchController witch = hitInfo.GetComponent<witchController>();
        if (indera != null)
        {
            Destroy(gameObject);
            indera.TakeDamage(20);
        }
        Destroy(gameObject);
    }
}
