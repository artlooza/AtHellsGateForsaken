using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator doorAnim;

    public GameObject areaToSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnim.SetTrigger("OpenDoor");

            // spawn enemies in the area
            areaToSpawn.SetActive(true);
        }
    }
}

