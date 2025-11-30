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
        //Debug.Log($"Projectile collided with: {other.name} (Tag: {other.tag})");

        // Ignore the boss that fired this projectile
        if (owner != null && other.gameObject == owner)
        {
            //Debug.Log("Ignored: Owner");
            return;
        }

        // Ignore children of owner (like EnemySprite)
        if (owner != null && other.transform.IsChildOf(owner.transform))
        {
            //Debug.Log("Ignored: Child of owner");
            return;
        }

        // Only damage if we hit the actual player body (object with PlayerHealth directly on it)
        // Ignore child objects like the gun
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            //Debug.Log("Hit player body - dealing damage");
            playerHealth.DamagePlayer((int)damage);
            Destroy(gameObject);
            return;
        }

        // If we hit a Player-tagged object without PlayerHealth (like the Gun), ignore it
        if (other.CompareTag("Player"))
        {
            //Debug.Log($"Ignored player child object: {other.name}");
            return;  // Don't destroy - let projectile pass through
        }

        // Hit walls/environment (not other enemies)
        if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            //Debug.Log($"Hit wall/environment: {other.name}");
            Destroy(gameObject);
        }
    }
}
