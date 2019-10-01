using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PipeMotion : MonoBehaviour {

	public float pipeSpeed = 50f;

	Rigidbody myRigidbody;

	bool canScore = true;

	void Start () {
		myRigidbody = GetComponent<Rigidbody> ();
	}

	void Update () {
		myRigidbody.velocity = new Vector3 (-pipeSpeed * Time.deltaTime, 0f, 0f);

		if (transform.position.x < -8.05f && canScore) {
			canScore = false;
			if (GameController.instance.playerScore != null) {
				GameController.instance.playerScore ();
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Player")) {
			if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("FlappyBird")) {
				GameController.instance.isGameOver = true;
			} else if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Neuroevolution")) {
				if (other.gameObject.GetComponent<Player> ().destroyPlayer != null) {
					other.gameObject.GetComponent<Player> ().destroyPlayer ();
				}
			}
		}

		if (other.CompareTag ("LeftBorder")) {
			Destroy (transform.parent.gameObject);
		}
	}
}
