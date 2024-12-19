using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostController : MonoBehaviour, IDamageable
{
    public float StunTime
    {
        get { return stunTime; }
        set { stunTime = value; }
    }

    public int health;
    [SerializeField] GameObject ghostDeath; //death animation

    public Transform player;
    public float lineOfSight;
    public float moveSpeed;

    public int ghostDamage;
    [SerializeField] BoxCollider2D ghostCollider;
    [SerializeField] LayerMask playerLayer;

    public Animator animator;
    public GameObject indera;
    public float stunTime;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FindObjectOfType<audioManager>().Play("ghostDie");
        GameObject instance = Instantiate(ghostDeath, transform.position, Quaternion.identity);

        Destroy(gameObject);

        Destroy(instance, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Animator
        animator = GetComponent<Animator>();

        //Get player position
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer < lineOfSight)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, moveSpeed * Time.deltaTime);

            if (transform.position.x > player.position.x)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (transform.position.x < player.position.x)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        if (playerInRange() == true)
        {
            //Debug.Log("Ghost hits player");
            FindObjectOfType<audioManager>().Play("ghostDetect");
            PlayerController player = indera.GetComponent<PlayerController>();
            player.TakeDamage(ghostDamage);
        }
    }

    public bool playerInRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(ghostCollider.bounds.center, ghostCollider.bounds.size, 0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
