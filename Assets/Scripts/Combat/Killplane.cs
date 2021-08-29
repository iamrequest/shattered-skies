using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Killplane : MonoBehaviour {
    private Collider m_collider;
    public float killplaneActionDelay;
    // Optional: follow player

    private void OnTriggerEnter(Collider other) {
        //KillplaneInteraction(other.gameObject);
        StartCoroutine(DoKillPlaneAction(other.gameObject));
    }

    private void OnCollisionEnter(Collision collision) {
        //KillplaneInteraction(collision.collider.gameObject);
        StartCoroutine(DoKillPlaneAction(collision.collider.gameObject));
    }

    private void KillplaneInteraction(GameObject other) {
        if (other.TryGetComponent(out Damageable damageable)) {
            damageable.OnKillPlaneEntered();
        } else {
            Damageable parentDamageable = other.GetComponentInParent<Damageable>();
            if (parentDamageable) {
                parentDamageable.OnKillPlaneEntered();
            }
        }
    }

    private IEnumerator DoKillPlaneAction(GameObject other) {
        yield return new WaitForSeconds(killplaneActionDelay);

        if (other != null) {
            KillplaneInteraction(other);
        }
    }
}
