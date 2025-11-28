using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int health;

    public int maxArmor;
    private int armor;

    public GameObject damageEffect; // Player damaged effect

    [Header("Audio")]
    public AudioClip armorDamagedClip;  // Drag .wav file here
    public AudioClip healthDamagedClip; // Drag .wav file here
    private AudioSource audioSource;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        CanvasManager.Instance.UpdateHealth(health);
        CanvasManager.Instance.UpdateArmor(armor);

        // Get or add AudioSource for playing damage sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            DamagePlayer(30);
            Debug.Log("Player has been damaged!");
        }
    }
    public void DamagePlayer(int damage)
    {
        if (armor > 0)
        {
            if (armorDamagedClip != null)
                audioSource.PlayOneShot(armorDamagedClip);

            if (armor >= damage)
            {
                armor -= damage;
            }
            else if (armor < damage)
            {
                int remainingDamage;

                remainingDamage = damage - armor;

                armor = 0;

                health -= remainingDamage;
            }
        }
        else
        {
            if (healthDamagedClip != null)
                audioSource.PlayOneShot(healthDamagedClip);
            health -= damage;
        }
        if (health <= 0)
        {
            Debug.Log("Player is dead!");

            // Unlock cursor before returning to menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Load main menu scene
            SceneManager.LoadScene("Main Menu Scene IA");
        }

        CanvasManager.Instance.UpdateHealth(health);
        CanvasManager.Instance.UpdateArmor(armor);
    }
    public void GiveHealth(int amount, GameObject pickup)
    {
        if( health < maxHealth)
        {
            health += amount;
            Destroy(pickup);
        }


        if(health > maxHealth)
        {
            health = maxHealth;
        }

        CanvasManager.Instance.UpdateHealth(health);
    }
    public void GiveArmor(int amount, GameObject pickup)
    {
        //armor += amount;

        // if we use the pickup add the amount and destroy the Pikcup gameobject
        if (armor < maxArmor)
        {
            armor += amount;
            Destroy(pickup);
        }

        /// we check if that pickup just went over our max allowed amount
        /// set it to the max
        if (armor > maxArmor)
        {
            armor += maxArmor;
        }
        CanvasManager.Instance.UpdateArmor(armor);
    }

}