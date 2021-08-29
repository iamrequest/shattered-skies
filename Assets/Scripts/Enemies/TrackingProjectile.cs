using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : EnemyProjectile {
    [Range(0f, 1f)]
    public float rotationSpeed;
    [Range(0f, 15f)]
    public float speed;

    private void FixedUpdate() {
        Vector3 thisToPlayer = (Player.Instance.cam.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(thisToPlayer);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        rb.MovePosition(rb.position + rb.transform.forward * speed * Time.deltaTime);
    }
}
