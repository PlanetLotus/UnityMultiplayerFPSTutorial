using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {
    private void Start() {
        fxManager = GameObject.FindObjectOfType<FXManager>();
        weaponData = gameObject.GetComponentInChildren<WeaponData>();
    }

    // Update is called once per frame
    private void Update() {
        cooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1")) {
            Fire();
        }

    }

    private void Fire() {
        if (cooldown > 0)
            return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 hitPoint;
        Transform hitTransform = GetClosestHitObject(ray, out hitPoint);

        if (hitTransform != null) {
            Debug.Log("We hit: " + hitTransform.name);

            Health health = hitTransform.GetComponent<Health>();

            while (health == null && hitTransform.parent != null) {
                hitTransform = hitTransform.parent;
                health = hitTransform.GetComponent<Health>();
            }

            if (health != null) {
                TeamMember teamMember = hitTransform.GetComponent<TeamMember>();
                TeamMember myTeamMember = this.GetComponent<TeamMember>();

                if (teamMember == null || teamMember.TeamId == 0 || myTeamMember == null || myTeamMember.TeamId == 0 || teamMember.TeamId != myTeamMember.TeamId) {
                    health.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, weaponData.Damage);
                }
            }

            // TODO: Store this locally. GetComonent is expensive.
            DoGunFX(hitPoint);
        } else {
            // Didn't hit anything, but should still show FX
            hitPoint = Camera.main.transform.position + Camera.main.transform.forward * 100f;
            DoGunFX(hitPoint);
        }

        cooldown = weaponData.FireRate;
    }

    private void DoGunFX(Vector3 hitPoint) {
        fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, weaponData.transform.position, hitPoint);
    }

    private Transform GetClosestHitObject(Ray ray, out Vector3 hitPoint) {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0f;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits) {
            if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }

        return closestHit;
    }

    private float cooldown = 0f;
    private FXManager fxManager;
    private WeaponData weaponData;
}
