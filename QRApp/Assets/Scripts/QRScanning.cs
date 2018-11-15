using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRScanning : MonoBehaviour
{

    // Use this for initialization
    private WebCamTexture camTexture;
    private Rect screenRect;
    private int screenHeight, screenWidth, halfScreenHeight, halfScreenWidth;
    private float timer;

    public Text resultText;

    float rotAngle = 90;
    Vector2 pivotPoint;

    void Start() {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        halfScreenHeight = Screen.height / 2;
        halfScreenWidth = Screen.width / 2;

        screenRect = new Rect(0, 0, screenWidth, screenHeight);

        camTexture = new WebCamTexture();
        camTexture.requestedHeight = screenHeight;
        camTexture.requestedWidth = screenWidth;

        if (camTexture != null) {
            camTexture.Play();
        }

        timer = Time.time;
    }

    void OnGUI() {
        pivotPoint = new Vector2(halfScreenWidth, halfScreenHeight);
        GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);

        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
        
        if (Time.time - timer > 1) {
            ReadCode();
            timer = Time.time;
        }
    }

    void ReadCode() {
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        try {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            resultText.text = "Looking";
            var result = barcodeReader.Decode(camTexture.GetPixels32(),
              camTexture.width, camTexture.height);
            if (result != null) {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                resultText.text = result.Text;
            }
        } catch (UnityException ex) { Debug.LogWarning(ex.Message); }
    }
}
