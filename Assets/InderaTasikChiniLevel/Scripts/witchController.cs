using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class witchController : MonoBehaviour, IDamageable
{
    public float StunTime
    {
        get { return stunTime; }
        set { stunTime = value; }
    }

    public GameObject projectile;
    public Transform projectileSpawner;

    public int health;
    [SerializeField] GameObject witchDeath; //death animation

    public Transform player;
    public float lineOfSight;
    public float moveSpeed;

    public float attackCooldown;
    public float attackRange;
    [SerializeField] BoxCollider2D box;
    [SerializeField] LayerMask playerLayer;
    private float cooldownTimer = 2;

    public float attackTimer;
    public bool canWalk;
    public float stunTime;

    public Animator animator;
    public GameObject indera;
    public void TakeDamage(int damage)
    {
        FindObjectOfType<audioManager>().Play("witchHit");

        health -= damage;
        animator.SetTrigger("witchHit");

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FindObjectOfType<audioManager>().Play("witchDie");

        GameObject instance = Instantiate(witchDeath, transform.position, transform.rotation);

        Destroy(gameObject);

        Destroy(instance, 2f);
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

        animator.SetInteger("witchState", 0); // idle witch

        //cooldownTimer += Time.deltaTime;

        if (attackTimer < 1f)
        {
            moveSpeed = 0f;
            attackTimer += Time.deltaTime;
        }
        else
        {
            if (distanceFromPlayer < lineOfSight)
            {
                if (stunTime >= 1f)
                {
                    moveSpeed = 2f;

                    animator.SetInteger("witchState", 1); // walk witch

                    transform.position = Vector2.MoveTowards(transform.position, player.position, -1 * moveSpeed * Time.deltaTime);

                    if (transform.position.x > player.position.x)
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    if (transform.position.x < player.position.x)
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
                else if (stunTime < 1f)
                {
                    moveSpeed = 0f;
                    stunTime += Time.deltaTime;
                }
            }
        }

        if (distanceFromPlayer <= attackRange)
        {
            Debug.Log("Detected");

            if (cooldownTimer >= attackCooldown)
            {
                Debug.Log("Witch Attacking");

                cooldownTimer = 0;
                animator.SetInteger("witchState", 2); // witch attack
                if (transform.position.x > player.position.x)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if (transform.position.x < player.position.x)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                shoot();
                attackTimer = 0;
            }
            cooldownTimer += Time.deltaTime;
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    void shoot()
    {
        GameObject instant = Instantiate(projectile, projectileSpawner.position, projectileSpawner.rotation);
        Destroy(instant, 2.5f);
    }

    private void OnDrawGizmosSelected()
    {
        //Detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);

        //Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
