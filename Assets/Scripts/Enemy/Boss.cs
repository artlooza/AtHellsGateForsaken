using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Contact Damage")]
    public float contactDamage = 30f;
    public float damageRate = 1f;
    private float nextDamageTime = 0f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 15f;
    public float attackCooldown = 2f;
    public float projectileDamage = 15f;
    public float projectileSpeed = 10f;
    private float nextAttackTime = 0f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float preferredDistance = 8f; // Boss tries to keep this distance from player

    [Header("Awareness")]
    public float awarenessRadius = 15f;
    public bool isAggro = false;

    [Header("Effects")]
    public GameObject hitEffect;
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    public float deathAnimationLength = 1f;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private EnemyManager enemyManager;
    private SpriteRenderer spriteRenderer;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer;
    private Transform playerTransform;
    private NavMeshAgent navAgent;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteAnim = GetComponentInChildren<Animator>();
        angleToPlayer = GetComponent<AngleToPlayer>();
        enemyManager = FindFirstObjectByType<EnemyManager>();

        PlayerMove player = FindFirstObjectByType<PlayerMove>();
        if (player != null)
            playerTransform = player.transform;

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
            navAgent.speed = moveSpeed;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = Vector3.forward * 3f + Vector3.up * 1f;
            firePoint = fp.transform;
        }
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        // Update directional sprite animation (only when aggro)
        if (spriteAnim != null && angleToPlayer != null && isAggro)
            spriteAnim.SetFloat("spriteRot", angleToPlayer.lastIndex);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if player is within awareness radius
        if (!isAggro && distanceToPlayer <= awarenessRadius)
            isAggro = true;

        // Don't move or attack until aggro
        if (!isAggro) return;

        // Movement - try to maintain preferred distance
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (distanceToPlayer > preferredDistance + 2f)
            {
                // Move closer
                navAgent.SetDestination(playerTransform.position);
            }
            else if (distanceToPlayer < preferredDistance - 2f)
            {
                // Move away
                Vector3 awayDir = (transform.position - playerTransform.position).normalized;
                Vector3 targetPos = transform.position + awayDir * 3f;
                navAgent.SetDestination(targetPos);
            }
            else
            {
                // Stay in place, face player
                navAgent.SetDestination(transform.position);
            }
        }

        // Always look at player
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        // Ranged attack
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
        }

        // Check death
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Attack()
    {
        if (projectilePrefab == null) return;

        nextAttackTime = Time.time + attackCooldown;

        // Play attack sound
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        // Spawn projectile
        Vector3 direction = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        BossProjectile bp = projectile.GetComponent<BossProjectile>();
        if (bp != null)
        {
            bp.damage = projectileDamage;
            bp.speed = projectileSpeed;
            bp.owner = gameObject;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log($"[BOSS: {gameObject.name}] Taking {damage} damage! Health before: {currentHealth}");

        currentHealth -= damage;

        //Debug.Log($"[BOSS: {gameObject.name}] Health after: {currentHealth}");

        // Hit effect
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Play hurt sound
        if (hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        // Flash red
        if (spriteRenderer != null)
            StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        isDead = true;

        Debug.Log($"[BOSS: {gameObject.name}] DIED!");

        // Trigger death animation
        if (spriteAnim != null)
            spriteAnim.SetBool("isDead", true);

        // Stop movement
        if (navAgent != null)
            navAgent.isStopped = true;

        // Play death sound
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Remove from enemy manager
        Enemy enemyComponent = GetComponent<Enemy>();
        if (enemyComponent != null && enemyManager != null)
            enemyManager.RemoveEnemy(enemyComponent);

        // Destroy after death animation finishes
        Destroy(gameObject, deathAnimationLength);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only damage if we hit the actual player body, not child objects like the gun
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.DamagePlayer((int)contactDamage);
    }

    private void OnTriggerStay(Collider other)
    {
        // Only damage if we hit the actual player body, not child objects like the gun
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && Time.time >= nextDamageTime)
        {
            playerHealth.DamagePlayer((int)contactDamage);
            nextDamageTime = Time.time + damageRate;
        }
    }

    // Visualize ranges in editor
    private void OnDrawGizmosSelected()
    {
        // Awareness radius (when boss wakes up)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Preferred distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);
    }
}
