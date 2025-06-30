using Unity.Cinemachine;
using UnityEngine;

public class Health : MonoBehaviour {
    public int HealthPoints = 3;
    public static Health Instance;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            HealthPoints = 3;
            PublicConstants.Pmh.Velocity = Vector3.zero;
            transform.position = PublicConstants.PSpawn;
            Score.Multiplier = 1f;
            Score.timeInAir = 0f;
            Score.totalDist = 0f;
            Score.FinalLevelScore = 0f;
            Score.PlayTime = 0f;
            Score.FinalScore = 0f;
        }
    }
    public void TakeDamage(int damage = 1)
    {
        HealthPoints -= damage;
        if (HealthPoints <= 0) {
            Die();
        }
    }
    public void Die()
    {
        PublicConstants.Pmh.Velocity = Vector3.zero;
        transform.position = PublicConstants.PSpawn;
        Score.Multiplier = 1f;
        Score.FinalScore = 0f;
        PublicConstants.Singleton.R_PCam.GetComponent<CinemachineCamera>().Lens.FieldOfView = 80f;
        MovementHandler.initialFov = 80f;
        Grappling.initialFov = 80f;
        Grappling.targetFov = 80f;
    }
}
