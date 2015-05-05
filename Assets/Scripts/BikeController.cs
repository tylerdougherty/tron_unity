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
		public List<Vector3> _trail;
		public List<Vector3> _otherTrail;
		private bool _dead;
		private float _speed;
		private Collider _collider;
		private int _playerNumber;
		private NetworkView _networkView;
		private Vector3 _myPosition;


		// Use this for initialization
		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_isRotating = false;
			_dead = false;
			_speed = Speed;
			_collider = GetComponent<Collider>();
			_networkView = GetComponent<NetworkView> ();
			
			_trail = new List<Vector3>();
			_otherTrail = new List<Vector3>();
			_myPosition = new Vector3 ();
			//This works for only two players, but it's something...
			if (Network.isClient)
				_playerNumber = 1;
			else
				_playerNumber = 0;
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

				if (rPressed && Network.isServer)
					Debug.Log ("restart");

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
				_myPosition = transform.position;
				_trail.Add (transform.position);
				//Debug.Log ("My Trail Count: " + _trail.Count);
				_networkView.RPC("getPosition", RPCMode.Others, _myPosition);
				//Debug.Log ("Other Trail Count: " + _otherTrail.Count);

				//Debug.Log (transform.position);
				//Debug.Log (_collider.transform.position);
				//Debug.Log ("---");

				_networkView.RPC ("CheckHit", RPCMode.All);
			} else {
				enabled = false;
			}
		}

		[RPC]
		void getPosition(Vector3 position){
			//Debug.Log ("Client: " +position);
			//Debug.Log (this._playerNumber + ": " + this._otherTrail.Count);
			this._otherTrail.Add (position);
		}

		[RPC]
		private void CheckHit()
		{
			Debug.Log (_playerNumber + ": " + _otherTrail.Count);
			for (var i = 0; i < _trail.Count - 1; i ++) {
				RaycastHit rh = new RaycastHit();
				if (Physics.Linecast (_trail [i], _trail [i+1],out rh)){
					if(rh.collider == _collider) {
						Debug.Log ("Hit My Line");
						_dead = true;
					}
				}
			}
			for (var i = 0; i < _otherTrail.Count - 1; i ++) {
				RaycastHit rh = new RaycastHit();
				if (Physics.Linecast (_otherTrail [i], _otherTrail [i+1],out rh)){
					if(rh.collider == _collider){
						Debug.Log ("Hit Other Line");
						_dead = true;
					}
				}
			}
		}

//		// Called once per frame-rate frame
//		private void FixedUpdate()
//		{
//			
//		}
	}
}
