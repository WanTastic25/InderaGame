using System;
using System.Collections;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        //Declarations
        public Animator animator;
        private Rigidbody2D rb;
        public LayerMask groundLayers;
        [SerializeField] Collider2D standingCollider;

        private Transform currentSpawnPoint;
        [SerializeField] GameObject indera;

        public float movingSpeed;
        private float moveInput;
        private bool facingRight = true;
        
        public float groundCheckRadius;
        private bool isGrounded;
        public float jumpForce;
        public Transform groundCheck;
        
        public float crouchSpeed;
        public Transform overheadCheck;
        public float overheadCheckRadius;

        public float dashingPower;
        public float dashingTime;
        public float dashingCooldown;

        public float timeBtwSpawns;
        public float startTimeBtwSpawns;
        [SerializeField] GameObject AfterImage;

        public float extraJumpValue;
        private float extraJump;

        public float invincibilityTimer = 1.5f;

        public float inderaHealth = 100;

        public float currentSpeed;

        public HealthBarScript healthBar;

        public bossHealthBar bossBar;

        public collectibleManager stuff;

        public AudioSource footsteps;

        //Booleans
        bool grounded;
        bool crouch;
        bool canDash = true;
        bool isMoving = false;
        bool isDashing;
        bool isDead;

        // Start is called before the first frame update
        void Start()
        {
            //Get Rigidbody
            rb = GetComponent<Rigidbody2D>();
            
            //Get Animator
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        // Update is called once per frame
        void Update()
        {
            healthBar.setHealth(inderaHealth); //Player Health Bar

            invincibilityTimer += Time.deltaTime;
            
            if (isDashing)
            {
                createAfterImage();
                Physics2D.IgnoreLayerCollision(7, 8, true);
                return;
            }
            else if (isDashing == false)
            {
                Physics2D.IgnoreLayerCollision(7, 8, false);
            }

            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);

            if (isMoving == true && grounded && !footsteps.isPlaying)
            {
                footsteps.Play();
            }
            else if (isMoving == false || !grounded)
            {
                footsteps.Stop();
            }

            //Check for overheads when crouching
            bool overlapcheck = Physics2D.OverlapCircle(overheadCheck.position, overheadCheckRadius, groundLayers);

            if (!isDead)
            {
                //Crounching and Idling
                if ((Input.GetButtonDown("Crouch") && grounded && isGrounded) || (overlapcheck && grounded && isGrounded))
                {
                    crouch = true;
                    standingCollider.enabled = false;
                    animator.SetInteger("playerState", 2); // crouch animation for crouch

                }
                else if (Input.GetButtonUp("Crouch") && !overlapcheck && grounded)
                {
                    crouch = false;
                    standingCollider.enabled = true;
                    animator.SetInteger("playerState", 0); // idle animation
                }

                //Moving Left or Right
                if (Input.GetButtonUp("Horizontal") && crouch == true)
                {
                    isMoving = false;
                    animator.SetInteger("playerState", 2); // Idle Animation
                }
                else if (Input.GetButton("Horizontal"))
                {
                    isMoving = true;
                    moveInput = Input.GetAxis("Horizontal");
                    currentSpeed = crouch ? crouchSpeed : movingSpeed;
                    Vector3 direction = new Vector3(moveInput, 0, 0);
                    transform.position += direction * currentSpeed * Time.deltaTime;

                    if (Input.GetKey(KeyCode.DownArrow) && grounded && isGrounded || (overlapcheck && grounded && isGrounded))
                    {
                        crouch = true;
                        animator.SetInteger("playerState", 3); // crouchwalk Animation
                    }
                    else
                    {
                        standingCollider.enabled = true;
                        crouch = false;
                        animator.SetInteger("playerState", 1); // Run Animation
                    }

                }
                else if (Input.GetButtonUp("Horizontal") && crouch == false)
                {
                    isMoving = false;
                    animator.SetInteger("playerState", 0); // Idle Animation
                }

                //Jumping and double jumping
                if (grounded == true)
                {
                    extraJump = extraJumpValue;
                }
                if (Input.GetButton("Jump") && grounded && isGrounded)
                {
                    FindObjectOfType<audioManager>().Play("jumping");
                    rb.velocity = Vector2.up * jumpForce;
                }
                else if (Input.GetButtonDown("Jump") && !grounded && extraJump == 1)
                {
                    FindObjectOfType<audioManager>().Play("doublejump");
                    animator.SetInteger("playerState", 0);
                    rb.velocity = Vector2.up * jumpForce;
                    extraJump--;
                }
                if (!grounded)
                {
                    animator.SetInteger("playerState", 4); //Jump Animation
                }

                //Dodging and Dashing
                if (Input.GetButton("Horizontal") && Input.GetKey(KeyCode.F) && canDash)
                {
                    StartCoroutine(Dash());
                }

                //Flipping
                if (moveInput < 0 && facingRight || moveInput > 0 && !facingRight)
                {
                    Flip();
                }
            }
            else if (isDead)
            {
                isMoving = false;
            }

        }

        private IEnumerator Dash()
        {
            FindObjectOfType<audioManager>().Play("dashing");
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.right.x * dashingPower, 0f);
            yield return new WaitForSeconds(dashingTime);
            rb.velocity = Vector2.zero;
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
            /*Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;*/
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
            isGrounded = colliders.Length > 1;
        }

        private void createAfterImage()
        {
            if (timeBtwSpawns <= 0)
            {
                GameObject instance = Instantiate(AfterImage, transform.position, transform.rotation);
                instance.transform.localScale = this.transform.localScale;
                Sprite currentSPrite = GetComponent<SpriteRenderer>().sprite;
                instance.GetComponent<SpriteRenderer>().sprite = currentSPrite;
                Destroy(instance, 2f);
                timeBtwSpawns = startTimeBtwSpawns;
            }
            else
            {
                timeBtwSpawns -= Time.deltaTime;
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDashing)
            {
                Debug.Log("Dodged");
                Physics2D.IgnoreLayerCollision(7, 8, true);
            }
            else
            {
                if (invincibilityTimer >= 1.5f)
                {
                    FindObjectOfType<audioManager>().Play("playerHit");
                    animator.SetTrigger("damaged");
                    inderaHealth -= damage;
                    invincibilityTimer = 0f;
                    Debug.Log("Health = " + inderaHealth);
                }
            }

            if (inderaHealth <= 0f && !isDead)
            {
                Debug.Log("Health 0");
                StartCoroutine(Die());
            }
        }

        public IEnumerator Die()
        {
            FindObjectOfType<audioManager>().Play("playerDeath");
            animator.SetTrigger("dead");
            inderaHealth = 0f;
            isDead = true;

            yield return new WaitForSeconds(2.5f);
    
            transform.position = currentSpawnPoint.position;
            FindObjectOfType<audioManager>().Play("campFire");
            isDead = false;
            inderaHealth = 100f;
            animator.SetInteger("playerState",0);
        }

        //Trigger Stuff
        private void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (hitInfo.transform.tag == "Checkpoint")
            {
                FindObjectOfType<audioManager>().Play("campFire");
                currentSpawnPoint = hitInfo.transform;
                hitInfo.GetComponent<Animator>().SetTrigger("Ignite");
                hitInfo.GetComponent<Collider2D>().enabled = false;
            }

            if (hitInfo.transform.tag == "Respawner")
            {
                transform.position = currentSpawnPoint.position;
            }

            if (hitInfo.transform.tag == "bossStarter")
            {
                FindObjectOfType<audioManager>().Play("bossBegin"); //BossMusic
                bossBar.activateUI();
                Destroy(hitInfo.gameObject);
            }

            if (hitInfo.transform.tag == "Collectible")
            {
                FindObjectOfType<audioManager>().Play("collecting");
                Destroy(hitInfo.gameObject);
                stuff.collectibleCounter += 1;
            }
        }
    }
}
