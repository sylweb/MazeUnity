using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {


	public float jump_max_force;

	private Rigidbody body;
	private float speed;
	private bool canJump=true;

	void Start ()
	{
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		body = GetComponent<Rigidbody>();
		canJump = true;

		if (SystemInfo.deviceType == DeviceType.Desktop) {
			speed = 200.0f;
		} else
			speed = 300.0f;

	}


	void Update() {
		
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

			if (Input.GetKeyDown(KeyCode.Space) && canJump == true) {
				body.AddForce (0.0f, jump_max_force, 0.0f);
				canJump = false;
			}
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
		if (transform.position.y < -5) {
			resetPosition ();
		}
	}
		
	void OnCollisionEnter (Collision other)
	{
		//If it's floor
		if(other.gameObject.tag == "floor_piece")
		{
			canJump = true;
		} 
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "collectable") {

			//Generate a new random position for the callectable
			float posX = Random.Range(-9.0f,7.0f);
			float posZ = Random.Range(-9.0f,7.0f);
			other.gameObject.GetComponent<Transform> ().position = new Vector3 (posX, -0.5f, posZ);

			//Remove randomly 3 floor pieces
			randomlyRemoveFloorPiece ();
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

	void randomlyRemoveFloorPiece() {
		
		//Make one floor piece disapear
		bool foundOneToRemove = false;
		do {
			float posX = Random.Range (-9.0f, 7.0f);
			float posZ = Random.Range (-9.0f, 7.0f);

			//We don't want to make the floor under the ball disapear
			if (posX < transform.position.x && transform.position.x < posX + 1)
				break;
			if (posZ < transform.position.z && transform.position.z < posZ + 1)
				break;

			foreach (GameObject floorPiece in GameObject.FindGameObjectsWithTag ("floor_piece")) {
				if (floorPiece.GetComponent<Transform> ().transform.position.x > posX && floorPiece.GetComponent<Transform> ().transform.position.x < posX + 1) {
					if (floorPiece.GetComponent<Transform> ().transform.position.z > posZ && floorPiece.GetComponent<Transform> ().transform.position.z < posZ + 1) {	

						if (floorPiece.GetComponent<Renderer> ().enabled == true) {
							floorPiece.GetComponent<Renderer> ().enabled = false;
							floorPiece.GetComponent<Collider> ().enabled = false;
							foundOneToRemove = true;
						}
						break;
					}
				}
			}
		} while(!foundOneToRemove);
	}
}
