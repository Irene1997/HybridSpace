using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoHandler : MonoBehaviour {
    SerialPort stream;

	// Use this for initialization
	void Start () {
        stream = new SerialPort("COM4", 9600);
        stream.ReadTimeout = 50;
        stream.Open();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
