using UnityEngine;

public class DoorSound : MonoBehaviour
{
    private AudioSource doorAudio;

    void Awake()
    {
        doorAudio = GetComponent<AudioSource>();
    }

    // Call this when the door starts opening
    public void PlayDoorOpenSound()
    {
        if (doorAudio != null && !doorAudio.isPlaying)
        {
            doorAudio.Play();
        }
    }
}
