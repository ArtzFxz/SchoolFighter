using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private bool facingRight;
    private bool previousDirectionRight;

    private bool isDead;

    private Transform target;

    private float enemySpeed = 0.3f;
    private float currentSpeed;

    private float verticalForce, horizontalForce;
    
    private bool isWalking = false;

    private float walkTimer;

    public int maxHealth;
    public int currentHealth;

    private float staggerTime = 0.5f;
    private bool isTakingDamage = false;
    private float damageTimer;

    private float attackRate = 1f;
    private float nextAttack;

    public Sprite enemyImage;

    public GameObject projectile;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Buscar o Player e armazenar sua posição
        target = FindAnyObjectByType<PlayerController>().transform;

        // Inicializar a velocidade do inimigo
        currentSpeed = enemySpeed;

        // Inicializar a vida do inimigo
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (target.position.x < this.transform.position.x)
        {
            facingRight = false;
        }
        else
        {
            facingRight = true;
        }

        if (facingRight && !previousDirectionRight)
        {
            this.transform.Rotate(0, 180, 0);
            previousDirectionRight = true;
        }

        if (!facingRight && previousDirectionRight)
        {
            this.transform.Rotate(0, -180, 0);
            previousDirectionRight = false;
        }

        walkTimer += Time.deltaTime;

        if (horizontalForce == 0 && verticalForce == 0)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }

        // Gereciar o tempo de stagger
        if (isTakingDamage && !isDead)
        {
            damageTimer += Time.deltaTime;

            ZeroSpeed();

            if (damageTimer >= staggerTime)
            {
                isTakingDamage = false;
                damageTimer = 0;

                ResetSpeed();
            }
        }

        // Atualiza o animator
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isWalking", isWalking);
    }

    private void ResetSpeed()
    {
        currentSpeed = enemySpeed;
    }

    private void ZeroSpeed()
    {
        currentSpeed = 0;
    }

    public void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        isTakingDamage = true;

        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        FindFirstObjectByType<UIManager>().UpdateEnemyUI(maxHealth, currentHealth, enemyImage);

        if (currentHealth <= 0)
        {
            isDead = true;

            rb.linearVelocity = Vector2.zero;

            animator.SetTrigger("Dead");
        }
    }

    public void FixedUpdate()
    {
        if (!isDead)
        {
            Vector3 targetDistance = target.position - this.transform.position;

            if (walkTimer >= Random.Range(2.5f, 3.5f))
            {
                verticalForce = targetDistance.y / Mathf.Abs(targetDistance.y);
                horizontalForce = targetDistance.x / Mathf.Abs(targetDistance.x);

                walkTimer = 0;
            }

            if (Mathf.Abs(targetDistance.x) < 1f)
            {
                horizontalForce = 0;
            }

            if (Mathf.Abs(targetDistance.y) < 0.05f)
            {
                verticalForce = 0;
            }

            if (!isTakingDamage)
            {
                rb.linearVelocity = new Vector2(horizontalForce * currentSpeed, verticalForce * currentSpeed);
            }

            //Lógica do ataque
            if (Mathf.Abs(targetDistance.x) < 1.3f && Mathf.Abs(targetDistance.y) < 0.05f && Time.time > nextAttack)
            {
                // Ataque do inimigo
                animator.SetTrigger("Attack");
                ZeroSpeed();

                nextAttack = Time.time + attackRate;
            }
        }
    }

    public void Shoot()
    {
        // Definir posiçao do spawn do projetil
        Vector2 spawnPosition = new Vector2(this.transform.position.x, this.transform.position.y + 0.2f);

        //Spawnar o projetil na posiçao definida 
        GameObject shotObject = Instantiate(projectile, spawnPosition, Quaternion.identity);

        //Ativar o projetil
        shotObject.SetActive(true);

        var shotPhysics = shotObject.GetComponent<Rigidbody2D>();

        if (facingRight)
        {
            //Aplica força no projetil para que haja o deslocamento para a direita
            shotPhysics.AddForceX(80f);
        }
        else
        {
            //Aplica força no projetil para que haja deslocamento para a esquerda
            shotPhysics.AddForceX(-80f);
        }
    }
}
