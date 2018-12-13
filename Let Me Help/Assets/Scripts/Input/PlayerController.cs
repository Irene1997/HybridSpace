using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Camera camera;
    public Transform leftWheel, rightWheel;
    public float rotationMultiplier, movementMultiplier;


	Rigidbody rigidbody;

	void Start() {
		rigidbody = GetComponent<Rigidbody> ();
	}

    void Update()
    {
        //Quaternion rot = camera.transform.rotation;

        //float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        //x += (Input.GetAxis("Fire1") - Input.GetAxis("Fire2")) * Time.deltaTime * 150f;

        //      float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        //float scroll = Input.GetAxis("Mouse ScrollWheel");

        //float yRot = rot.eulerAngles.y;

        float left = Input.GetAxis("Vertical2");
        float right = Input.GetAxis("Vertical");



        float rotation = (left - right) * rotationMultiplier;
        float movement = (left + right) * movementMultiplier;

        //Debug.Log("Left: " + left + " Right: " + right);

        rigidbody.AddTorque(Vector3.up * rotation);
        rigidbody.AddRelativeForce(Vector3.forward * movement);

        //rigidbody.AddForceAtPosition(transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical") * 0.1f, rightWheel.position, ForceMode.Impulse);
        //rigidbody.AddForceAtPosition(transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical2") * 0.1f, leftWheel.position, ForceMode.Impulse);

		//rigidbody.AddRelativeForce (Vector3.forward * scroll * 10f, ForceMode.Impulse);
  //      //transform.Translate(0, 0, scroll);
		//transform.Rotate (0, x, 0);

		//transform.rotation = rot;

        //transform.Translate(0, 0, z);
    }
}