using UnityEngine;
using System.Collections;

public class TrailController : MonoBehaviour {

	/// The line renderer component that is attached to the gameObject.. 
	LineRenderer lineRenderer;
	/// The cube that we are finding in the scene. ///
	
	GameObject cube; // Use this for initialization 
	void Start () { 
		lineRenderer = gameObject.GetComponent<LineRenderer>(); 
		//the cube that is in the scene has beeen assigned the tag as a player. 
		cube = GameObject.FindWithTag("Player"); 
		//It has two indexes by default. The initial point denotes the Index 0, and the 
		//final point(of the line renderer) denotes the index 1 
		lineRenderer.SetPosition(0, cube.transform.position); 
		
		//Set the width of the line renderer. 
		lineRenderer.SetWidth(0.5F, 0.5F); 
	} 
	
	void Update() { 
		//the end position of the line will follow the player where ever it goes. 
		//This is the effect that I am talking about. 
		lineRenderer.SetPosition(1, cube.transform.position); 
	} 
}
