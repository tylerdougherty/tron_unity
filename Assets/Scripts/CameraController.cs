using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject playerPrefab;
	public Vector3 offset;

	private GameObject player;
	private bool following = false;
	private GameObject followedPlayer;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (!following)
		{
			followedPlayer = GameObject.Find("MyPlayer");
			if (followedPlayer != null)
			{
				transform.parent = followedPlayer.transform;
				following = true;
			}
		}

		if (following && followedPlayer != null)
		{
			var newOffset = new Vector3(offset.x, offset.y, offset.z);

			transform.rotation = followedPlayer.transform.rotation;
			transform.localPosition = newOffset;
		}

		//transform.position = player.transform.position + offset;
		//transform.rotation = Quaternion.Slerp (transform.rotation, player.transform.rotation, 1);
	}
}
