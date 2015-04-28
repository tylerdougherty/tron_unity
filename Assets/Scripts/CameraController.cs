using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public GameObject player;
	
	private Vector3 offset;
	
	// Use this for initialization
	void Start () {
		offset = this.transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//transform.position = player.transform.position + offset;
		//transform.rotation = Quaternion.Slerp (transform.rotation, player.transform.rotation, 1);
	}
}
