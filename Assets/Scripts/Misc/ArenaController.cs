using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    [Header("Arena Setup")]
    public Door exitDoor;                    // The door that locks when player enters
    public GameObject[] prePlacedEnemies;    // Enemies already in the arena
    public EnemySpawner spawner;             // Optional: spawner for additional enemies

    [Header("Arena Size")]
    public Vector3 arenaSize = new Vector3(20f, 5f, 20f);  // Width, Height, Depth

    [Header("Lock Settings")]
    public bool lockOnEntry = true;
    public string lockedMessage = "Door is locked! Defeat all enemies to escape.";

    [Header("Key Reward (Optional)")]
    public bool dropKeyOnComplete = false;
    public string keyColor = "red";  // "red", "blue", "green"
    public GameObject keyPickupPrefab;  // Optional: physical key pickup
    public Transform keySpawnPoint;  // Where key spawns (defaults to arena center)

    [Header("Audio (Optional)")]
    public AudioSource battleMusicSource;  // Music that plays during combat
    public AudioSource completionJingleSource;  // Jingle/sound when arena completes

    [Header("State")]
    public bool arenaActive = false;
    public bool arenaCompleted = false;

    private List<GameObject> allEnemies = new List<GameObject>();

    private void Start()
    {
        // Sync BoxCollider size with arena size
        UpdateColliderSize();

        // Add pre-placed enemies to tracking list
        if (prePlacedEnemies != null)
        {
            foreach (var enemy in prePlacedEnemies)
            {
                if (enemy != null)
                {
                    allEnemies.Add(enemy);
                }
            }
        }
    }

    private void UpdateColliderSize()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.size = arenaSize;
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !arenaActive && !arenaCompleted)
        {
            StartArena();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player exited arena (no action needed currently)
    }

    private void Update()
    {
        if (arenaActive && !arenaCompleted)
        {
            CheckArenaCompletion();
        }
    }

    private void StartArena()
    {
        arenaActive = true;

        // Lock the exit door
        if (exitDoor != null && lockOnEntry)
        {
            exitDoor.LockDoor(lockedMessage);
        }

        // Play battle music
        if (battleMusicSource != null)
        {
            battleMusicSource.Play();
        }

        // Start the spawner if assigned
        if (spawner != null)
        {
            spawner.gameObject.SetActive(true);  // Activate the spawner first
            spawner.StartSpawning();
        }

        Debug.Log("[ArenaController] Arena started! Door locked.");
    }

    private void CheckArenaCompletion()
    {
        // Remove destroyed enemies from tracking
        allEnemies.RemoveAll(enemy => enemy == null);

        // Check if spawner has enemies too (if assigned)
        int totalEnemies = allEnemies.Count;

        if (spawner != null)
        {
            // If spawner isn't complete yet, don't check for arena completion
            if (!spawner.isCompleted)
            {
                return;
            }
        }

        // All enemies defeated?
        if (totalEnemies == 0)
        {
            CompleteArena();
        }
    }

    private void CompleteArena()
    {
        arenaCompleted = true;
        arenaActive = false;

        // Stop battle music and play completion jingle
        if (battleMusicSource != null && battleMusicSource.isPlaying)
        {
            battleMusicSource.Stop();
        }

        if (completionJingleSource != null)
        {
            completionJingleSource.Play();
        }

        // Unlock the exit door
        if (exitDoor != null)
        {
            exitDoor.UnlockDoor();
        }

        // Award key if enabled
        if (dropKeyOnComplete)
        {
            if (keyPickupPrefab != null)
            {
                // Spawn physical key pickup
                Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position;
                Instantiate(keyPickupPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"[ArenaController] Spawned {keyColor} key at {spawnPos}");
            }
            else
            {
                // Give key directly to player
                PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.GiveKey(keyColor);
                    CanvasManager.Instance.UpdateKeys(keyColor);
                    Debug.Log($"[ArenaController] Gave {keyColor} key directly to player");
                }
            }
        }

        Debug.Log("[ArenaController] Arena completed! Door unlocked.");
    }

    // Public method to add dynamically spawned enemies
    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
        }
    }

    // Update collider size when values change in Inspector
    private void OnValidate()
    {
        UpdateColliderSize();
    }

    // Visualize arena bounds in editor
    private void OnDrawGizmos()
    {
        // Apply transform matrix to respect rotation
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw semi-transparent arena volume
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);  // Red with transparency
        Gizmos.DrawCube(Vector3.zero, arenaSize);

        // Draw wire outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, arenaSize);

        // Reset matrix for world-space drawing
        Gizmos.matrix = Matrix4x4.identity;

        // Draw line to exit door
        if (exitDoor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, exitDoor.transform.position);

            // Draw sphere at door
            Gizmos.DrawWireSphere(exitDoor.transform.position, 0.5f);
        }

        // Draw spheres for pre-placed enemies
        if (prePlacedEnemies != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var enemy in prePlacedEnemies)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                    Gizmos.DrawWireSphere(enemy.transform.position, 0.3f);
                }
            }
        }
    }

    // Draw selected gizmo with better visibility
    private void OnDrawGizmosSelected()
    {
        // Apply transform matrix to respect rotation
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw solid arena volume when selected
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);  // Orange with transparency
        Gizmos.DrawCube(Vector3.zero, arenaSize);

        // Draw thick wire outline
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, arenaSize);

        // Reset matrix
        Gizmos.matrix = Matrix4x4.identity;
    }
}
