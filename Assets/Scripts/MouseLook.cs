using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 1.5f;
    public float smoothing = 1.5f;

    private float xMousePos;
    private float smoothedMousePos;

    private float currentLookingPos;

    private void Start()
    {
       // lock and hide the cursor
       Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 

    }
    void Update()
    {
        GetInput();
        ModifyInput();
        MovePlayer();
    }

    void GetInput()
    {
        xMousePos += Input.GetAxis("Mouse X");
    }

    void ModifyInput()
    {

        xMousePos *= sensitivity * smoothing;
        smoothedMousePos = Mathf.Lerp(smoothedMousePos, xMousePos, 1f / smoothing);
    }

    void MovePlayer()
    {
        currentLookingPos += smoothedMousePos;
        transform.localRotation = Quaternion.AngleAxis(currentLookingPos, transform.up);
    }
}
