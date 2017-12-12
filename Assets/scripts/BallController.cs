using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public Transform camPos;
	public GameObject scoreText;
	public GameObject fpsText;
	public float smoothing;

	private Rigidbody body; 
	private LineRenderer directionToNextCoin;
	private Vector3 camOffset;
	private Vector3 scoreOffset;
	private Vector3 fpsOffset;

	private GameObject[,] floor;
	private GameObject coin;
	private float coinPositionY;
	private float speed;
	private int score = 0;
	private Vector3 ballOrigine;


	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		this.body = GetComponent<Rigidbody> ();
		camOffset = camPos.position - transform.position;
		scoreOffset = scoreText.transform.position - transform.position;
		fpsOffset = fpsText.transform.position - transform.position;


		GameObject[] temp = GameObject.FindGameObjectsWithTag("floor_piece");

		floor = new GameObject[17,17];
		for (int i = 0; i < temp.Length; i++) {
			floor [(int)(temp [i].transform.position.x + 9),(int)(temp [i].transform.position.z + 9)] = temp [i];
		}

		coin = GameObject.FindGameObjectWithTag ("collectable");
		this.coinPositionY = coin.transform.position.y;
		directionToNextCoin = GetComponent <LineRenderer> ();
		directionToNextCoin.enabled = true;
		directionToNextCoin.startColor = Color.green;
		directionToNextCoin.endColor = Color.green;
		speed = 600.0f;
		if (SystemInfo.deviceType == DeviceType.Handheld) speed = (speed * 70 / 100) + speed;

		scoreText.GetComponent<TextMesh> ().text = "Score : 0";
		score = 0;

		ballOrigine = this.transform.position;
	}

	void ResetGame() {
		/*for (int i = 0; i < floor.Length; i++) {
			floor [i].GetComponent<Renderer> ().enabled = true;
			floor [i].GetComponent<Collider> ().enabled = true;
		}*/

		transform.position = ballOrigine;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit(); 
			
		// Smoothly interpolate between the camera's current position and it's target position.
		Vector3 targetCamPos = transform.position + camOffset;//
		camPos.position = Vector3.Lerp (camPos.position, targetCamPos, smoothing);

		//Replace HUD text
		scoreText.transform.position = transform.position + scoreOffset;
		fpsText.transform.position = transform.position + fpsOffset;
		int fps = (int)(1.0f / Time.deltaTime);
		fpsText.GetComponent<TextMesh> ().text = ""+fps;

		//A ray that point to the next coin location
		directionToNextCoin.SetPosition (0, transform.position);
		directionToNextCoin.SetPosition (1, coin.transform.position);

	}

	void FixedUpdate() {

		if (transform.position.y < -2)
			ResetGame ();

		if (SystemInfo.deviceType == DeviceType.Desktop) {

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
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == coin) HitACoin ();
	}

	void HitACoin() {

		score = score + 1;
		scoreText.GetComponent<TextMesh> ().text = "Score : "+score;
		
		//Randomly delete a floor piece
		int floorXIndex = 0;
		int floorZIndex = 0;
		do {
			floorXIndex = (int)Random.Range (0.0f, 16.0f);
			floorZIndex = (int)Random.Range (0.0f, 16.0f);
		}while(floor[floorXIndex,floorZIndex] == null || floor[floorXIndex,floorZIndex].GetComponent<Collider> ().enabled == false);
		floor[floorXIndex, floorZIndex].GetComponent<MeshRenderer>().enabled = false;
		floor[floorXIndex, floorZIndex].GetComponent<Collider> ().enabled = false;

		//Randomly generate a new position for the orb but not in a hole
		int x = 0;
		int z = 0;
		bool tooCloseToBall = false;
		do {

			x = (int)Random.Range (-9.0f, 7.0f);
			z = (int)Random.Range (-9.0f, 7.0f);

			tooCloseToBall = false;
			if(transform.position.x > (x-1) && transform.position.x < (x+1) && transform.position.z > (z-1) && transform.position.z < (z+1))
				tooCloseToBall = true;

		} while(floor [x+9, z+9] == null || floor [x+9, z+9].GetComponent<Collider> ().enabled == false || tooCloseToBall);
		coin.transform.position = new Vector3(x,coinPositionY,z);
	}
}
