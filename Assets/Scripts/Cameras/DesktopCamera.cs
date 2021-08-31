using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Camera))]
public class DesktopCamera : MonoBehaviour {
    private Camera screenshotCam;

    public bool isTimePaused = false;
    [Range(0f, 1f)]
    public float positionLerpSpeed, rotationLerpSpeed;

    public SteamVR_Action_Boolean timestopAction;
    public SteamVR_Action_Boolean screenshotAction;

    private void Awake() {
        screenshotCam = GetComponent<Camera>();
        screenshotCam.depth = 50;
    }

    private void OnEnable() {
        if(timestopAction != null) timestopAction.AddOnStateDownListener(ToggleTimeStop, SteamVR_Input_Sources.Any);
        screenshotAction.AddOnStateDownListener(Screenshot, SteamVR_Input_Sources.Any);
    }


    private void OnDisable() {
        if(timestopAction != null) timestopAction.RemoveOnStateDownListener(ToggleTimeStop, SteamVR_Input_Sources.Any);
        screenshotAction.RemoveOnStateDownListener(Screenshot, SteamVR_Input_Sources.Any);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            ToggleTimeStop();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Screenshot();
        }
    }

    private void FixedUpdate() {
        // If we have an active photo mode manager, get the active cam from that. Otherwise, just get the reference from the player prefab.
        Camera activeVRCam;
        if (VRPhotoModeManager.Instance != null) {
            activeVRCam = VRPhotoModeManager.Instance.ActiveVRCam;
        } else {
            activeVRCam = Player.Instance.cam;
        }

        screenshotCam.transform.position = Vector3.Lerp(screenshotCam.transform.position, activeVRCam.transform.position, positionLerpSpeed);
        screenshotCam.transform.rotation = Quaternion.Slerp(screenshotCam.transform.rotation, activeVRCam.transform.rotation, rotationLerpSpeed);
    }


    private void Screenshot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        Screenshot();
    }

    private void ToggleTimeStop(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        ToggleTimeStop();
    }




    private void ToggleTimeStop() {
        isTimePaused = !isTimePaused;
        if (isTimePaused) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    private void Screenshot() {
        // For some reason, this works here, but not in ScreenshotCamera. Weird URP/SteamVR/OpenVR thing?
        DateTime dt = DateTime.Now;
        String filename = $"{dt.Year}-{dt.Month}-{dt.Day}_{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond}.png";
        Debug.Log($"Saving screenshot as ./{filename}");
        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}
