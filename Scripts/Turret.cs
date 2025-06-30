using UnityEngine;

public class Turret : MonoBehaviour {
    [SerializeField] private ParticleSystem PS_projectile;
    [SerializeField] private float radius = 50f;

    void Update()
    {
        if (Physics.Raycast(transform.position, PublicConstants.Singleton.R_Player.transform.position, out RaycastHit hit, radius)) {
            Fire(hit);
        }
    }
    private void Fire(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer == 3) {
            transform.LookAt(hit.collider.gameObject.transform);
            PS_projectile.Play();
            Health.Instance.TakeDamage(1);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
