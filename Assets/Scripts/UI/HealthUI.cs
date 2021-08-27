using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthUI : MonoBehaviour {
    private Slider slider;
    public Image fillImage;
    public Gradient colorGradient;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    private void Start() {
        UpdateUI();
    }

    /*
    Player damageable is null on start() for some reason. I think it's just execution order stuff
    private void OnEnable() {
        Player.Instance.damageable.onDamageApplied.AddListener(OnPlayerHealthChanged);
        Player.Instance.damageable.onHealthDepleted.AddListener(OnPlayerHealthChanged);
        Player.Instance.damageable.onHealed.AddListener(OnPlayerHealthChanged);
    }
    private void OnDisable() {
        Player.Instance.damageable.onDamageApplied.RemoveListener(OnPlayerHealthChanged);
        Player.Instance.damageable.onHealthDepleted.RemoveListener(OnPlayerHealthChanged);
        Player.Instance.damageable.onHealed.RemoveListener(OnPlayerHealthChanged);
    }


    private void OnPlayerHealthChanged(BaseDamager arg0, Damageable arg1) {
        UpdateUI();
    }
    private void OnPlayerHealthChanged(float arg0, BaseDamager arg1, Damageable arg2) {
        UpdateUI();
    }
    */
    private void Update() {
        UpdateUI();
    }
    private void UpdateUI() {
        slider.value = Player.Instance.damageable.healthCurrent / Player.Instance.damageable.healthMax;
        fillImage.color = colorGradient.Evaluate(slider.value);
    }
}
