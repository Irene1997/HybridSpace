﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

public class ArduinoHandler : MonoBehaviour {
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;
    [Tooltip("The ReadTimeout of the streams in milliseconds")]
    public int readTimeout = 1;
    [Tooltip("The read delay after a failed read attempt in seconds")]
    public float readDelay = 0.05f;
    // The dictionary containing all created Arduino connections by name
    public IDictionary<string, Arduino> namedArduinos;
    IList<Arduino> arduinos;


    public void Start() {
        arduinos = new List<Arduino>();
        namedArduinos = new Dictionary<string, Arduino>();
        // Get all connected ports
        string[] ports = SerialPort.GetPortNames();
        // Try to make a connection to an Arduino for each port
        foreach (string port in ports) {
            Arduino arduino = Arduino.StartArduino(port, readTimeout, readDelay);
            if (arduino != null) {
                StartCoroutine(arduino.AsynchronousReadFromArduino((string s) => ReadMessage(arduino, s)));
                arduinos.Add(arduino);
            }
        }

        // Send initializing commands to each Arduino
        foreach (Arduino arduino in arduinos) {
            arduino.Write("N");
        }
    }

    // Handle incomming messages from the Aruinos
    void ReadMessage(Arduino arduino, string message) {

        Debug.Log("Received '" + message + "' from " + arduino.name);
        switch (message[0]) {
            case 'N':

                string name = message.TrimStart("N ".ToCharArray());
                arduino.name = name;
                namedArduinos.Add(new KeyValuePair<string, Arduino>(name, arduino));
                if (name == "MazeArduino") {
                    arduino.Write("D");
                } else {
                    Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
                }

                break;
            case 'D':

                int doorStates = int.Parse(message.Split(' ')[1]);
                for (int i = 0; i < GameController.Instance.doorScripts.Length; ++i) {
                    if (((doorStates >> i) & 1) == 1) {
                        GameController.Instance.doorScripts[i].Open();
                    } else {
                        GameController.Instance.doorScripts[i].Close();
                    }
                }

                Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");

                break;
            case 'R':

                string[] parts = message.Split(' ');
                GameController.Instance.playerScript.UpdateMovement(int.Parse(parts[1]), int.Parse(parts[2]));
                break;

            default:
                Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
                break;
        }

        //Debug.Log("Received '" + message + "' from " + arduino.name);
        //switch (arduino.name) {
        //    case "Unnamed":
        //        if (message[0] == 'N') {
        //            string name = message.TrimStart("N ".ToCharArray());
        //            arduino.name = name;
        //            namedArduinos.Add(new KeyValuePair<string, Arduino>(name, arduino));
        //            if (name == "MazeArduino") {
        //                arduino.Write("D");
        //            } else {
        //                Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
        //            }
        //        }
        //        break;
        //    case "MazeArduino":
        //        if (message[0] == 'D') {
        //            int doorStates = int.Parse(message.Split(' ')[1]);
        //            for(int i = 0; i < GameController.Instance.doorScripts.Length; ++i) {
        //                if (((doorStates >> i) & 1) == 1) {
        //                    GameController.Instance.doorScripts[i].Open();
        //                } else {
        //                    GameController.Instance.doorScripts[i].Close();
        //                }
        //            }
        //        } else {
        //            Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
        //        }
        //        break;
        //    case "Wheelchair":
        //        switch (message[0]) {
        //            case 'R':
        //                string[] parts = message.Split(' ');
        //                GameController.Instance.playerScript.UpdateMovement(int.Parse(parts[1]), int.Parse(parts[2]));
        //                break;
        //            default:
        //                Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
        //                break;
        //        }
        //        break;
        //    default:
        //        Debug.Log("Message '" + message + "' from " + arduino.name + " could not be processed.");
        //        break;
        //}
    }

    // Try to send a message to an Arduino
    public void WriteToArduino(string arduinoName, string message) {
        if (namedArduinos.ContainsKey(arduinoName)) {
            namedArduinos[arduinoName].Write(message);
        } else {
            Debug.Log(arduinoName + " is not a known name");
        }
    }

    // Close connections on exit
    public void OnApplicationQuit() {
        Debug.Log("Closing Arduino connections...");
        foreach (Arduino arduino in arduinos) {
            arduino.Close();
        }
    }
}

public class Arduino {
    public string name = "Unnamed", port;
    public SerialPort stream;
    float readDelay;

    // Returns an Arduino instance if a connection with the port could be made
    public static Arduino StartArduino(string port, int readTimeout, float readDelay, int baudrate = 9600) {
        SerialPort stream;
        try {
            stream = new SerialPort(port, baudrate) { ReadTimeout = readTimeout };
            stream.Open();
            return new Arduino(port, stream, readDelay);
        } catch (Exception e) {
            Debug.LogWarning(e);
            return null;
        }
    }

    // The constructor for the Arduino class
    Arduino(string port, SerialPort stream, float readDelay) {
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
                    callback(dataString.TrimEnd('\r', '\n'));
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
