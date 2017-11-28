﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public float speed;
	public float jump_max_force;

	private Rigidbody body;
	private bool canJump=true;

	void Start ()
	{
		body = GetComponent<Rigidbody>();
		foreach(GameObject floorPiece in GameObject.FindGameObjectsWithTag ("floor_piece"))
		{
			floorPiece.GetComponent<Renderer> ().enabled = false;;
		}
		canJump = true;
	
	}


	void Update() {
		if (Input.GetKeyDown(KeyCode.Space) && canJump == true) {
			body.AddForce (0.0f, jump_max_force, 0.0f);
			canJump = false;
		}
	}

	void FixedUpdate ()
	{
		if (SystemInfo.deviceType == DeviceType.Desktop) 
		{ 
			// Player movement in desktop devices
			// Definition of force vector X and Y components
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");
			// Building of force vector
			Vector3 movement = new Vector3 (moveHorizontal,0.0f,moveVertical);
			// Adding force to rigidbody
			body.AddForce(movement * speed * Time.deltaTime);
		}
		else
		{
			// Player movement in mobile devices
			// Building of force vector 
			Vector3 movement = new Vector3 (Input.acceleration.x, 0.0f, Input.acceleration.y);
			// Adding force to rigidbody
			body.AddForce(movement * speed * Time.deltaTime);
		}

		//reset ball position when player falls
		if (transform.position.y < -10) {
			resetPosition ();
		}
	}

	void resetPosition() {
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		body.inertiaTensorRotation = Quaternion.identity;
		body.inertiaTensor = Vector3.zero;
		body.isKinematic = true;
		body.isKinematic = false;
		Vector3 orig = new Vector3 (0.0f, 0.5f, 0.0f);
		transform.position = orig;
	}

	void OnCollisionEnter (Collision col)
	{
		//If it's floor
		if(col.gameObject.tag == "floor_piece")
		{
			canJump = true;
		}

		//If it's a collactable 
		if (col.gameObject.tag == "collectable") {
			float posX = Random.Range(-9.0f,7.0f);
			float posZ = Random.Range(-9.0f,7.0f);
			col.gameObject.GetComponent<Transform> ().position = new Vector3 (posX, -0.5f, posZ);
		}
	}
}
