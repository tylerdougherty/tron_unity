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
		private List<Vector3> _trail;
		private bool _dead;
		private Collider _collider;

		// Use this for initialization
		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_isRotating = false;
			_trail = new List<Vector3>();
			_dead = false;
			_collider = GetComponent<Collider>();
		}

		// Update is called once per frame
		private void Update()
		{
			if (_dead)
			{
				_rb.velocity = Vector3.zero;
				return;
			}

			// Get input
			var aPressed = Input.GetKey("a");
			var dPressed = Input.GetKey("d");

			if (!(aPressed || dPressed))
				_isRotating = false;

			if (aPressed && !_isRotating)
			{
				_isRotating = true;
				transform.Rotate(0, -90, 0);
			}
			if (dPressed && !_isRotating)
			{
				_isRotating = true;
				transform.Rotate(0, 90, 0);
			}

			_rb.velocity = transform.forward*Speed;

			// Store trail coordinates
			_trail.Add(transform.position);
			Debug.Log(transform.position);
			Debug.Log(_collider.transform.position);
			Debug.Log("---");

			if (Hit())
				_dead = true;
		}

		private bool Hit()
		{
			for (var i = 0; i < _trail.Count - 1; i++)
				if (Physics.Linecast(_trail[i], _trail[i + 1]))
					return true;
			return false;
		}

//		// Called once per frame-rate frame
//		private void FixedUpdate()
//		{
//			
//		}
	}
}
