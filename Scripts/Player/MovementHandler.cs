using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovementHandler : MonoBehaviour {

    public static MovementHandler Singleton;
    public float MoveSpeed = 10f;
    public float SlowedSpeed = 4f;
    public float JumpForce = 5f;
    private Rigidbody rb;
    public Vector3 Velocity;
    private bool isSliding;
    public bool MovementSlowed = false;
    public float groundDrag = 12f;
    public float airDrag = 0.98f;
    public float MovementMultiplier = 1f;
    public Vector3 MoveDirVect;
    public Vector3 CamZVec3;
    public Vector3 CamXVec3;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float maxWallRunTime;
    [SerializeField] private float slideMomentumIncrease = 2f;
    [SerializeField] private GameObject Cam;
    [SerializeField] private GameObject Visuals;
    public bool DisableDeceleration = false;
    public Vector2 PlayerVisualsRotation = new();

    public bool isWallRunning;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    [SerializeField] private float minJumpHeight;
    [SerializeField] private float wallCheckDist;
    public bool UnlockPlayerRotation = false;
    private bool wallLeft;
    private bool wallRight;
    private float x = 0;
    private float z = 0;
    private bool unlockColliderHeight = false;
    [SerializeField] private List<AudioClip> SceneBGMusic;
    public static float initialFov = 80f;
    private bool wasGrounded = false;

    private void Start()
    {
        MovementMultiplier = 1f;
        initialFov = Cam.GetComponent<CinemachineCamera>().Lens.FieldOfView;
        rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true; // Prevent the Rigidbody from rotating
        AudioClip bgMusic = SceneBGMusic[0];
        GetComponent<AudioSource>().PlayOneShot(bgMusic);
    }

    private void Update()
    {
        HandleMovement();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        Slide();
        CheckForWall();

        Cam.GetComponent<CinemachineCamera>().Lens.FieldOfView = Mathf.Lerp(
            Cam.GetComponent<CinemachineCamera>().Lens.FieldOfView, initialFov * ((MovementMultiplier + 2) / 3), Time.deltaTime * 2);
        if (!UnlockPlayerRotation) PlayerVisualsRotation = Vector2.zero;
        if (!unlockColliderHeight) GetComponent<CapsuleCollider>().height = 1.95f;
        Visuals.transform.localRotation = Quaternion.Euler(PlayerVisualsRotation.x, 0f, PlayerVisualsRotation.y);
        rb.linearVelocity = new Vector3(Velocity.x, rb.linearVelocity.y, Velocity.z);
        if (!Grappling.IsGrappling && !IsGrounded(5f) && !isWallRunning) {
            MovementMultiplier = Mathf.Lerp(MovementMultiplier, 1f, Time.deltaTime * 0.1f);
        }

        wasGrounded = IsGrounded();
    }
    void LateUpdate()
    {

        if ((wallLeft || wallRight) && !IsGrounded(minJumpHeight)) {
            if (!isWallRunning) StartWallRun();

            // wall jump
            if (Input.GetKeyDown(KeyCode.Space)) WallJump();
        }
        if (!(wallRight || wallLeft) && isWallRunning) StopWallRun();
        if (exitingWall) {
            if (isWallRunning) StopWallRun();
        }

    }
    private void FixedUpdate()
    {

        if (transform.position.y <= -100 || Input.GetKey(KeyCode.Backspace)) {
            Health.Instance.Die();
        }
        if (isWallRunning && (wallLeft || wallRight)) WallRunMovement();
    }

    public bool IsGrounded(float dist = 2f)
    {
        return Physics.Raycast(transform.position, Vector3.down, dist);
    }

    private float _z;
    private float _x;
    private void HandleMovement()
    {
        if (isSliding) return;
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");


        CamZVec3 = new(Cam.transform.forward.x, 0, Cam.transform.forward.z);
        CamXVec3 = new(Cam.transform.right.x, 0, Cam.transform.right.z);

        var FinalMoveSpeed = MoveSpeed;
        if (MovementSlowed) FinalMoveSpeed = SlowedSpeed;

        MoveDirVect = new Vector3(x, 0, z).normalized;


        var vel = (FinalMoveSpeed * MovementMultiplier * ((z * CamZVec3).normalized + (x * CamXVec3)).normalized);


        if (x != 0 || z != 0) {
            Velocity.x = vel.x;
            Velocity.z = vel.z;
        }
        else {
            Decelerate(true);
            Decelerate(false);
        }
    }


    private void Jump()
    {
        //if (isSliding) return;
        if (!IsGrounded()) return;
        float horizontalJumpForce = 0;
        Vector3 jumpDir = new Vector3(horizontalJumpForce, JumpForce, 0);
        if (isWallRunning) {
            jumpDir += (wallLeft ? leftWallHit.normal : rightWallHit.normal) * JumpForce * 10f;
            jumpDir += transform.forward * JumpForce * 0.5f;
            Velocity += (wallLeft ? rightWallHit.normal : leftWallHit.normal) * JumpForce * 10f;
            StopWallRun();
        }

        Score.FinalScore += 10 * (MovementMultiplier * 0.5f);
        rb.AddForce(jumpDir, ForceMode.VelocityChange); //add vertical jump force
    }
    private Vector3 slideTempfwd;
    private Vector3 slideTempCamX;
    private Vector3 slideTempCamZ;
    private bool slideCanLockRotation = false;
    private void Slide()
    {
        if (Grappling.IsGrappling) return;
        var slideSpeed = MoveSpeed * slideMomentumIncrease;
        //Vector3 slideDirection = (z * slideTempCamZ + x * slideTempCamX).normalized;

        if (!isSliding) {
            slideTempCamX = CamXVec3;
            slideTempCamZ = CamZVec3;
            slideTempfwd = transform.forward;
        }
        if (!IsGrounded(5f)) { isSliding = false; return; }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Score.FinalScore += 10 * (MovementMultiplier * 0.5f) * Time.deltaTime;
            isSliding = true;
            Velocity = slideTempfwd * slideSpeed;
            slideCanLockRotation = true;
            UnlockPlayerRotation = true;
            PlayerVisualsRotation.x = -30;
            unlockColliderHeight = true;
            GetComponent<CapsuleCollider>().height = 0.5f;
        }
        if (isSliding) {
            Vector3 slideDirection = (z * slideTempCamZ + x * slideTempCamX).normalized;
            if (x != 0) {
                Velocity = slideDirection * slideSpeed;
            }
        }

        if (!Input.GetKey(KeyCode.LeftControl)) {
            isSliding = false;
            if (slideCanLockRotation) { UnlockPlayerRotation = false; slideCanLockRotation = false; unlockColliderHeight = false; }
        }
    }

    private void Decelerate(bool is_x)
    {
        if (DisableDeceleration) return;
        if (!IsGrounded()) {
            MovementSlowed = true;
            if (is_x) Velocity.x = Mathf.Lerp(Velocity.x, 0, 1 - Mathf.Exp(-airDrag * Time.deltaTime));
            else Velocity.z = Mathf.Lerp(Velocity.z, 0, 1 - Mathf.Exp(-airDrag * Time.deltaTime));
        }

        else {
            MovementSlowed = false;
            if (is_x) Velocity.x = Mathf.Lerp(Velocity.x, 0, 1 - Mathf.Exp(-groundDrag * Time.deltaTime));
            else Velocity.z = Mathf.Lerp(Velocity.z, 0, 1 - Mathf.Exp(-groundDrag * Time.deltaTime));
        }
    }
    private void CheckForWall()
    {
        if (exitingWall) return;
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit a, wallCheckDist, LayerMask.NameToLayer("Default"))) {
            x = 0;
            isWallRunning = false; // Stop wall running if there's a wall on the left
        }
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit b, wallCheckDist, LayerMask.NameToLayer("Default"))) {
            x = 0;
            isWallRunning = false; // Stop wall running if there's a wall on the right
        }
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDist, whatIsWall);
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDist, whatIsWall);
    }

    Vector3 wallNormal;
    Vector3 wallForward;
    private void StartWallRun()
    {
        if (Grappling.IsGrappling) return;
        isWallRunning = true;
        UnlockPlayerRotation = true;
        MouseHandler.Singleton.SetTargetCamRotationZ(wallLeft ? -7 : 7);
    }

    private void WallRunMovement()
    {
        if ((!wallLeft && !wallRight) && isWallRunning) {
            StopWallRun();
            return;
        }
        DisableDeceleration = true;
        //MovementMultiplier = 4f;
        Score.FinalScore += 30 * (MovementMultiplier * 0.2f) * Time.deltaTime;
        rb.useGravity = true;
        rb.mass = 80f;

        wallNormal = wallLeft ? leftWallHit.normal : rightWallHit.normal;
        var wallUp = wallLeft ? leftWallHit.transform.up : rightWallHit.transform.up;
        wallForward = Vector3.Cross(wallNormal, wallUp);

        if ((transform.forward - wallForward).magnitude * 2 > (transform.forward - -wallForward).magnitude) wallForward = -wallForward;

        wallForward.y = 0;
        wallForward.Normalize();

        //Cam.transform.localPosition = new(0, 0.524999976f, 2.5f * wallRunCamMove);

        Velocity = wallForward * Time.deltaTime;
        //rb.AddForce(-transform.up * 1000, ForceMode.Acceleration);
        if (!exitingWall) {
            if (z != 0) {
                rb.AddForce(-wallNormal * 100, ForceMode.Force);
            }
        }
    }
    private void StopWallRun()
    {
        //Cam.transform.localPosition = new(0, 0.524999976f, 0.50999999f);
        UnlockPlayerRotation = false;
        DisableDeceleration = false;
        //MovementMultiplier = MovementMultiplier >= 2f ? MovementMultiplier / 2 : MovementMultiplier;
        isWallRunning = false;
        rb.useGravity = true;
        rb.mass = 80f;

        // Reset the target camera rotation
        MouseHandler.Singleton.SetTargetCamRotationZ(0);
    }

    private bool exitingWall;
    private float exitWallTimer;
    private float exitWallTime;
    private void WallJump()
    {
        if (IsGrounded()) return;
        Score.FinalScore += 100 * (MovementMultiplier);
        isWallRunning = false;
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = ((Velocity + wallNormal + (transform.up * 0.3f)) / 2).normalized;

        // reset y velocity and add force
        //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply * 200f, ForceMode.VelocityChange);
        //Velocity = forceToApply;

        // Add a short delay before wall detection
        StartCoroutine(DelayWallDetection());
    }

    private IEnumerator DelayWallDetection()
    {
        yield return new WaitForSeconds(0.7f); // Adjust the delay as needed
        exitingWall = false;
    }
}
