using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour {
    private const bool DEBUG = true;

    public Transform eyeTransform;
    public LayerMask visibleLayers;

    [Range(0f, 20f)]
    public float visionDistance;

    [Tooltip("The angular radius (degrees) that the enemy can see on each side")]
    [Range(0f, 90f)]
    public float visionRadius;

    public bool IsInSight(Vector3 target, string tag) {
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
}
