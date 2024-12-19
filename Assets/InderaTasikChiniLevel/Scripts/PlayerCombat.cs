using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    private PlayerController playerControl;

    public GameObject windProjectile;
    public GameObject fireProjectile;
    public Transform castPoint;

    private float firetimeBtwCasts;
    public float firestartTimeBtwCasts;

    private float timeBtwCasts;
    public float startTimeBtwCasts;

    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    public float attackRate;
    private float nextAttackTime = 0f;
    private int comboCount = 0;

    bool attack1;
    bool attack2;
    bool attack3;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && timeBtwCasts <= 0)
        {
            FindObjectOfType<audioManager>().Play("castWind");
            animator.SetTrigger("castWind");
            Invoke("magic", 0.3f);
        }
        else
        {
            timeBtwCasts -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && firetimeBtwCasts <= 0)
        {
            FindObjectOfType<audioManager>().Play("castFire");
            animator.SetTrigger("castWind");
            Invoke("firemagic", 0.3f);
        }
        else
        {
            firetimeBtwCasts -= Time.deltaTime;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                FindObjectOfType<audioManager>().Play("swordSlash");
                slash();
                nextAttackTime = Time.time + 1f / attackRate;
                comboCount++;
                StartCoroutine(AttackMode());
            }

            if (Input.GetKeyDown(KeyCode.W) && comboCount > 1)
            {
                FindObjectOfType<audioManager>().Play("swordSlash");
                swipe();
                nextAttackTime = Time.time + 1f / attackRate;
                StartCoroutine(AttackMode());
            }

            if (Input.GetKeyDown(KeyCode.W) && comboCount > 2)
            {
                FindObjectOfType<audioManager>().Play("swordSlash");
                spin();
                nextAttackTime = Time.time + 1f / attackRate;
                comboCount++;
                StartCoroutine(AttackMode());
            }

            if (Input.GetKeyDown(KeyCode.W) && comboCount > 3)
            {
                FindObjectOfType<audioManager>().Play("swordSlash");
                slash();
                nextAttackTime = Time.time + 1f / attackRate;
                comboCount = 0;
                StartCoroutine(AttackMode());
            }
        }
    }

    void magic()
    {
            if (timeBtwCasts <= 0)
            {
                GameObject instant = Instantiate(windProjectile, castPoint.position, castPoint.rotation);
                timeBtwCasts = startTimeBtwCasts;
                Destroy(instant,1f);
            }
    }

    void firemagic()
    {
        if (firetimeBtwCasts <= 0)
        {
            GameObject instant = Instantiate(fireProjectile, castPoint.position, castPoint.rotation);
            firetimeBtwCasts = firestartTimeBtwCasts;
            Destroy(instant, 1f);
        }
    }

    void slash()
    {
        animator.SetTrigger("attack1");
        // Detect enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage each enemy
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            IDamageable enemy = enemyCollider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                //Debug.Log("Slash");
                enemy.TakeDamage(5);
                enemy.StunTime = 0f;
            }
        }
    }

    void swipe()
    {
        animator.SetTrigger("attack2");
        // Detect enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage each enemy
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            IDamageable enemy = enemyCollider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                //Debug.Log("Slash");
                enemy.TakeDamage(10);
                enemy.StunTime = 0f;
            }
        }
    }

    void spin()
    {
        animator.SetTrigger("attack3");
        // Detect enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage each enemy
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            IDamageable enemy = enemyCollider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                //Debug.Log("Slash");
                enemy.TakeDamage(15);
                enemy.StunTime = 0f;
            }
        }
    }

    private IEnumerator AttackMode()
    {
        playerControl = GetComponent<PlayerController>();
        playerControl.movingSpeed = 0f;
        yield return new WaitForSeconds(0.8f);
        playerControl.movingSpeed = 5f;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
