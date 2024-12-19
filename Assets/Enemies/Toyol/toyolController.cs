using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toyolController : MonoBehaviour, IDamageable
{
    public float StunTime
    {
        get { return stunTime; }
        set { stunTime = value; }
    }

    public int health;
    [SerializeField] GameObject toyolDeath; //death animation

    public Transform player;
    public float lineOfSight;
    public float moveSpeed;

    public float attackCooldown;
    [SerializeField] public float range;
    public int toyolDamage;
    [SerializeField] BoxCollider2D attackRange;
    [SerializeField] LayerMask playerLayer;
    private float cooldownTimer = 2;

    public float attackTimer;
    public bool canWalk;
    public float stunTime;

    public Animator animator;
    public GameObject indera;
    public AudioSource footsteps;

    bool isMoving;

    public void TakeDamage(int damage)
    {
        FindObjectOfType<audioManager>().Play("toyolHit"); //sound effect toyol hit

        health -= damage;
        animator.SetTrigger("toyolHit");

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        FindObjectOfType<audioManager>().Play("toyolDie"); //sound effect toyol dies

        GameObject instance = Instantiate(toyolDeath, transform.position, transform.rotation);

        Destroy(gameObject);

        Destroy(instance, 2f);
    }

    void Start()
    {
        //Get Animator
        animator = GetComponent<Animator>();

        //Get player position
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Skeleton Movement and Player Detection
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        animator.SetInteger("toyolState", 0); // idle toyol

        if (isMoving == true && !footsteps.isPlaying)
        {
            footsteps.Play();
        }
        else if (isMoving == false)
        {
            footsteps.Stop();
        }

        if (attackTimer < 1f)
        {
            moveSpeed = 0f;
            attackTimer += Time.deltaTime;
        }
        else
        {
            if (distanceFromPlayer < lineOfSight && playerInRange() == true)
            {
                isMoving = false;
                moveSpeed = 0f;
            }
            else if (distanceFromPlayer < lineOfSight)
            {
                if (stunTime >= 1f)
                {
                    isMoving = true;

                    moveSpeed = 3f;

                    animator.SetInteger("toyolState", 1); // walk toyol

                    transform.position = Vector2.MoveTowards(this.transform.position, player.position, moveSpeed * Time.deltaTime);

                    if (transform.position.x > player.position.x)
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    if (transform.position.x < player.position.x)
                    {
                        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                else if (stunTime < 1f)
                {
                    isMoving = false;
                    moveSpeed = 0f;
                    stunTime += Time.deltaTime;
                }
            }
            else
            {
                isMoving = false;
            }
        }


        // Toyol Attack Range and Attack
        if (playerInRange() == true)
        {
            //Debug.Log("Detected");

            if (cooldownTimer >= attackCooldown)
            {
                //Debug.Log("Attacking");
                FindObjectOfType<audioManager>().Play("toyolSlash");
                cooldownTimer = 0;
                animator.SetInteger("toyolState", 2); // toyol attack
                attackTimer = 0;
                StartCoroutine(countSeconds());
            }
            cooldownTimer += Time.deltaTime;
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    public bool playerInRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(attackRange.bounds.center + transform.right * range, attackRange.bounds.size, 0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator countSeconds()
    {
        PlayerController player = indera.GetComponent<PlayerController>();
        yield return new WaitForSeconds(0.28f);
        if (playerInRange() == true)
            player.TakeDamage(toyolDamage);
        else
            Debug.Log("Toyol Missed");
    }

    private void OnDrawGizmosSelected()
    {
        //Detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);

        //Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackRange.bounds.center + transform.right * range, attackRange.bounds.size);
    }
}
