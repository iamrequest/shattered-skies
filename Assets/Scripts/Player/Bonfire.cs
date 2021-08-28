using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bonfire : Checkpoint {
    private VisualEffect vfx;

    public bool isKindled, isLit;
    public AudioClip onBonfireLitSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;

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
            float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
            SFXPlayer.Instance.PlaySFX(onBonfireLitSFX, transform.position, sfxPitch, VolumeManager.Instance.bonfireLit);

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
