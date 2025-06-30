using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PublicConstants : MonoBehaviour {
    public static PublicConstants Singleton;

    //PLAYER references
    public GameObject R_HandPointA;
    public GameObject R_Player;
    public GameObject R_PCam;
    public static Ray PlayerCrosshairDir;
    public static MovementHandler Pmh;
    public static Rigidbody Prb;
    public static Grappling Pgrp;
    public static Vector3 PSpawn;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        PSpawn = R_Player.transform.position;
        Pmh = R_Player.GetComponent<MovementHandler>();
        Prb = R_Player.GetComponent<Rigidbody>();
        Pgrp = R_Player.GetComponent<Grappling>();
    }
    private void Update()
    {
        PlayerCrosshairDir = Singleton.R_PCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }
}
