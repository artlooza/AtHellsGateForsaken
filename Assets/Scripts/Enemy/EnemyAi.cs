using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private EnemyAwareness enemyAwareness;
    private Transform playersTransform;
    private NavMeshAgent enemyNavMeshAgent;

    //[Header("Movement Settings")]
    public float moveSpeed = 2f;
    //public float aggroSpeedMultiplier = 1.5f;
    private void Start()
    {
        enemyAwareness = GetComponent<EnemyAwareness>();
        playersTransform = Object.FindFirstObjectByType<PlayerMove>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

        enemyNavMeshAgent.speed = moveSpeed;



    }

    private void Update()
    {
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
