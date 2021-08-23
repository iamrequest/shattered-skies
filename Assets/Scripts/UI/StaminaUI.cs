using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StaminaUI : MonoBehaviour {
    private Slider slider;
    public Image fillImage;
    public Gradient colorGradient;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    private void Update() {
        slider.value = StaminaManager.Instance.GetNormalizedStamina();
        fillImage.color = colorGradient.Evaluate(slider.value);
    }
}
