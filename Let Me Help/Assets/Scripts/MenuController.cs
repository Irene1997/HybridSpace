using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PlayerInput();

	}

    void PlayerInput()
    {
        float left = Input.GetAxis("Vertical2");
        float right = Input.GetAxis("Vertical");

        
        if (Input.GetAxis("Vertical2") != 0)
        { //left Wheel == Start
            SceneManager.LoadScene(1);
        }
        else if (Input.GetAxis("Vertical") != 0)
        { //right Wheel == Stop
            Application.Quit();
        }
    }
}
