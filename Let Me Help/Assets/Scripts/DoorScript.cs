using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorScript : MonoBehaviour {
    enum State { Open, Closed, Opening, Closing }

    [SerializeField]
    State state;
    public float speed;
    public Vector3 doorScale;
    public bool startOpen;

    // Use this for initialization
    void Start() {
        doorScale = transform.parent.localScale;
        if (startOpen) {
            SetOpen();
        } else {
            SetClosed();
        }
        //For testing purposes
        //Open();
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case State.Opening:
                doorScale.y -= speed * Time.deltaTime;
                if (doorScale.y <= 0f) {
                    SetOpen();
                } else {
                    transform.parent.localScale = doorScale;
                }
                break;
            case State.Closing:
                doorScale.y += speed * Time.deltaTime;
                if (doorScale.y >= 1f) {
                    SetClosed();
                } else {
                    transform.parent.localScale = doorScale;
                }
                break;
            default:
                break;
        }
    }

    public void SetOpen() {
        state = State.Open;
        doorScale.y = 0f;
        transform.parent.localScale = doorScale;
    }

    public void SetClosed() {
        state = State.Closed;
        doorScale.y = 1f;
        transform.parent.localScale = doorScale;
    }

    public void Open() {
        if (state != State.Open) {
            state = State.Opening;
        }
    }

    public void Close() {
        if (state != State.Closed) {
            state = State.Closing;
        }
    }
}
