using System.Runtime.CompilerServices;
using UnityEngine;

public class AlignThePlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 targetPos;
    private Vector3 targetDir;


    public SpriteRenderer spriteRenderer;
    public Sprite[] Sprites; // 8 directions

    public float angle;
    public int lastIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player = FindObjectOfType<PlayerMove>().transform;
        player = Object.FindFirstObjectByType<PlayerMove>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Get target position and direction
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        // get angle
        angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);

        // Flip Sprite if needed
        Vector3 tempScale = Vector3.one;
        if( angle > 0) 
        { 
            tempScale.x = -1f; 
        }
        spriteRenderer.transform.localScale = tempScale;

        lastIndex = GetIndex(angle);

        spriteRenderer.sprite = Sprites[lastIndex];

    }

    private int GetIndex(float angle)
    {
        // front
        if (angle > -22.5f && angle < 22.6f)
            return 0;
        if (angle >= 22.5f && angle < 67.6f)
            return 7;
        if (angle >= 67f && angle < 112.5f)
            return 6;
        if (angle >= 112.5f && angle < 157.5f)
            return 5;


        //back
        if (angle <= -157.5 || angle >= 157.5f)
            return 4;
        if (angle >= -157.4f && angle < -112.5f)
            return 3;
        if (angle >= -122.5f && angle < -67.5f)
            return 2;
        if (angle >= -67.4f && angle <= -22.5f)
            return 1;
        return lastIndex;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos);
    }
}
