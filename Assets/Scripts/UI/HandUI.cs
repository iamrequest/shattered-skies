using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour {
    public List<Image> childImages;
    public AnimationCurve opacityCurve;
    private float currentViewingAngle;

    void LateUpdate() {
        currentViewingAngle = Vector3.Dot(transform.forward, Player.Instance.cam.transform.forward);

        foreach (Image image in childImages) {
            Color c = image.color;
            c.a = opacityCurve.Evaluate(currentViewingAngle);
            image.color = c;
        }
    }
}
