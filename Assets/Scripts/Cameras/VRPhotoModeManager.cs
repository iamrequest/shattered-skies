using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

/// <summary>
/// Super simple photo mode. Includes time stop and screenshots.
/// Notes:
///   - This does NOT stop the player from moving or performing inputs while active.
///   - Screenshots are taken from the desktop view, which may be different from what the player sees.
///       It's recommended to take screenshots from the DesktopCamera prefab instead.
/// </summary>
public class VRPhotoModeManager : MonoBehaviour {
    public static VRPhotoModeManager Instance { get; private set; }
    private Camera photoModeCam;

    public Camera ActiveVRCam {
        get {
            if (IsPhotoModeOn) return photoModeCam;
            else return Player.Instance.cam;
        }
    }
    public bool IsPhotoModeOn { get; private set; }
    public bool IsTimeStopped { get; private set; }
    public bool IsSprinting { get; private set; }

    public TrackedPoseDriver leftHand, rightHand;
    public SteamVR_Action_Vector2 moveAction;
    public SteamVR_Action_Vector2 turnAction;
    public SteamVR_Action_Boolean sprintAction;
    public SteamVR_Action_Boolean togglePhotoModeAction, timeStopAction, screenShotAction;

    [Header("Motion")]
    [Range(0f, 25f)]
    public float defaultSpeed;
    [Range(0f, 25f)]
    public float sprintSpeed;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }
        photoModeCam = GetComponentInChildren<Camera>();
    }

    private void OnEnable() {
        moveAction.AddOnAxisListener(DoMove, SteamVR_Input_Sources.Any);
        sprintAction.AddOnChangeListener(SetSprint, SteamVR_Input_Sources.Any);
        turnAction.AddOnAxisListener(DoTurn, SteamVR_Input_Sources.Any);
        togglePhotoModeAction.AddOnStateDownListener(TogglePhotoMode, SteamVR_Input_Sources.Any);
        timeStopAction.AddOnStateDownListener(ToggleTimeStop, SteamVR_Input_Sources.Any);

        if(screenShotAction != null) screenShotAction.AddOnStateDownListener(TakeScreenshot, SteamVR_Input_Sources.Any);
    }
    private void OnDisable() {
        moveAction.RemoveOnAxisListener(DoMove, SteamVR_Input_Sources.Any);
        sprintAction.RemoveOnChangeListener(SetSprint, SteamVR_Input_Sources.Any);
        turnAction.RemoveOnAxisListener(DoTurn, SteamVR_Input_Sources.Any);
        togglePhotoModeAction.RemoveOnStateDownListener(TogglePhotoMode, SteamVR_Input_Sources.Any);
        timeStopAction.RemoveOnStateDownListener(ToggleTimeStop, SteamVR_Input_Sources.Any);

        if(screenShotAction != null) screenShotAction.RemoveOnStateDownListener(TakeScreenshot, SteamVR_Input_Sources.Any);
    }

    private void Update() {
        if (!IsPhotoModeOn) {
            AlignToPlayer();
        }
    }

    private void AlignToPlayer() {
        Vector3 hmdOffset = photoModeCam.transform.position - transform.position;
        hmdOffset.y -= Player.Instance.cam.transform.localPosition.y;

        transform.position = Player.Instance.playerController.transform.position - hmdOffset;
        transform.rotation = Player.Instance.playerController.transform.rotation;
    }


    private void DoMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) {
        if (!IsPhotoModeOn) return;
        float moveSpeed = IsSprinting ? sprintSpeed : defaultSpeed;

        transform.position += photoModeCam.transform.forward * moveSpeed * axis.y * Time.unscaledDeltaTime;
        transform.position += photoModeCam.transform.right * moveSpeed * axis.x * Time.unscaledDeltaTime;
    }
    private void SetSprint(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        IsSprinting = newState;
    }


    private void DoTurn(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) {
        if (!IsPhotoModeOn) return;
        
        transform.RotateAround(photoModeCam.transform.position, 
            Vector3.up, 
            Player.Instance.playerController.SmoothTurnSpeed * axis.x * Time.unscaledDeltaTime);
    }


    private void TakeScreenshot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        DateTime dt = DateTime.Now;
        String filename = $"{dt.Year}-{dt.Month}-{dt.Day}_{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond}.png";

        Debug.Log($"Saving screenshot as ./{filename}");
        ScreenCapture.CaptureScreenshot(filename, 1);
    }


    private void SetTimeStop(bool isTimeStopped) {
        // Optional: Uncomment this. I'm allowing timestop in play mode for better footage recording
        //if (!IsPhotoModeOn) return;
        IsTimeStopped = isTimeStopped;

        if (IsTimeStopped) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    private void ToggleTimeStop(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        SetTimeStop(!IsTimeStopped);
    }


    private void TogglePhotoMode(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        IsPhotoModeOn = !IsPhotoModeOn;

        if (IsPhotoModeOn) {
            photoModeCam.enabled = true;
            Player.Instance.cam.enabled = false;
        } else {
            photoModeCam.enabled = false;
            Player.Instance.cam.enabled = true;

            SetTimeStop(false);
        }
    }
}
