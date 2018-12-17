using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

public class ArduinoHandler : MonoBehaviour {
    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial ports where the Arduinos are connected")]
    public string[] ports = { "COM3", "COM4", "COM5" };
    [Tooltip("The names of the Aruinos")]
    public string[] names = { "ARD1", "ARD2" };
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;

    IDictionary<string, Arduino> arduinos;

    public void Start() {
        arduinos = new Dictionary<string, Arduino>();
        int nextName = 0;
        foreach (string port in ports) {
            if (nextName < names.Length) {
                Arduino arduino = Arduino.StartArduino(names[nextName], port);
                if (arduino != null) {
                    StartCoroutine(arduino.AsynchronousReadFromArduino((string s) => ReadMessage(names[nextName] + s)));
                    arduinos.Add(new KeyValuePair<string, Arduino>(names[nextName], arduino));
                    nextName++;
                }
            }

        }

        foreach (Arduino arduino in arduinos.Values) {
            arduino.Write("LED_ON");
            arduino.Write("ASK_STATE");
        }
    }

    void ReadMessage(string message) {
        Debug.Log("Received '" + message + "' from an Arduino.");
        if (message == names[0] + "Open") {
            GameController.Instance.doors[0].Open();
        } else if (message == names[0] + "Close") {
            GameController.Instance.doors[0].Close();
        } else if (message == names[1] + "Open") {
            GameController.Instance.doors[1].Open();
        } else if (message == names[1] + "Close") {
            GameController.Instance.doors[1].Close();
        } else {
            Debug.Log("Message '" + message + "' could not be processed.");
        }
    }

    public void OnApplicationQuit() {
        Debug.Log("Closing Arduino connections...");
        foreach (Arduino arduino in arduinos.Values) {
            arduino.Close();
        }
    }
}

class Arduino {
    public string name, port;
    public SerialPort stream;

    public static Arduino StartArduino(string name, string port, int baudrate = 9600) {
        SerialPort stream;
        try {
            stream = new SerialPort(port, baudrate) { ReadTimeout = 15 };
            stream.Open();
            return new Arduino(name, port, stream);
        } catch (Exception e) {
            Debug.LogWarning(e);
            return null;
        }
    }

    Arduino(string name, string port, SerialPort stream) {
        this.name = name;
        this.port = port;
        this.stream = stream;
    }

    public void Write(string message) {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    //public IEnumerator Coroutine(Action<string> callback) {
    //    return AsynchronousReadFromArduino(stream, callback, () => Debug.LogError("Error!"), 10000f);
    //}


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
                yield return new WaitForSeconds(0.01f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void Close() {
        stream.Close();
    }
}
