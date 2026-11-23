using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float SenX = 100f;
    public float SenY = 100f;

    public float xRotation;
    public float yRotation;

    public float strafeTiltAmount = 5f;
    public float forwardTiltAmount = 3f;
    public float tiltSmoothSpeed = 8f;

    private float currentTiltZ;
    private float currentTiltX;

    public float dashTiltAmount = 15f;
    public float dashTiltSmoothSpeed = 10f;
    public bool IsSprinting;

    private Vector3 currentDashTilt;
    private Vector3 targetDashTilt;

    public Transform Orientation;

    public static PlayerCamera Instance;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Instance = this;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SenX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SenY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (IsSprinting)
        {
            strafeTiltAmount = 3f;
            forwardTiltAmount = 5f;
        }
        else
        {
            strafeTiltAmount = 1f;
            forwardTiltAmount = 1f;
        }

        float targetTiltZ = -moveX * strafeTiltAmount;
        float targetTiltX = moveZ * forwardTiltAmount;

        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTiltZ, Time.deltaTime * tiltSmoothSpeed);

        currentTiltX = Mathf.Lerp(currentTiltX, targetTiltX, Time.deltaTime * tiltSmoothSpeed);

        currentDashTilt = Vector3.Lerp(currentDashTilt, targetDashTilt, Time.deltaTime * dashTiltSmoothSpeed);

        transform.rotation = Quaternion.Euler(xRotation + currentTiltX + currentDashTilt.x, yRotation, currentTiltZ + currentDashTilt.z);

        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void StartDashTilt()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        targetDashTilt.z = -moveX * dashTiltAmount;
    }

    public void StopDashTilt()
    {
        targetDashTilt = Vector3.zero;
    }
}
