using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public Camera camera;
    public Transform leftWheel, rightWheel;
    public float rotationMultiplier, movementMultiplier;

	Rigidbody rigidbody;

    public int playerHP = 20;
    public Text hpText;

	void Start() {
		rigidbody = GetComponent<Rigidbody> ();
	}

    void Update()
    {
        UpdateMovement();
        UpdateHP();

    }

    /// <summary>
    /// Checks the playerHP each update and writes it to the UI
    /// </summary>
    void UpdateHP()
    {
        if(playerHP <= 0)
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
    void UpdateMovement()
    {
        
        float left = Input.GetAxis("Vertical2");
        float right = Input.GetAxis("Vertical");

        float rotation = (left - right) * rotationMultiplier;
        float movement = (left + right) * movementMultiplier;

        rigidbody.AddTorque(Vector3.up * rotation);
        rigidbody.AddRelativeForce(Vector3.forward * movement);

    }

    /// <summary>
    /// Decreases the playerHP by the given amount
    /// </summary>
    /// <param name="enemyDamage">The amount of damage to be done to the player</param>
    public void Damage(int enemyDamage)
    {
        playerHP -= enemyDamage;
    }
}