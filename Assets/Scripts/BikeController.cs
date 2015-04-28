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

		bool wPressed = Input.GetKey ("a");
		bool dPressed = Input.GetKey ("d");

		if (!(wPressed || dPressed))
			isRotating = false;

		if (wPressed && !isRotating) {
			isRotating = true;
			rb.transform.Rotate (0,90,0);
		}
		if (dPressed && !isRotating) {
			isRotating = true;
			rb.transform.Rotate (0,-90,0);
		}

		Vector3 movement = new Vector3 (0, 0, 1);
		rb.AddForce (movement * speed);
	}
}
