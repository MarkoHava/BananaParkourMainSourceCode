using System;
using System.Drawing;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.XR.Oculus.Input;
using UnityEngine;

public class Grappling : MonoBehaviour {
    private Vector3 grapplePoint;
    private SpringJoint sj;
    public static bool IsGrappling;
    public LayerMask WhatIsGrappable;
    [SerializeField] private float springJointDamper;
    [SerializeField] private float springJointMinDist;
    [SerializeField] private float springJointMaxDist;
    [SerializeField] private float springJointSpring;
    [SerializeField] private float springJointMassScale;
    [SerializeField] private float MaxGrappleDistance;
    [SerializeField] private float grappleDelay;
    [SerializeField] private Transform grappleTip;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private ParticleSystem _onFiredEffect;
    [SerializeField] private float StopGrappleJump;
    [SerializeField] private GameObject PointPrefab;
    [SerializeField] private AudioClip HookAudioClip;

    private MovementHandler movementHandler;
    [SerializeField] private CinemachineCamera cam;
    public static float initialFov;
    public static float targetFov;
    private float inbetweenGrappleTimer = 0f;

    private void Start()
    {
        initialFov = cam.Lens.FieldOfView;
        targetFov = initialFov;
    }
    private void LateUpdate()
    {
        if (IsGrappling) lr.SetPosition(0, grappleTip.position);
        movementHandler = GetComponent<MovementHandler>();
        cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFov, Time.deltaTime * 0.5f);
    }

    private void Update()
    {
        GuideGrappableSurfaces();
        if (!IsGrappling) inbetweenGrappleTimer += Time.deltaTime;
        else inbetweenGrappleTimer = 0f;
        if (Input.GetKeyDown(KeyCode.Mouse0) && inbetweenGrappleTimer > 0.6f) StartGrapple();
        if (Input.GetKeyUp(KeyCode.Mouse0)) StopGrapple();
        if (movementHandler.IsGrounded() && !IsGrappling) movementHandler.MovementMultiplier = 1f;
    }

    private GameObject GrappableGuidePoint;

    private void StartGrapple()
    {
        GetComponent<Rigidbody>().mass = 1f;
        var camRay = PublicConstants.PlayerCrosshairDir;
        if (Physics.Raycast(camRay.origin, camRay.direction, out RaycastHit hit, MaxGrappleDistance, WhatIsGrappable)) {
            GetComponent<AudioSource>().PlayOneShot(HookAudioClip);
            IsGrappling = true;
            grapplePoint = hit.point;
            initialFov = cam.Lens.FieldOfView;
            GrappableGuidePoint = Instantiate(PointPrefab, grapplePoint, Quaternion.identity);
            PublicConstants.Prb.AddForce(camRay.direction * 5, ForceMode.Acceleration);
            ExecuteGrapple();
        }
        else {
            StopGrapple();
        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }
    private void ExecuteGrapple()
    {
        targetFov = cam.Lens.FieldOfView * 1.5f;
        float distFromPoint = Vector3.Distance(transform.position, grapplePoint);
        sj = PublicConstants.Singleton.R_Player.AddComponent<SpringJoint>();
        sj.autoConfigureConnectedAnchor = false;
        sj.connectedAnchor = grapplePoint;
        sj.maxDistance = distFromPoint * springJointMaxDist;
        sj.minDistance = distFromPoint * springJointMinDist;
        sj.spring = springJointSpring;
        sj.damper = springJointDamper;
        sj.massScale = springJointMassScale;
        lr.positionCount = 2;
    }
    public void StopGrapple()
    {
        if (IsGrappling) {
            if (!(movementHandler.MovementMultiplier >= 4f)) movementHandler.MovementMultiplier *= 1.3f;
            //GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.forward) * StopGrappleJump * movementHandler.MovementMultiplier, ForceMode.Impulse);
        }
        targetFov = initialFov;
        lr.positionCount = 0;
        IsGrappling = false;
        lr.enabled = false;
        GetComponent<Rigidbody>().mass = 1f;
        Destroy(GrappableGuidePoint, grappleDelay);
        Destroy(sj);
    }
    GameObject guidePoint = null;
    private void GuideGrappableSurfaces()
    {
        if (IsGrappling) return;
        var camRay = PublicConstants.PlayerCrosshairDir;
        if (Physics.Raycast(camRay.origin, camRay.direction, out RaycastHit hit, MaxGrappleDistance, WhatIsGrappable)) {

            if (!Physics.CheckSphere(hit.point, 100f, LayerMask.NameToLayer("Guide")) || guidePoint == null) {
                guidePoint = Instantiate(PointPrefab, hit.point, Quaternion.identity);
            }
            else {
                Destroy(guidePoint);
                guidePoint = null;
            }
        }
    }
}
