using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float HitPoints = 10f;

    public void TakeDamage(float amount) {
        currentHitPoints -= amount;

        if (currentHitPoints <= 0) {
            Die();
        }
    }

    private void Start() {
        currentHitPoints = HitPoints;
    }

    private void Die() {
        Destroy(gameObject);
    }

    private float currentHitPoints;
}
