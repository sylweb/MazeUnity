using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public Transform target;
	private Rigidbody body;
	public float speed;
	public float smoothing = 1.0f; 
	private Vector3 offset;

	public LineRenderer directionToNextCoin;

	GameObject[] floor;
	GameObject coin;

	// Use this for initialization
	void Start () {
		this.body = GetComponent<Rigidbody> ();
		offset = target.position - transform.position;
		floor = GameObject.FindGameObjectsWithTag("floor_piece");
		coin = GameObject.FindGameObjectWithTag ("collectable");
		directionToNextCoin = GetComponent <LineRenderer> ();
		directionToNextCoin.enabled = true;
		directionToNextCoin.startColor = Color.green;
		directionToNextCoin.endColor = Color.green;
	}

	void ResetGame() {
		for (int i = 0; i < floor.Length; i++) {
			floor [i].GetComponent<Renderer> ().enabled = true;
			floor [i].GetComponent<Collider> ().enabled = true;
		}

		transform.position = new Vector3 (-0.75f,-0.5f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y < -2)
			ResetGame ();

		if (SystemInfo.deviceType == DeviceType.Desktop) {

			float xAxis = 0.0f;
			float yAxis = 0.0f;

			Vector3 vec = new Vector3 (Time.deltaTime * speed * Input.GetAxis ("Horizontal"), 0.0f, Time.deltaTime * speed * Input.GetAxis ("Vertical"));
			this.body.AddForce (vec);
		} else {
			Vector3 dir = Vector3.zero;
			dir.x = Input.acceleration.x;
			dir.z = Input.acceleration.y;
			if (dir.sqrMagnitude > 1) dir.Normalize ();

			dir = dir * Time.deltaTime * speed;
			this.body.AddForce (dir);
		}
			
		// Smoothly interpolate between the camera's current position and it's target position.
		Vector3 targetCamPos = transform.position + offset;//
		target.position = Vector3.Lerp (target.position, targetCamPos, smoothing);

		//A ray that point to the next coin location
		directionToNextCoin.SetPosition (0, transform.position);
		directionToNextCoin.SetPosition (1, coin.transform.position);

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == coin) HitACoin ();
	}

	void HitACoin() {
		//Randomly generate a new position for the orb
		int x = (int) Random.Range (-9.0f, 7.0f);
		int z = (int) Random.Range (-9.0f, 7.0f);
		coin.transform.position = new Vector3(x,-0.5f,z);

		//Randomly delete a floor piece
		int floorIndex = (int)Random.Range (0.0f, floor.Length-1);
		floor[floorIndex].GetComponent<MeshRenderer>().enabled = false;
		floor [floorIndex].GetComponent<Collider> ().enabled = false;
	}
}
