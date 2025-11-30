using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 12f;
    public float momentumDamping = 5f;

    [Header("Sprint Settings")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    private CharacterController myCC;
    public Animator camAnim;
    private bool isWalking;
    private bool isSprinting;

    private float currentSpeed;
    private Vector3 inputVector;
    private Vector3 movementVector;
    private float myGravity = -10f;


    [Header("Audio")]
    public AudioClip footstepSound;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;
    private AudioSource audioSource;

    void Start()
    {
        myCC = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        GetInput();
        MovePlayer();
        HandleFootsteps();

        if (camAnim != null)
        {
            camAnim.SetBool("isWalking", isWalking);
            camAnim.SetBool("isRunning", isSprinting);
        }
    }

    void HandleFootsteps()
    {
        if (isWalking && myCC.isGrounded && footstepSound != null)
        {
            // Only play if not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.clip = footstepSound;
                audioSource.volume = footstepVolume;
                audioSource.Play();
            }
        }
        else
        {
            // Stop the sound when not walking
            if (audioSource.isPlaying && audioSource.clip == footstepSound)
            {
                audioSource.Stop();
            }
        }
    }

    void GetInput()
    {
        // if we're holding down wasd, then give us -1, 0, 1 values
        if(Input.GetKey(KeyCode.W) ||
           Input.GetKey(KeyCode.A) ||
           Input.GetKey(KeyCode.S) ||
           Input.GetKey(KeyCode.D))
        {
            inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            inputVector.Normalize();
            inputVector = transform.TransformDirection(inputVector);

            isWalking = true;

            // Check if sprinting (works in any direction while moving)
            isSprinting = Input.GetKey(sprintKey);
        }
        else
        {
            inputVector = Vector3.Lerp(inputVector, Vector3.zero, momentumDamping * Time.deltaTime);

            // Kill tiny residual movement to prevent drift
            if (inputVector.magnitude < 0.01f)
            {
                inputVector = Vector3.zero;
            }

            isWalking = false;
            isSprinting = false;
        }

        // Determine current speed based on sprint state
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        movementVector = (inputVector * currentSpeed) + (Vector3.up * myGravity);
    }

    void MovePlayer()
    {
        myCC.Move(movementVector * Time.deltaTime);
    }

}