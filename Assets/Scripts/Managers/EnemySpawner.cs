using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnInterval = 2f;
    public float minDistanceFromPlayer = 5f;

    [Header("Spawn Area")]
    public Transform[] spawnPoints; // Optional: specific spawn points
    public bool useRandomPositionInRoom = true;
    public Vector3 roomSize = new Vector3(10f, 0f, 10f); // Size of spawn area if no spawn points

    [Header("Key Reward")]
    public int killsRequired = 10; // How many enemies to kill to get the key
    public string keyColor = "red"; // "red", "blue", or "green"
    public GameObject keyPickupPrefab; // Optional: spawn a key pickup instead of giving directly
    public Transform keySpawnPoint; // Where to spawn the key pickup

    [Header("State")]
    public bool isActive = false;
    public bool isCompleted = false; // Room has been cleared
    public bool startActiveOnPlay = false; // For testing - start spawning immediately
    public bool stopWhenPlayerLeaves = false; // Set true if you want spawning to stop when player exits
    public bool useDistanceDetection = true; // Use distance check instead of trigger

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Transform playerTransform;
    private Coroutine spawnCoroutine;
    private int totalKills = 0;
    private PlayerInventory playerInventory;

    private void Start()
    {
        // Find the player
        PlayerMove player = Object.FindFirstObjectByType<PlayerMove>();
        if (player != null)
        {
            playerTransform = player.transform;
            playerInventory = player.GetComponent<PlayerInventory>();
        }
        else
        {
           //Debug.LogWarning("EnemySpawner: Could not find player!");
        }

        if (enemyPrefab == null)
        {
            //Debug.LogError("EnemySpawner: No enemy prefab assigned!");
        }

        // Auto-start for testing
        if (startActiveOnPlay)
        {
            StartSpawning();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("EnemySpawner: Trigger entered by " + other.name + " (Tag: " + other.tag + ")");

        if (other.CompareTag("Player") && !isActive && !isCompleted)
        {
            //Debug.Log("EnemySpawner: Starting to spawn enemies!");
            StartSpawning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isActive && stopWhenPlayerLeaves)
        {
            StopSpawning();
        }
    }

    public void StartSpawning()
    {
        isActive = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        isActive = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isActive && !isCompleted)
        {
            // Count destroyed enemies as kills
            int previousCount = spawnedEnemies.Count;
            spawnedEnemies.RemoveAll(enemy => enemy == null);
            int enemiesKilled = previousCount - spawnedEnemies.Count;
            totalKills += enemiesKilled;

            // Check if player has killed enough enemies
            if (totalKills >= killsRequired)
            {
                CompleteRoom();
                yield break;
            }

            // Only spawn if under the limit
            if (spawnedEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void CompleteRoom()
    {
        isCompleted = true;
        isActive = false;

        // Clear remaining enemies (optional - remove if you want them to stay)
        // ClearAllEnemies();

        // Award the key
        if (keyPickupPrefab != null && keySpawnPoint != null)
        {
            // Spawn a key pickup for the player to collect
            Instantiate(keyPickupPrefab, keySpawnPoint.position, Quaternion.identity);
        }
        else if (playerInventory != null)
        {
            // Give the key directly to the player
            playerInventory.GiveKey(keyColor);
            CanvasManager.Instance.UpdateKeys(keyColor);
        }

        //Debug.Log("Room completed! " + keyColor + " key awarded!");
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
           // Debug.LogError("EnemySpawner: Cannot spawn - no enemy prefab assigned!");
            return;
        }

        Vector3 spawnPosition = GetValidSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(newEnemy);
            //Debug.Log("EnemySpawner: Spawned enemy at " + spawnPosition);
        }
        else
        {
            Debug.LogWarning("EnemySpawner: Failed to find valid spawn position!");
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidatePosition;

            if (spawnPoints != null && spawnPoints.Length > 0 && !useRandomPositionInRoom)
            {
                // Use predefined spawn points
                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                candidatePosition = randomPoint.position;
            }
            else
            {
                // Random position within room bounds
                candidatePosition = transform.position + new Vector3(
                    Random.Range(-roomSize.x / 2f, roomSize.x / 2f),
                    0f,
                    Random.Range(-roomSize.z / 2f, roomSize.z / 2f)
                );
            }

            // Check distance from player
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(candidatePosition, playerTransform.position);
                if (distanceToPlayer < minDistanceFromPlayer)
                {
                    continue; // Too close to player, try again
                }
            }

            // Verify position is on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePosition, out hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Fallback: couldn't find valid position
        Debug.LogWarning("EnemySpawner: Could not find valid spawn position after " + maxAttempts + " attempts");
        return Vector3.zero;
    }

    // Optional: Call this to clear all spawned enemies
    public void ClearAllEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    // Visualize the spawn area in editor
    private void OnDrawGizmosSelected()
    {
        // Color the spawn area based on key color
        Color areaColor = GetKeyGizmoColor();
        areaColor.a = 0.3f;
        Gizmos.color = areaColor;
        Gizmos.DrawCube(transform.position, new Vector3(roomSize.x, 2f, roomSize.z));

        // Wire outline
        areaColor.a = 1f;
        Gizmos.color = areaColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, 2f, roomSize.z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);

        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.5f);
                }
            }
        }

        // Show key spawn point
        if (keySpawnPoint != null)
        {
            Gizmos.color = GetKeyGizmoColor();
            Gizmos.DrawSphere(keySpawnPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, keySpawnPoint.position);
        }
    }

    private Color GetKeyGizmoColor()
    {
        switch (keyColor.ToLower())
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            case "green": return Color.green;
            default: return Color.white;
        }
    }
}
