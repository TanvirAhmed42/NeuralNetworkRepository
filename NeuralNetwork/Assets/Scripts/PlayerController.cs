using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float gravity = -9.8f;
	public float jumpHeight = 3f;

	Rigidbody myRigidbody;

	bool isJumping = false;

	void Start () {
		myRigidbody = GetComponent<Rigidbody> ();
	}

	void Update () {
		CustomGravity ();

		if (FlappyBirdNeuralNetwork.instance.playMode == FlappyBirdNeuralNetwork.PlayMode.SupervisedTraining) {
			if (!GameController.instance.isGameOver) {
				if (Input.GetKeyDown (KeyCode.Space)) {
					isJumping = true;
					myRigidbody.velocity = new Vector3 (myRigidbody.velocity.x, CalculateVelocity () * 2f, 0f);
				} else {
					isJumping = false;
				}
			}
		}
	}

	public void CustomJump () {
		myRigidbody.velocity = new Vector3 (myRigidbody.velocity.x, CalculateVelocity () * 2f, 0f);
	}

	void CustomGravity () {
		myRigidbody.AddForce (0f, gravity * Time.deltaTime, 0f);
	}

	float CalculateVelocity () {
		float velocity = Mathf.Sqrt (-2 * gravity * jumpHeight);

		return velocity * Time.deltaTime;
	}

	public float[] GetPlayerInput () {
		float[] input = new float[1];

		if (isJumping) {
			input [0] = 1f;
		} else {
			input [0] = 0f;
		}

		return input;
	}
}
