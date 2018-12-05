﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

public class ArduinoHandler : MonoBehaviour {
    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM3";
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;

    private SerialPort stream;

    public void Start() {
        Open();
        StartCoroutine(AsynchronousReadFromArduino((string s) => ReadMessage(s), () => Debug.LogError("Error!"), 10000f));
    }

    public void Open() {
        // Opens the serial port
        stream = new SerialPort(port, baudrate);
        stream.ReadTimeout = 50;
        stream.Open();
        WriteToArduino("ASK_STATE");
        //this.stream.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

        //WriteToArduino("PING");
        //WriteToArduino("ECHO echo");
        //WriteToArduino("LED_OFF");
        //WriteToArduino("ERRORTEST");
    }

    void ReadMessage(string message) {
        Debug.Log("Received '" + message + "' from the Arduino.");
        switch (message) {
            case "Open":
                GameController.Instance.doors[0].Open();
                break;
            case "Close":
                GameController.Instance.doors[0].Close();
                break;
            default:
                break;
        }
    }

    public void WriteToArduino(string message) {
        // Send the request
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0) {
        stream.ReadTimeout = timeout;
        try {
            return stream.ReadLine();
        } catch (TimeoutException) {
            return null;
        }
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do {
            // A single read attempt
            try {
                dataString = stream.ReadLine();
            } catch (TimeoutException) {
                dataString = null;
            }

            if (dataString != null) {
                callback(dataString);
                yield return null;
            } else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void OnApplicationQuit() {
        Debug.Log("Closing Arduino connection...");
        stream.Close();
    }
}