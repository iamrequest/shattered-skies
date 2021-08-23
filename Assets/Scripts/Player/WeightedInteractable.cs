using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;
using HurricaneVR.Framework.Core;

/// <summary>
/// This approach to stamina is a bit of a hack, since HVR joint strength is stored in a scriptable object (not a var).
///     Not sure if I can modify that in builds.
///     Update: Turns out this is actually a really good solution - the player can't just lift the sword via the physics hand when they have low stamina
/// Tween the mass of this rigidbody depending on how much stamina the player has.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HVRGrabbable))]
public class WeightedInteractable : MonoBehaviour {
    private Rigidbody rb;
    private HVRGrabbable grabbable;

    [Range(0f, 30f)]
    public float lowStaminaWeight, highStaminaWeight;

    [Header("Stamina Consumption")]
    [Tooltip("If true, this interactable will decrease the player's stamina when held and moved around")]
    public bool consumesStaminaWhenMoved;
    public Transform staminaMeasurementTransform;
    public float staminaConsumptionMultiplier;
    private Vector3 previousMeasurementPosition;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<HVRGrabbable>();

        previousMeasurementPosition = staminaMeasurementTransform.position;
    }

    void FixedUpdate() {
        if (consumesStaminaWhenMoved && grabbable.IsBeingHeld) {
            ConsumeStamina();
        }

        SetWeight();
    }

    /// <summary>
    /// Set the weight of this interactable (ie: the mass of the rigidbody) relative to how much stamina the player has
    /// </summary>
    private void SetWeight() {
        rb.mass = Mathfs.Remap(0f, 1f, lowStaminaWeight, highStaminaWeight, StaminaManager.Instance.GetStaminaFromCurve());
    }

    /// <summary>
    /// Consume stamina relative to how much the measurement transform has moved this physics frame.
    /// </summary>
    private void ConsumeStamina() {
        Vector3 positionDelta =  staminaMeasurementTransform.position - previousMeasurementPosition;

        // Ignore movement from the player's body
        positionDelta -= StaminaManager.Instance.positionDelta;

        StaminaManager.Instance.ConsumeStamina(positionDelta.magnitude * staminaConsumptionMultiplier);

        previousMeasurementPosition = staminaMeasurementTransform.position;
    }
}
