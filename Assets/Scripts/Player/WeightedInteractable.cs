using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;

/// <summary>
/// This approach to stamina is a bit of a hack, since HVR joint strength is stored in a scriptable object (not a var).
///     Not sure if I can modify that in builds.
/// Tween the mass of this rigidbody depending on how much stamina the player has.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class WeightedInteractable : MonoBehaviour {
    private Rigidbody rb;

    [Range(0f, 30f)]
    public float lowStaminaWeight, highStaminaWeight;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        SetWeight();
    }

    public void SetWeight() {
        rb.mass = Mathfs.Remap(0f, 1f, lowStaminaWeight, highStaminaWeight, StaminaManager.Instance.GetStaminaFromCurve());
    }
}
