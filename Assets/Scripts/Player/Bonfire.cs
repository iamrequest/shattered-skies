using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bonfire : Checkpoint {
    private VisualEffect vfx;

    public bool isKindled, isLit;

    private void Awake() {
        vfx = GetComponent<VisualEffect>();

        if (isKindled) {
            vfx.SendEvent("OnKindle");
        }
        if (isLit) {
            vfx.SendEvent("OnLit");
        }
    }

    public void KindleBonfire() {
        if (!isKindled) {
            isKindled = true;
            vfx.SendEvent("OnKindle");
        }
    }
    public void LightBonfire() {
        if (!isLit) {
            isLit = true;
            vfx.SendEvent("OnLit");
        }

        Player.Instance.activeCheckpoint = this;
    }
    public void Stop() {
        isKindled = false;
        isLit = false;
        vfx.Stop();
    }
}
