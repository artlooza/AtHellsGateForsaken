using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public bool isRedKey, isGreenKey, isBlueKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(isRedKey)
            {
                other.GetComponent<PlayerInventory>().hasRed = true;
            }
            if(isGreenKey)
            {
                other.GetComponent<PlayerInventory>().hasGreen = true;
            }
            if(isBlueKey)
            {
                other.GetComponent<PlayerInventory>().hasBlue = true;
            }
            Destroy(gameObject);
        }
    }
}
