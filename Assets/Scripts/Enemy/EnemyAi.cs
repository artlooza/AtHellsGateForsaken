using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAi : MonoBehaviour
{
    private EnemyAwareness enemyAwareness;
    private Transform playersTransform;
    private NavMeshAgent enemyNavMeshAgent;

    //[Header("Movement Settings")]
    public float moveSpeed = 2f;
    //public float aggroSpeedMultiplier = 1.5f;
    private bool isInitialized = false;

    private void Start()
    {
        enemyAwareness = GetComponent<EnemyAwareness>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

        PlayerMove player = Object.FindFirstObjectByType<PlayerMove>();
        if (player != null)
        {
            playersTransform = player.transform;
        }

        // Delay NavMesh initialization until NavMesh is ready
        StartCoroutine(InitializeNavMeshAgent());
    }

    private IEnumerator InitializeNavMeshAgent()
    {
        // Wait until NavMesh is ready and agent can be placed
        while (!enemyNavMeshAgent.isOnNavMesh)
        {
            // Try to warp to current position on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                enemyNavMeshAgent.Warp(hit.position);
            }
            yield return new WaitForSeconds(0.1f);
        }

        enemyNavMeshAgent.speed = moveSpeed;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || !enemyNavMeshAgent.isOnNavMesh) return;

        if (enemyAwareness.isAggro)
        {
            enemyNavMeshAgent.SetDestination(playersTransform.position);
        }
        else
        {
            enemyNavMeshAgent.SetDestination(transform.position);
        }
    }
}
