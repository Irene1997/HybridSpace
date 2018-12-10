using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class ControllerInput : MonoBehaviour {

    // 1
    private SteamVR_TrackedObject trackedObj;
    // 2
    //private SteamVR_Controller.Device Controller {
    //    get { return SteamVR_Controller.Input((int)trackedObj.index); }
    //}
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update () {
        //// 1
        //if (Controller.GetAxis() != Vector2.zero)
        //{
        //    Debug.Log(gameObject.name + Controller.GetAxis());
        //}
    }
}
