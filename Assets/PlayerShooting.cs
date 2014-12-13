using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {
    public float FireRate = 0.5f;
    public float Damage = 25f;

    // Update is called once per frame
    void Update() {
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
                health.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, Damage);
            }
        }

        cooldown = FireRate;
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
}
