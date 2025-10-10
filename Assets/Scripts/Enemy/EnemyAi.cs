using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private EnemyAwareness enemyAwareness;
    private Transform playersTransform;
    private NavMeshAgent enemyNavMeshAgent;
    private void Start()
    {
       enemyAwareness = GetComponent<EnemyAwareness>();
        //playersTransform = FindObjectOfType<PlayerMove>().transform;
        playersTransform = Object.FindFirstObjectByType<PlayerMove>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();

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
