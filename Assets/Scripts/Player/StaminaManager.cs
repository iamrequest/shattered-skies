﻿using HurricaneVR.Framework.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : MonoBehaviour {
    private HVRPlayerController playerController;

    public float currentStamina;
    public float maxStamina;

    [Tooltip("Regen per second")]
    [Range(0f, 20f)]
    public float staminaRegenRate;

    // Consider overriding this rate if the player is holding a weighted interactable
    [Tooltip("Stamina consumption per second of sprinting")]
    [Range(0f, 20f)]
    public float sprintStaminaConsumptionRate;

    // Use this to non-linearize the stamina decrease
    public AnimationCurve staminaRemapCurve;

    private Vector3 previousPosition;
    [HideInInspector]
    public Vector3 positionDelta;

    public static StaminaManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }

        playerController = GetComponentInChildren<HVRPlayerController>();

        previousPosition = playerController.transform.position;
    }

    private void Update() {
        // Regen stamina
        // Consume stamina from sprinting
        if (playerController.Sprinting) {
            ConsumeStamina(sprintStaminaConsumptionRate * Time.deltaTime);
        } else {
            currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0f, maxStamina);
        }
    }

    private void FixedUpdate() {
        positionDelta = playerController.transform.position - previousPosition;
        previousPosition = playerController.transform.position;
    }

    public float GetNormalizedStamina() {
        return currentStamina / maxStamina;
    }
    public float GetStaminaFromCurve() {
        return Mathf.Clamp01(staminaRemapCurve.Evaluate(GetNormalizedStamina()));
    }

    public void ConsumeStamina(float staminaReduction) {
        currentStamina = Mathf.Clamp(currentStamina - staminaReduction, 0f, maxStamina);
    }
}
