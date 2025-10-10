using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{
    public float awarenessRadius = 8;
    public bool isAggro;
    private Transform playerTransform;

    private void Start()
    {
        //playerTransform = FindObjectOfType<PlayerMove>().transform;
        playerTransform = Object.FindFirstObjectByType<PlayerMove>().transform;
    }

    public void Update()
    {
        var dist = Vector3.Distance(transform.position, playerTransform.position);

        if(dist < awarenessRadius)
        {
            isAggro = true;
        }
        if (isAggro)
        {
            // perhaps something later.
        }
    }

}
