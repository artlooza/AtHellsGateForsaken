using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Animation")]
    public Animator doorAnim;

    [Header("Key Requirements")]
    public bool requiresKey;
    public bool reqRed, reqGreen, reqBlue;

    [Header("Interaction")]
    public string interactionPrompt = "Press [E] to Open";
    public string lockedPrompt = "Requires {0} Key";

    [Header("Door State")]
    public bool stayOpen = false;  // Door won't close when player leaves
    public GameObject areaToSpawn;

    private bool playerInRange = false;
    private bool isOpen = false;
    private PlayerInventory playerInventory;

    void Update()
    {
        // Check for 'E' key press when player is in range
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            TryOpenDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            ShowInteractionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            CanvasManager.Instance.HideInteractionPrompt();

            // Close door when player leaves (unless stayOpen is true)
            if (isOpen && !stayOpen)
            {
                CloseDoor();
            }
        }
    }

    void ShowInteractionPrompt()
    {
        string promptMessage = interactionPrompt;

        // If door requires a key, check if player has it
        if (requiresKey)
        {
            bool hasRequiredKey = false;
            string requiredKeyColor = "";

            if (reqRed)
            {
                requiredKeyColor = "Red";
                hasRequiredKey = playerInventory != null && playerInventory.hasRed;
            }
            else if (reqGreen)
            {
                requiredKeyColor = "Green";
                hasRequiredKey = playerInventory != null && playerInventory.hasGreen;
            }
            else if (reqBlue)
            {
                requiredKeyColor = "Blue";
                hasRequiredKey = playerInventory != null && playerInventory.hasBlue;
            }

            // Show "Requires [Color] Key" if player doesn't have the key
            if (!hasRequiredKey)
            {
                promptMessage = string.Format(lockedPrompt, requiredKeyColor);
            }
        }

        CanvasManager.Instance.ShowInteractionPrompt(promptMessage);
    }

    void TryOpenDoor()
    {
        bool canOpen = true;

        // Check key requirements
        if (requiresKey)
        {
            canOpen = false;

            if (reqRed && playerInventory != null && playerInventory.hasRed)
            {
                canOpen = true;
            }
            else if (reqGreen && playerInventory != null && playerInventory.hasGreen)
            {
                canOpen = true;
            }
            else if (reqBlue && playerInventory != null && playerInventory.hasBlue)
            {
                canOpen = true;
            }

            // Play locked sound if can't open
            if (!canOpen)
            {
                GetComponent<AudioSource>()?.Stop();
                GetComponent<AudioSource>()?.Play();
                return;
            }
        }

        // Open the door
        if (canOpen)
        {
            isOpen = true;
            doorAnim.SetTrigger("OpenDoor");

            // Play open sound
            GetComponent<AudioSource>()?.Stop();
            GetComponent<AudioSource>()?.Play();

            // Spawn area if assigned
            if (areaToSpawn != null)
            {
                areaToSpawn.SetActive(true);
            }

            // Hide prompt immediately when door opens
            CanvasManager.Instance.HideInteractionPrompt();
        }
    }

    void CloseDoor()
    {
        if (doorAnim != null)
        {
            doorAnim.SetTrigger("CloseDoor");
            isOpen = false;
        }
    }
}
