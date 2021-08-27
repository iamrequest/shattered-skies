using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;

public class EnemyVision : MonoBehaviour {
    private const bool DEBUG = true;

    public Transform eyeTransform;
    public LayerMask visibleLayers;

    [Range(0f, 20f)]
    public float visionDistance;

    [Tooltip("The angular radius (degrees) that the enemy can see on each side")]
    [Range(0f, 90f)]
    public float visionRadius;

    [Tooltip("The distance radius that the enemy can see on each side, regardless of what is in the way")]
    [Range(0f, 5f)]
    public float proximityVisionRadius;

    public bool IsInSight(Vector3 target, string tag) {
        if (DistanceToTarget(target) < proximityVisionRadius) return true;

        RaycastHit hit;

        // TODO: Convert to capsule cast + raycast. 
        // Check if we can see SOMETHING
        if (Physics.Raycast(eyeTransform.position, target - eyeTransform.position, out hit, visionDistance, visibleLayers)) {
            if (!hit.collider.CompareTag(tag)) {
                // Something is in the way of the target
                DrawDebugRay(hit.point, Color.yellow);
                return false;
            }

            if (!IsWithinViewingAngle(hit.point)) {
                // Outside vision radius
                DrawDebugRay(target, Color.magenta);
                return false;
            }

            DrawDebugRay(target, Color.green);
            return true;
        }

        // Too far away, no collisions in the way
        DrawDebugRay(target, Color.red);
        return false;
    }

    public bool isPlayerInSight() {
        // Necessary because there's no collider on the head, just on the body
        //Vector3 raycastOffset = new Vector3(0f, -.1f, 0f);
        //return IsInSight(Player.Instance.cam.transform.position + raycastOffset, Player.Instance.gameObject.tag);

        return IsInSight(Player.Instance.cam.transform.position, Player.Instance.gameObject.tag);
    }
    
    private bool IsWithinViewingAngle(Vector3 target) {
        float angleToPlayer = Vector3.Angle(target - eyeTransform.position, eyeTransform.forward);
        return visionRadius > angleToPlayer;
    }

    private  float DistanceToTarget(Vector3 target) {
        return (eyeTransform.position - target).magnitude;
    }

    private void DrawDebugRay(Vector3 target, Color color) {
        float drawDistance = Mathf.Min(visionDistance, (target - eyeTransform.position).magnitude);
        if(DEBUG)
            Debug.DrawRay(eyeTransform.position, (target - eyeTransform.position).normalized * drawDistance, color);
    }



    public void OnDrawGizmosSelected() {
        Debug.DrawRay(eyeTransform.position, eyeTransform.forward * visionDistance, Color.green);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyeTransform.position, proximityVisionRadius);

        // I think this is wrong, since it produces weird angles depending on which dir the enemy is facing.
        // But it's good enough for visualization
        // Just realized, this is a world space rotation offset, not local space. 
        Vector3 v = Quaternion.Euler(visionRadius, 0f, 0f) * eyeTransform.forward;
        Debug.DrawRay(eyeTransform.position, v * visionDistance, Color.magenta);
        v = Quaternion.Euler(-visionRadius, 0f, 0f) * eyeTransform.forward;
        Debug.DrawRay(eyeTransform.position, v * visionDistance, Color.magenta);

        v = Quaternion.Euler(0f, visionRadius, 0f) * eyeTransform.forward;
        Debug.DrawRay(eyeTransform.position, v * visionDistance, Color.magenta);
        v = Quaternion.Euler(0f, -visionRadius, 0f) * eyeTransform.forward;
        Debug.DrawRay(eyeTransform.position, v * visionDistance, Color.magenta);
    }
}
