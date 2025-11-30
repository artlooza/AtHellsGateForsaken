using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [Header("Phase System")]
    public int currentPhase = 1;
    public float phase1Health = 50f;
    public float phase2Health = 75f;
    private bool isTransitioning = false;
    private bool isInvulnerable = false;

    [Header("Attack Intervals")]
    public float attackDuration = 8f;
    public float reloadDuration = 3f;
    private float attackTimer = 0f;
    private bool isReloading = false;

    [Header("Weak Point")]
    public float weakPointMultiplier = 2f;
    public float weakPointAngle = 60f;

    [Header("Health")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("Contact Damage")]
    public float contactDamage = 30f;
    public float damageRate = 1f;
    private float nextDamageTime = 0f;

    [Header("Phase 1 - Fast Shots")]
    public float phase1AttackCooldown = 0.5f;
    public float phase1ProjectileSpeed = 12f;
    public float phase1Damage = 10f;

    [Header("Phase 2 - Bullet Hell")]
    public float phase2AttackCooldown = 0.3f;
    public float phase2ProjectileSpeed = 8f;
    public float phase2Damage = 20f;
    public int spiralProjectileCount = 12;
    public int ringProjectileCount = 16;
    public float patternDuration = 3f;
    private int currentPattern = 0;
    private float patternTimer = 0f;

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

    [Header("Transition Effects")]
    public float transitionDuration = 3f;
    public AudioClip powerUpSound;
    public Color powerUpFlashColor = Color.yellow;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("UI")]
    public string phase1Name = "Pablo";
    public string phase2Name = "Pablo the Destroyer";

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
        if (isDead || playerTransform == null || isTransitioning) return;

        // Update directional sprite animation (only when aggro)
        if (spriteAnim != null && angleToPlayer != null && isAggro)
            spriteAnim.SetFloat("spriteRot", angleToPlayer.lastIndex);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if player is within awareness radius
        if (!isAggro && distanceToPlayer <= awarenessRadius)
        {
            isAggro = true;
            CanvasManager.Instance.ShowBossHealthBar(this, phase1Name);
        }

        // Don't move or attack until aggro
        if (!isAggro) return;

        // Handle attack interval timing
        HandleAttackInterval();

        // Movement - try to maintain preferred distance (but not during reload!)
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (isReloading)
            {
                // During reload, stop moving - player's chance to flank!
                navAgent.SetDestination(transform.position);
            }
            else if (distanceToPlayer > preferredDistance + 2f)
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

        // Attack based on current phase (only if not reloading)
        if (!isReloading && distanceToPlayer <= attackRange)
        {
            if (currentPhase == 1)
            {
                Phase1Attack();  // Single fast shots
            }
            else
            {
                Phase2Attack();  // Bullet hell patterns
            }
        }

        // Check death (shouldn't happen in phase 1 due to transition)
        if (currentHealth <= 0 && !isDead && currentPhase == 2)
        {
            Die();
        }
    }

    void HandleAttackInterval()
    {
        attackTimer += Time.deltaTime;

        if (!isReloading)
        {
            // Currently attacking - check if time to reload
            if (attackTimer >= attackDuration)
            {
                isReloading = true;
                attackTimer = 0f;
                Debug.Log("[BOSS] Reloading... (Player's chance to attack!)");
            }
        }
        else
        {
            // Currently reloading - check if ready to attack again
            if (attackTimer >= reloadDuration)
            {
                isReloading = false;
                attackTimer = 0f;
                Debug.Log("[BOSS] Finished reloading - resuming attack!");
            }
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

    public void TakeDamage(float damage, Vector3? attackerPosition = null)
    {
        if (isDead || isInvulnerable) return;

        float finalDamage = damage;

        // Check for weak point hit (attacked from behind)
        if (attackerPosition.HasValue)
        {
            Vector3 toAttacker = (attackerPosition.Value - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toAttacker);

            // If attacker is behind the boss (angle > 180 - weakPointAngle)
            if (angle > 180f - weakPointAngle)
            {
                finalDamage *= weakPointMultiplier;
                Debug.Log($"[BOSS] WEAK POINT HIT! Damage: {finalDamage} (x{weakPointMultiplier})");
            }
        }

        Debug.Log($"[BOSS: {gameObject.name}] Taking {finalDamage} damage! Health before: {currentHealth}");

        currentHealth -= finalDamage;

        // Update health bar
        CanvasManager.Instance.UpdateBossHealth(currentHealth);

        Debug.Log($"[BOSS: {gameObject.name}] Health after: {currentHealth}");

        // Phase 1 -> Phase 2 transition
        if (currentPhase == 1 && currentHealth <= 0)
        {
            StartCoroutine(PhaseTransition());
            return;  // Don't die, transition instead
        }

        // Phase 2 death
        if (currentPhase == 2 && currentHealth <= 0)
        {
            Die();
            return;
        }

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

    private IEnumerator PhaseTransition()
    {
        isTransitioning = true;
        isInvulnerable = true;

        // Stop movement during transition
        if (navAgent != null)
            navAgent.isStopped = true;

        // Play power-up sound
        if (powerUpSound != null)
            audioSource.PlayOneShot(powerUpSound);

        Debug.Log("[BOSS] Phase 1 complete! Transitioning to Phase 2...");

        // Trigger UI transformation DURING the flash (name change + bar growth starts)
        CanvasManager.Instance.UpdateBossPhase(2, phase2Name);

        // Flashy power-up effect (pulse colors)
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            // Alternate between normal and power-up color
            spriteRenderer.color = (Mathf.Sin(elapsed * 10f) > 0) ? powerUpFlashColor : Color.white;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to normal color
        spriteRenderer.color = Color.white;

        // Enter Phase 2
        currentPhase = 2;
        currentHealth = phase2Health;

        // Update health bar to show full health
        CanvasManager.Instance.UpdateBossHealth(currentHealth);

        // Resume movement
        if (navAgent != null)
            navAgent.isStopped = false;

        isTransitioning = false;
        isInvulnerable = false;

        Debug.Log("[BOSS] Entered Phase 2! BULLET HELL ACTIVATED!");
    }

    void Phase1Attack()
    {
        nextAttackTime = Time.time + phase1AttackCooldown;
        Vector3 direction = (playerTransform.position - firePoint.position).normalized;
        SpawnProjectile(direction, phase1ProjectileSpeed, phase1Damage, false);

        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }

    void Phase2Attack()
    {
        // Update pattern timer
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + phase2AttackCooldown;

            // Cycle through patterns
            patternTimer += Time.deltaTime;
            if (patternTimer >= patternDuration)
            {
                patternTimer = 0f;
                currentPattern = (currentPattern + 1) % 3;  // Cycle through 3 patterns
                Debug.Log($"[BOSS] Switching to pattern {currentPattern}");
            }

            // Execute current pattern
            switch (currentPattern)
            {
                case 0: SpiralAttack(); break;
                case 1: RingBurstAttack(); break;
                case 2: HomingAttack(); break;
            }

            if (attackSound != null)
                audioSource.PlayOneShot(attackSound);
        }
    }

    void SpiralAttack()
    {
        // Fire projectiles in a rotating spiral pattern
        float currentAngle = Time.time * 100f;  // Rotate over time

        for (int i = 0; i < 2; i++)  // 2 spiral arms
        {
            float angle = currentAngle + (i * 180f);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            SpawnProjectile(direction, phase2ProjectileSpeed, phase2Damage, false);
        }
    }

    void RingBurstAttack()
    {
        // Fire projectiles in all directions at once
        float angleStep = 360f / ringProjectileCount;

        for (int i = 0; i < ringProjectileCount; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            SpawnProjectile(direction, phase2ProjectileSpeed, phase2Damage, false);
        }
    }

    void HomingAttack()
    {
        // Fire homing projectiles at player
        Vector3 direction = (playerTransform.position - firePoint.position).normalized;
        SpawnProjectile(direction, phase2ProjectileSpeed * 0.7f, phase2Damage, true);  // Slower but homing
    }

    void SpawnProjectile(Vector3 direction, float speed, float damage, bool isHoming)
    {
        if (projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        BossProjectile bp = projectile.GetComponent<BossProjectile>();
        if (bp != null)
        {
            bp.damage = damage;
            bp.speed = speed;
            bp.owner = gameObject;
            bp.isHoming = isHoming;
            bp.target = isHoming ? playerTransform : null;
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log($"[BOSS: {gameObject.name}] DIED!");

        // Hide health bar when boss dies
        CanvasManager.Instance.HideBossHealthBar();

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
