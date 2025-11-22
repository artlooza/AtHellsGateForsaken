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


    void Start()
    {
        myCC = GetComponent<CharacterController>();

    }

    void Update()
    {
        GetInput();
        MovePlayer();

        camAnim.SetBool("isWalking", isWalking);
        camAnim.SetBool("isRunning", isSprinting);
    }

    void GetInput()
    {
        // Check if sprinting (only works while moving forward)
        isSprinting = Input.GetKey(sprintKey) && Input.GetKey(KeyCode.W);

        // Determine current speed based on sprint state
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

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
        }
        else
        {
            inputVector = Vector3.Lerp(inputVector, Vector3.zero, momentumDamping * Time.deltaTime);
            isWalking = false;
            isSprinting = false;
        }
        movementVector = (inputVector * currentSpeed) + (Vector3.up * myGravity);
    }

    void MovePlayer()
    {
        myCC.Move(movementVector * Time.deltaTime);
    }

}