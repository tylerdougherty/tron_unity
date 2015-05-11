using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class BikeController : MonoBehaviour
	{
		public float Speed;

		private static List<Vector3>[] _trails;
		private static bool _dead = false;

		private Rigidbody _rb;
		private int _playerNumber;
		private NetworkView _networkView;

		// Use this for initialization
		private void Start()
		{
			_networkView = GetComponent<NetworkView>();
			_playerNumber = int.Parse(_networkView.owner.ToString());

			//Network player specific execution
			if (_networkView.isMine)
			{
				_rb = GetComponent<Rigidbody>();
				name = "MyPlayer";
			}

			//Initialize trails
			if (_trails == null)
			{
				var n = Network.connections.Length + 1;
				_trails = new List<Vector3>[n];
				for (var q = 0; q < n; q++)
				{
					_trails[q] = new List<Vector3>();
				}
			}
		}

		// Update is called once per frame
		private void Update()
		{
			// Get input
			if (_networkView.isMine)
			{
				if (_dead)
				{
					_rb.velocity = Vector3.zero;
					return;
				}

				if (Input.GetKeyDown("a")) //left
					transform.Rotate(0, -90, 0);
				if (Input.GetKeyDown("d")) //right
					transform.Rotate(0, 90, 0);

				_rb.velocity = transform.forward * Speed;
			}

			//Deal with storing the trails and collisions
			if (Network.isServer)
			{
				_trails[_playerNumber].Add(transform.position);

				if (_networkView.isMine)
				{
					CheckTrailHit();
				}
			}
		}

		private void CheckTrailHit()
		{
			foreach (var currentTrail in _trails)
				for (var y = 0; y < currentTrail.Count - 1; y++)
				{
					RaycastHit rh = new RaycastHit();
					if (Physics.Linecast(currentTrail[y], currentTrail[y + 1], out rh))
					{
						if (rh.collider.gameObject.tag == "Player")
						{
							var dead = rh.collider.gameObject.GetComponent<BikeController>()._networkView;

							var n = int.Parse(dead.owner.ToString());
							if (n == 0)
								Die();
							else
								_networkView.RPC("Die", dead.owner);

							//Network.Destroy(dead.viewID);
						}
					}
				}
		}

		[RPC]
		private void Die()
		{
			_dead = true;
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player" || other.gameObject.tag == "Collider")
			{
				Die();
			}
		}

		/*
		[RPC]
		void getPosition(Vector3 position)
		{
			//Debug.Log ("Client: " +position);
			//Debug.Log (this._playerNumber + ": " + this._otherTrail.Count);
			this._otherTrail.Add(position);
		}
		*/

		/*
		[RPC]
		private void CheckTrailHit()
		{
			Debug.Log(_playerNumber + ": " + _otherTrail.Count);
			for (var i = 0; i < _trail.Count - 1; i++)
			{
				RaycastHit rh = new RaycastHit();
				if (Physics.Linecast(_trail[i], _trail[i + 1], out rh))
				{
					if (rh.collider == _collider)
					{
						Debug.Log("Hit My Line");
						_dead = true;
					}
				}
			}
		}
		*/
	}
}
