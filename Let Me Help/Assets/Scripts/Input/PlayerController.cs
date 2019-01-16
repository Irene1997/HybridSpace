using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Camera camera;
    public Transform leftWheel, rightWheel;
    public GameObject modelLeftWheel, modelRightWheel;
    public float rotationMultiplier, movementMultiplier;
    //int currentPosLeft = 0, currentPosRight = 0;

    Rigidbody rigidbody;

    public int playerHP = 20;
    public Text hpText;

    float left, right;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //OldUpdateMovement();
        //UpdateMovement(left, right);
        //left = 0; right = 0;
        OldUpdateMovement();
        UpdateMovement(left / 18, right / 18);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("coll: " + col.gameObject.tag);
        if (col.gameObject.tag == "Finish")
        {
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Checks the playerHP each update and writes it to the UI
    /// </summary>
    void UpdateHP()
    {
        if (playerHP <= 0)
        {
            GameController.Instance.PlayerDied();
        }
        else
        {
            hpText.text = "HP: " + playerHP.ToString();
        }

    }


    /// <summary>
    /// Checks for movement from the 'wheels' and moves the player
    /// </summary>
    void OldUpdateMovement()
    {

        float left = Input.GetAxis("Vertical2");
        float right = Input.GetAxis("Vertical");

        float rotation = (left - right) * rotationMultiplier * 2;
        float movement = (left + right) * movementMultiplier;

        rigidbody.AddTorque(Vector3.up * rotation);
        rigidbody.AddRelativeForce(Vector3.forward * movement);

        MoveModelWheel(left, right);
    }

    //public void ChangePosLeft(int newPosLeft) {
    //    float leftChange = newPosLeft - currentPosLeft;
    //    currentPosLeft = newPosLeft;
    //    UpdateMovement(leftChange, 0);
    //}

    //public void ChangePosRight(int newPosRight) {
    //    float rightChange = newPosRight - currentPosRight;
    //    currentPosRight = newPosRight;
    //    UpdateMovement(0, rightChange);
    //}

    public void UpdateMovement(float leftChange, float rightChange)
    {

        float rotation = (leftChange - rightChange) * rotationMultiplier;
        float movement = (leftChange + rightChange) * movementMultiplier;

        rigidbody.AddTorque(Vector3.up * rotation);
        rigidbody.AddRelativeForce(Vector3.forward * movement);

        MoveModelWheel(leftChange, rightChange);
    }

    void MoveModelWheel(float leftChange, float rightChange)
    {
        modelLeftWheel.transform.Rotate(Vector3.forward * leftChange * 10);
        modelRightWheel.transform.Rotate(Vector3.forward * rightChange * 10);
    }

    public void CallMovement(float leftChange, float rightChange)
    {
        left = leftChange; right = rightChange;
    }


    /// <summary>
    /// Decreases the playerHP by the given amount
    /// </summary>
    /// <param name="enemyDamage">The amount of damage to be done to the player</param>
    public void Damage(int enemyDamage)
    {
        playerHP -= enemyDamage;
        UpdateHP();
    }
}