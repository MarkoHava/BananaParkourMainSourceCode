
using UnityEngine;

public class MouseHandler : MonoBehaviour {
    public static MouseHandler Singleton;
    private void Awake()
    {
        Singleton = this;
    }
    private Vector2 CamRotation;
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject Player;
    [SerializeField] private int CameraYRestriction;
    public float sensitivityX;
    public float sensitivityY;
    public static float CamRotationZ; // Current Z rotation for the camera
    private float targetCamRotationZ = 0; // Target Z rotation for the camera
    private float rotationSmoothTime = 0.1f; // Smoothing time
    private float rotationVelocity; // Velocity for SmoothDamp

    // true when F5 is pressed to unlock mouse, generally used to lock movement
    public static bool MouseLockedMode = false;

    void Start()
    {
        sensitivityX *= Screen.width / 2;
        sensitivityY *= Screen.height / 2;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) {
            Cursor.lockState = CursorLockMode.None;
            MouseLockedMode = true;
        }
        if (Input.GetKeyDown(KeyCode.F6)) {
            Cursor.lockState = CursorLockMode.Locked;
            MouseLockedMode = false;
        }

        if (MouseLockedMode) { return; }
        RotateCamToMousePos();
    }

    private void RotateCamToMousePos()
    {
        CamRotation.x += Input.GetAxis("Mouse X") * (sensitivityX * Time.deltaTime);
        CamRotation.y += Input.GetAxis("Mouse Y") * (sensitivityY * Time.deltaTime);

        CamRotation.y = Mathf.Clamp(CamRotation.y, -CameraYRestriction, CameraYRestriction);

        // Smoothly interpolate the Z rotation
        CamRotationZ = Mathf.SmoothDamp(CamRotationZ, targetCamRotationZ, ref rotationVelocity, rotationSmoothTime);

        Camera.transform.localRotation = Quaternion.Euler(-CamRotation.y, 0, CamRotationZ);
        Player.transform.rotation = Quaternion.Euler(0, CamRotation.x, 0);
    }

    // Public method to set the target Z rotation
    public void SetTargetCamRotationZ(float targetRotationZ)
    {
        targetCamRotationZ = targetRotationZ;
    }
}
