using UnityEngine;
using System.Collections;

public class BikeController : MonoBehaviour {

	public float speed;

	private Rigidbody rb;
	private bool isRotating;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		isRotating = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		bool aPressed = Input.GetKey ("a");
		bool dPressed = Input.GetKey ("d");

		if (!(aPressed || dPressed))
			isRotating = false;

		if (aPressed && !isRotating) {
			isRotating = true;
			transform.Rotate (0,-90,0);
		}
		if (dPressed && !isRotating) {
			isRotating = true;
			transform.Rotate (0,90,0);
		}

		transform.Translate (Vector3.forward * speed);
	}
}
