using UnityEngine;

public class damagetrigger : MonoBehaviour

{
    private bool damagePlayer;
    private PlayerHealth playerHealth;

    public int damageAmount = 10;
    public float timeBetweenDamage = 1.5f;

    private float damageCounter;

    void Start()
    {
        damageCounter = timeBetweenDamage;
        playerHealth = FindObjectOfType<PlayerHealth>();
    }
    void update()
    {
        if (damagePlayer)
        {
            if (damageCounter > timeBetweenDamage)
            {

                playerHealth.DamagePlayer(damageAmount);

                damageCounter = 0f;
            }
            damageCounter += Time.deltaTime;
        }
        else
        {
            damageCounter = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageCounter = timeBetweenDamage;
            damagePlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damagePlayer = false;
        }
    }
}