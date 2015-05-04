using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class BikeController : MonoBehaviour
	{
		public float Speed;

		private Rigidbody _rb;
		private bool _isRotating;
		private List<List<Vector3>> _trails;
		private bool _dead;
		private float _speed;
		private Collider _collider;
		private int playerNumber;


		// Use this for initialization
		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_isRotating = false;
			_dead = false;
			_speed = Speed;
			_collider = GetComponent<Collider>();
			
			_trails = new List<List<Vector3>>();
			_trails.Add (new List<Vector3> ());
			_trails.Add (new List<Vector3> ());
			//This works for only two players, but it's something...
			if (Network.isClient)
				playerNumber = 1;
			else
				playerNumber = 0;
		}

		// Update is called once per frame
		private void Update()
		{
			if (GetComponent<NetworkView>().isMine) {

				if (_dead) {
					_rb.velocity = Vector3.zero;
					return;
				}

				// Get input
				var aPressed = Input.GetKey ("a");
				var dPressed = Input.GetKey ("d");
				var rPressed = Input.GetKey ("r");

				//if (rPressed && Network.isServer)
					//Debug.Log ("restart");

				if (!(aPressed || dPressed))
					_isRotating = false;

				if (aPressed && !_isRotating) {
					_isRotating = true;
					transform.Rotate (0, -90, 0);
				}
				if (dPressed && !_isRotating) {
					_isRotating = true;
					transform.Rotate (0, 90, 0);
				}

				_rb.velocity = transform.forward * _speed;

				// Store trail coordinates
				_trails[playerNumber].Add (transform.position);
				//Debug.Log (transform.position);
				//Debug.Log (_collider.transform.position);
				//Debug.Log ("---");

				if (Hit ())
					_dead = true;
			} else {
				enabled = false;
			}
		}

		private bool Hit()
		{
			for (var i = 0; i < _trails.Count; i ++) {
				for (var j = 0; j < _trails[i].Count - 1; j++) {
					if (Physics.Linecast (_trails [i] [j], _trails [i] [j + 1]) && _collider == GetComponent<Collider>())
						return true;
				}
			}
			return false;
		}

//		// Called once per frame-rate frame
//		private void FixedUpdate()
//		{
//			
//		}
	}
}
