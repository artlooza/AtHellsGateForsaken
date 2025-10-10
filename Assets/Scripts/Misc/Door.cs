using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator doorAnim;

    public bool requiresKey;
    public bool reqRed, reqGreen, reqBlue;
    public GameObject areaToSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(requiresKey)
            {
                // do additioanl checks.
                if(reqRed && other.GetComponent<PlayerInventory>().hasRed)
                {
                    doorAnim.SetTrigger("OpenDoor");
                    areaToSpawn.SetActive(true);
                }
                if(reqBlue && other.GetComponent<PlayerInventory>().hasBlue)
                {
                    doorAnim.SetTrigger("OpenDoor");
                    areaToSpawn.SetActive(true);
                }
                if(reqGreen && other.GetComponent<PlayerInventory>().hasGreen)
                {
                    doorAnim.SetTrigger("OpenDoor");
                    areaToSpawn.SetActive(true);
                }          
            }
            else
            {
                doorAnim.SetTrigger("OpenDoor");
                areaToSpawn.SetActive(true);

            }
            // spawn enemies in the area
        }
    }
}

