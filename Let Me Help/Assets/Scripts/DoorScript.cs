using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorScript : MonoBehaviour {
    enum State { Open, Closed, Opening, Closing }

    public int id;
    [SerializeField]
    State state;
    public float speed;
    [SerializeField]
    float openProgress = 1f;
    public bool startOpen;

    Vector3 closedPosition, openVector;

    // Use this for initialization
    void Start() {
        openVector = Vector3.down * transform.lossyScale.y;
        closedPosition = transform.position - openVector;
        if (startOpen) {
            SetOpen();
        } else {
            SetClosed();
        }
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case State.Opening:
                openProgress += speed * Time.deltaTime;
                if (openProgress >= 1f) {
                    SetOpen();
                } else {
                    transform.position = closedPosition + openVector * openProgress;
                }
                break;
            case State.Closing:
                openProgress -= speed * Time.deltaTime;
                if (openProgress <= 0f) {
                    SetClosed();
                } else {
                    transform.position = closedPosition + openVector * openProgress;
                }
                break;
            default:
                break;
        }
    }

    public void SetOpen() {
        state = State.Open;
        openProgress = 1f;
        transform.position = closedPosition + openVector;
    }

    public void SetClosed() {
        state = State.Closed;
        openProgress = 0f;
        transform.position = closedPosition;
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
