using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer;
    private SpriteRenderer spriteRenderer;


    private float enemyHealth = 2f;

    public float contactDamage = 20f; // damage dealt to player on contact
    public float damageRate = 1f; // how often damage is applied
    private float nextDamageTime = 0f; // time until next damage can be applied

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject gunHitEffect;
    public float flashDuration = 0.1f; // how long the red flash lasts
    public Color flashColor = Color.red; // color to flash

    void Start()
    {
        spriteAnim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        angleToPlayer = GetComponent<AngleToPlayer>();

        enemyManager = UnityEngine.Object.FindFirstObjectByType<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // beginning of update set the animations rotational index
        spriteAnim.SetFloat("spriteRot", angleToPlayer.lastIndex);
        if (enemyHealth <= 0)
        {
            Debug.Log($"[{gameObject.name}] DIED!");
            enemyManager.RemoveEnemy(this);
            Destroy(gameObject);
        }

        // any animations we call will have updated index

    }
    
    public void TakeDamage(float damage)
    {
        Debug.Log($"[{gameObject.name}] Taking {damage} damage! Health before: {enemyHealth}");

        Instantiate(gunHitEffect, transform.position, Quaternion.identity);
        enemyHealth -= damage;

        Debug.Log($"[{gameObject.name}] Health after: {enemyHealth}");

        // Flash red when taking damage
        if(spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        // Store original color
        Color originalColor = spriteRenderer.color;

        // Flash to red
        spriteRenderer.color = flashColor;

        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);

        // Return to original color
        spriteRenderer.color = originalColor;
    }

    // This'll be activated Enemy touches
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Triggered by " + other.name);
        // Only damage if we hit the actual player body, not child objects like the gun
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.DamagePlayer((int)contactDamage); // Call your existing function
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Only damage if we hit the actual player body, not child objects like the gun
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null && Time.time >= nextDamageTime)
        {
            playerHealth.DamagePlayer((int)contactDamage); // Call your existing function
            nextDamageTime = Time.time + damageRate;
        }
    }




}
