using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oh shit it's that one class whose name is way more badass than what it actually does
/// </summary>
public class DeathController : MonoBehaviour {

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
        { //left Wheel == BackToMenu
            SceneManager.LoadScene(0);
        }
        else if (Input.GetAxis("Vertical") != 0)
        { //right Wheel == Restart Game
            SceneManager.LoadScene(1);
        }
    }
}
