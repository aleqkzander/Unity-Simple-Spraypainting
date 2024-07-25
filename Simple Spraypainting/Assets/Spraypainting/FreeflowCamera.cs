using UnityEngine;

public class FreeflowCamera : MonoBehaviour
{
    public float speed = 10.0f; // Movement speed
    public float lookSpeed = 2.0f; // Mouse look speed
    public bool freelookEnabled = true;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private void Start() => HandleFreelook(freelookEnabled);

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            switch (freelookEnabled)
            {
                case true:
                    freelookEnabled = false;
                    break;

                case false:
                    freelookEnabled = true;
                    break;
            }

            HandleFreelook(freelookEnabled);
        }

        if (freelookEnabled) 
            Move();
    }

    void Move()
    {
        // Mouse look
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Clamp the pitch value to avoid flipping
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Movement
        float moveForward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(moveRight, 0, moveForward);
    }

    void HandleFreelook(bool enabled)
    {
        if (enabled)
        {
            freelookEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            freelookEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}