using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossController : MonoBehaviour, IDamageable
{
    public float StunTime
    {
        get { return stunTime; }
        set { stunTime = value; }
    }

    public int health;
    [SerializeField] GameObject bossDeath; //death animation

    public Transform player;
    public float lineOfSight;
    public float moveSpeed;

    public float attackCooldown;
    [SerializeField] public float range;
    [SerializeField] public float colliderPosition;
    public int bossDamage;
    [SerializeField] BoxCollider2D attackRange;
    [SerializeField] LayerMask playerLayer;
    private float cooldownTimer = 2;

    public float attackTimer;
    public bool canWalk;
    public float stunTime;

    [SerializeField] GameObject wall;

    public Animator animator;
    public GameObject indera;

    public bossHealthBar healthBar;

    public void TakeDamage(int damage)
    {
        FindObjectOfType<audioManager>().Play("bossHit");

        health -= damage;
        animator.SetTrigger("bossHit");

        if (health <= 0)
        {
            Die();
            FindObjectOfType<audioManager>().Stop("bossBegin");
        }
    }

    public void Die()
    {
        healthBar.setHealth(health); //Boss health Bar
        healthBar.Start();
        GameObject instance = Instantiate(bossDeath, transform.position, transform.rotation);
        FindObjectOfType<audioManager>().Play("bossDie");

        Destroy(gameObject);
        Destroy(wall);
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
        healthBar.setHealth(health); //Boss health Bar

        // Boss Movement and Player Detection
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        animator.SetInteger("bossState", 0); // idle boss
        //cooldownTimer += Time.deltaTime;

        if (attackTimer < 1.5f)
        {
            moveSpeed = 0f;
            attackTimer += Time.deltaTime;
        }
        else
        {
            if (distanceFromPlayer < lineOfSight && playerInRange() == true)
            {
                moveSpeed = 0f;
            }
            else if (distanceFromPlayer < lineOfSight)
            {
                if (stunTime >= 1f)
                {
                    moveSpeed = 3f;

                    animator.SetInteger("bossState", 1); // walk boss

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
                else if (stunTime < 1f)
                {
                    moveSpeed = 0f;
                    stunTime += Time.deltaTime;
                }
            }
        }


        // Boss Attack Range and Attack
        if (playerInRange() == true)
        {
            Debug.Log("Detected");

            if (cooldownTimer >= attackCooldown && stunTime >= 1f)
            {
                //Debug.Log("Attacking");
                FindObjectOfType<audioManager>().Play("bossAttack");
                FindObjectOfType<audioManager>().Play("bossGrunt");
                cooldownTimer = 0;
                animator.SetInteger("bossState", 2); // boss attack
                attackTimer = 0;
                StartCoroutine(countSeconds());
            }
            cooldownTimer += Time.deltaTime;
            stunTime += Time.deltaTime;
        }
        else
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    public bool playerInRange()
    {
        RaycastHit2D hit = 
            Physics2D.BoxCast(attackRange.bounds.center + transform.right * range * transform.localScale.x * colliderPosition, 
            new Vector3(attackRange.bounds.size.x * range, attackRange.bounds.size.y, attackRange.bounds.size.z), 
            0, Vector2.left, 0, playerLayer);

        //Debug.Log(hit.collider != null);

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
        yield return new WaitForSeconds(0.55f);
        if (playerInRange() == true && stunTime >= 1.55f)
        {
            player.TakeDamage(bossDamage);
            Debug.Log("Boss should kill");
        }
        else
        {
            stunTime += Time.deltaTime;
            Debug.Log("Boss Missed");
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);

        //Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackRange.bounds.center + transform.right * range * transform.localScale.x * colliderPosition,
            new Vector3(attackRange.bounds.size.x * range, attackRange.bounds.size.y, attackRange.bounds.size.z));
    }
}
