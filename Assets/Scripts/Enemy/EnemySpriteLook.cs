using UnityEngine;

public class EnemySpriteLook : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform target;
    public bool canLookVertically;
    void Start()
    {
        //target = FindObjectOfType<PlayerMove>().transform;
        target = Object.FindFirstObjectByType<PlayerMove>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(canLookVertically)
        {
            transform.LookAt(target);
        }
        else
        {
            Vector3 modifiedTarget = target.position;
            modifiedTarget.y = transform.position.y;

            transform.LookAt(modifiedTarget);
        }
    }
}
