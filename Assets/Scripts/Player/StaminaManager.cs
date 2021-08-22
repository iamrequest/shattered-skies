using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour {
    public float currentStamina;
    public float maxStamina;

    // Use this to non-linearize the stamina decrease
    public AnimationCurve staminaRemapCurve;

    public static StaminaManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }
    }

    public float GetNormalizedStamina() {
        return currentStamina / maxStamina;
    }
    public float GetStaminaFromCurve() {
        return Mathf.Clamp01(staminaRemapCurve.Evaluate(GetNormalizedStamina()));
    }
}
