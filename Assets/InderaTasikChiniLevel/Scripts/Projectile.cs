using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;

    private void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        skeletonController skeleton = hitInfo.GetComponent<skeletonController>();
        BatController bat = hitInfo.GetComponent<BatController>();
        ghostController ghost = hitInfo.GetComponent<ghostController>();
        witchController witch = hitInfo.GetComponent<witchController>();
        bossController boss = hitInfo.GetComponent<bossController>();
        toyolController toyol = hitInfo.GetComponent<toyolController>();
        pontianakController ponti = hitInfo.GetComponent<pontianakController>();

        if (skeleton != null)
        {
            Destroy(gameObject);
            skeleton.TakeDamage(20);
            skeleton.animator.SetTrigger("skellyHit");
            skeleton.stunTime = 0f;
        }
        else if (bat != null)
        {
            Destroy(gameObject);
            bat.TakeDamage(100);
        }
        else if (ghost != null)
        {
            Destroy(gameObject);
            ghost.TakeDamage(100);
        }
        else if (witch != null)
        {
            Destroy(gameObject);
            witch.TakeDamage(20);
            witch.animator.SetTrigger("witchHit");
            witch.stunTime = 0f;
        }
        else if (boss != null)
        {
            Destroy(gameObject);
            boss.TakeDamage(15);
            boss.animator.SetTrigger("bossHit");
            boss.stunTime = 0f;
        }
        else if (toyol != null)
        {
            Destroy(gameObject);
            toyol.TakeDamage(100);
            toyol.animator.SetTrigger("toyolHit");
            toyol.stunTime = 0f;
        }
        else if (ponti != null)
        {
            Destroy(gameObject);
            ponti.TakeDamage(15);
            ponti.animator.SetTrigger("pontiHit");
            ponti.stunTime = 0f;
        }

        Destroy(gameObject);
    }
}
