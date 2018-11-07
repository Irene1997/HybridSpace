using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class Test : MonoBehaviour
{

    // Use this for initialization
    private WebCamTexture camTexture;
    private Rect screenRect;

    public Text resultText;

    void Start() {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height/2;
        camTexture.requestedWidth = Screen.width/2;
        if (camTexture != null) {
            camTexture.Play();
        }
    }

    void OnGUI() {
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleAndCrop);

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
