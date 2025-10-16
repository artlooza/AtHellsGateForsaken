using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer;


    private float enemyHealth = 2f;

    public float contactDamage = 20f; // damage dealt to player on contact
    public float damageRate = 1f; // how often damage is applied
    private float nextDamageTime = 0f; // time until next damage can be applied

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject gunHitEffect;
    void Start()
    {
        spriteAnim = GetComponentInChildren<Animator>();
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
            enemyManager.RemoveEnemy(this);
            Destroy(gameObject);
        }

        // any animations we call will have updated index

    }
    
    public void TakeDamage(float damage)
    {
        Instantiate(gunHitEffect, transform.position, Quaternion.identity);
        enemyHealth -= damage;
    }

    //// This'll be activated Enemy touches 
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy Triggered by " + other.name);
        if (other.CompareTag("Player"))
        {
            // Get the PlayerHealth component and call your existing DamagePlayer function
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer((int)contactDamage); // Call your existing function
            }
        }
    }


    // Handle contact damage to player
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Enemy Triggered by " + other.name + " with tag: " + other.tag);

    //    // Check if it's the player directly OR a child of the player (like Gun)
    //    PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

    //    // If no PlayerHealth on this object, check the parent
    //    if (playerHealth == null)
    //    {
    //        playerHealth = other.GetComponentInParent<PlayerHealth>();
    //    }

    //    // If still no PlayerHealth, check if this object has "Player" tag or parent does
    //    if (playerHealth == null && other.CompareTag("Player"))
    //    {
    //        playerHealth = other.GetComponent<PlayerHealth>();
    //    }

    //    if (playerHealth != null)
    //    {
    //        Debug.Log("PlayerHealth found! Dealing " + (int)contactDamage + " damage");
    //        playerHealth.DamagePlayer((int)contactDamage);
    //    }
    //    else
    //    {
    //        Debug.Log("No PlayerHealth found on " + other.name + " or its parent");
    //    }
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Enemy Triggered by " + other.name + " with tag: " + other.tag);

    //    // Only damage if it's specifically the Player, not the Gun or other child objects
    //    if (other.CompareTag("Player"))
    //    {
    //        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
    //        if (playerHealth != null)
    //        {
    //            Debug.Log("PlayerHealth found! Dealing " + (int)contactDamage + " damage");
    //            playerHealth.DamagePlayer((int)contactDamage);
    //        }
    //        else
    //        {
    //            Debug.Log("No PlayerHealth found on Player object");
    //        }
    //    }
    //    else if (other.name.Contains("Gun"))
    //    {
    //        Debug.Log("Gun detected, but not dealing contact damage - use a smaller enemy collider for contact damage");
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            // Continuous damage while touching
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer((int)contactDamage); // Call your existing function
            }

            nextDamageTime = Time.time + damageRate;
        }
    }




}
