using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float damage = 15f;
    public float speed = 10f;
    public float lifetime = 5f;
    public GameObject owner;  // Boss that fired this

    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile collided with: {other.name} (Tag: {other.tag})");

        // Ignore the boss that fired this projectile
        if (owner != null && other.gameObject == owner)
        {
            Debug.Log("Ignored: Owner");
            return;
        }

        // Ignore children of owner (like EnemySprite)
        if (owner != null && other.transform.IsChildOf(owner.transform))
        {
            Debug.Log("Ignored: Child of owner");
            return;
        }

        // Check for player - either by tag OR by having PlayerHealth component
        // This handles Gun collider (child of player) not being tagged
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (other.CompareTag("Player") || playerHealth != null)
        {
            Debug.Log("Hit player - dealing damage");
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer((int)damage);
            }
            Destroy(gameObject);
            return;
        }

        // Hit walls/environment (not other enemies)
        if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Debug.Log($"Hit wall/environment: {other.name}");
            Destroy(gameObject);
        }
    }
}
