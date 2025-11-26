using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 4;
    public float spawnInterval = 2f;
    public float minDistanceFromPlayer = 5f;

    [Header("Spawn Area")]
    public Transform[] spawnPoints; // Optional: specific spawn points
    public bool useRandomPositionInRoom = true;
    public Vector3 roomSize = new Vector3(10f, 0f, 10f); // Size of spawn area if no spawn points

    [Header("State")]
    public bool isActive = false;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Transform playerTransform;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        // Find the player
        PlayerMove player = Object.FindFirstObjectByType<PlayerMove>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            StartSpawning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
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
        while (isActive)
        {
            // Clean up destroyed enemies from list
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            // Only spawn if under the limit
            if (spawnedEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(newEnemy);
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
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(roomSize.x, 2f, roomSize.z));

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
    }
}
