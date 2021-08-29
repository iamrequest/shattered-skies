using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCamera : MonoBehaviour {
    public Camera cam;
    public RenderTexture renderTexture;
    public bool isTimePaused = false;
    public bool isTrackingMouse = false;


    [Range(0f, 5f)]
    public float turnSpeed;
    [Range(0f, 20f)]
    public float defaultSpeed, fastSpeed;
    [Range(0f, 5f)]
    public float slowSpeed;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isTrackingMouse = true;
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            ToggleTimeStop();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            isTrackingMouse = !isTrackingMouse;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Screenshot());
        }


        if (isTrackingMouse) {
            Rotate();
            Move();
        }
    }


    private void ToggleTimeStop() {
        isTimePaused = !isTimePaused;
        if (isTimePaused) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    private IEnumerator Screenshot() {
        yield return new WaitForEndOfFrame();
        cam.targetTexture = renderTexture;
        cam.Render();
        cam.targetTexture = null;


        DateTime dt = DateTime.Now;
        String filename = $"{dt.Year}-{dt.Month}-{dt.Day}_{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond}.png";
        Debug.Log($"Saving screenshot as ./{filename}");


        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        byte[] bytes;
        bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);



        // This is rendering to a black image for some reason
        //ScreenCapture.CaptureScreenshot(filename, 1);
    }

    private void Rotate() {
        float h = Input.GetAxis("Mouse X");
        float v = -Input.GetAxis("Mouse Y");
        //transform.Rotate(v, h, 0);

        Quaternion r1 = Quaternion.AngleAxis(h * turnSpeed, Vector3.up);
        Quaternion r2 = Quaternion.AngleAxis(v * turnSpeed, Vector3.right);
        transform.localRotation = transform.localRotation * r1 * r2;

        Vector3 euler = transform.rotation.eulerAngles;
        euler.z = 0f;
        transform.localRotation = Quaternion.Euler(euler);
    }

    private void Move() {
        // Delegate between sprint/
        float moveSpeed = defaultSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            moveSpeed = fastSpeed;
        } else if (Input.GetKey(KeyCode.LeftControl)) {
            moveSpeed = slowSpeed;
        }

        // I should really learn the new input system things at some point
        if (Input.GetKey(KeyCode.A)) {
            transform.position -= transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += transform.right * moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position -= transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.E)) {
            transform.position += transform.up * moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.Q)) {
            transform.position -= transform.up * moveSpeed * Time.unscaledDeltaTime;
        }
    }
}
