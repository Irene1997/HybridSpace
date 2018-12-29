using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

public class ArduinoHandler : MonoBehaviour {
    [Tooltip("The names of the Aruinos")]
    public string[] names = { "ARD1", "ARD2" };
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;
    [Tooltip("The ReadTimeout of the streams in milliseconds")]
    public int readTimeout = 1;
    [Tooltip("The read delay after a failed read attempt in seconds")]
    public float readDelay = 0.05f;
    // The dictionary containing all created Arduino connections by name
    IDictionary<string, Arduino> arduinos;

    public void Start() {
        arduinos = new Dictionary<string, Arduino>();
        int nextName = 0;
        // Get all connected ports
        string[] ports = SerialPort.GetPortNames();
        // Try to make a connection to an Arduino for each port
        foreach (string port in ports) {
            if (nextName < names.Length) {
                Arduino arduino = Arduino.StartArduino(names[nextName], port, readTimeout, readDelay);
                if (arduino != null) {
                    string name = names[nextName];
                    StartCoroutine(arduino.AsynchronousReadFromArduino((string s) => ReadMessage(name + s)));
                    arduinos.Add(new KeyValuePair<string, Arduino>(name, arduino));
                    nextName++;
                }
            }

        }

        // Send initializing commands to each Arduino
        foreach (Arduino arduino in arduinos.Values) {
            arduino.Write("LED_ON");
            arduino.Write("ASK_STATE");
        }
    }

    // Handle incomming messages from the Aruinos
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
        //} else if (message == names[0] + "LeftUp" || message == names[1] + "LeftUp") {
        //    GameController.Instance.playerScript.UpdateMovement(1, 0);
        //} else if (message == names[0] + "LeftDown" || message == names[1] + "LeftDown") {
        //    GameController.Instance.playerScript.UpdateMovement(-1, 0);
        //} else if (message == names[0] + "RightUp" || message == names[1] + "RightUp") {
        //    GameController.Instance.playerScript.UpdateMovement(1, 0);
        //} else if (message == names[0] + "RightDown" || message == names[1] + "RightDown") {
        //    GameController.Instance.playerScript.UpdateMovement(-1, 0);
        } else {
            string[] parts = message.Split(' ');
            if (parts[0] == names[0] + "Left" || parts[0] == names[1] + "Left") {
                GameController.Instance.playerScript.ChangePosLeft(int.Parse(parts[1]));
            } else if (parts[0] == names[0] + "Right" || parts[0] == names[1] + "Right") {
                GameController.Instance.playerScript.ChangePosRight(int.Parse(parts[1]));
            } else {
                Debug.Log("Message '" + message + "' could not be processed.");
            }
        }
    }

    // Close connections on exit
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
    float readDelay;

    // Returns an Arduino instance if a connection with the port could be made
    public static Arduino StartArduino(string name, string port, int readTimeout, float readDelay, int baudrate = 9600) {
        SerialPort stream;
        try {
            stream = new SerialPort(port, baudrate) { ReadTimeout = readTimeout };
            stream.Open();
            return new Arduino(name, port, stream, readDelay);
        } catch (Exception e) {
            Debug.LogWarning(e);
            return null;
        }
    }

    // The constructor for the Arduino class
    Arduino(string name, string port, SerialPort stream, float readDelay) {
        this.name = name;
        this.port = port;
        this.stream = stream;
        this.readDelay = readDelay;
    }

    // Write a message to the Arduino
    public void Write(string message) {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    // Reads messages from the stream
    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = "";
        int readChar = -1;

        do {
            // Tries to read a character
            try {
                readChar = stream.ReadByte();
            } catch (TimeoutException) {
                readChar = -1;
            }
            if (readChar == -1) {
                yield return new WaitForSeconds(readDelay);
            } else {
                dataString += (char)readChar;
                // Executes the callback if the line has ended
                if (dataString.EndsWith(stream.NewLine)) {
                    callback(dataString.TrimEnd('\r','\n'));
                    dataString = "";
                }
                yield return null;
            }

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    // Closes the stream
    public void Close() {
        stream.Close();
    }
}
