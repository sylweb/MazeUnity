using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object


	private Vector3 offset;         //Private variable to store the offset distance between the player and camera

	private float offsetX;
	private float offsetZ;

	// Use this for initialization
	void Start () 
	{
		offsetX = transform.position.x- player.transform.position.x;
		offsetZ = transform.position.z - player.transform.position.z;
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		//The camera will follow x and z movement of the player but not y because we don't want the camera to follow the player when he jumps or when he falls
		Vector3 pos = new Vector3(player.transform.position.x + offsetX, transform.position.y, player.transform.position.z + offsetZ);
		transform.position = pos;
	}
}
