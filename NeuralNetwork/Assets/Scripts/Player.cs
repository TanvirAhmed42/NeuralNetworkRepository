using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public delegate void PlayerCollided ();
	public PlayerCollided destroyPlayer;

	public NeuralNetwork nn;
	public NeuralNetworkSaveData data;

	public float gravity = -9.8f;
	public float jumpHeight = 3f;
	[HideInInspector]
	public float fitness;

	Rigidbody myRigidbody;

	GameObject bottomBorder;
	GameController gameController;

	void Start () {
		myRigidbody = GetComponent<Rigidbody> ();
		nn = new NeuralNetwork (3, 3, 2);
		bottomBorder = GameObject.FindGameObjectWithTag ("BottomBorder");
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		data = new NeuralNetworkSaveData (nn);
		destroyPlayer += DestroySelf;
	}

	void Update () {
		CustomGravity ();

		float[] inputs = GetInputs ();

		float[] outputs = nn.PredictOutput (inputs);

		if (outputs [0] > outputs [1]) {
			CustomJump ();
		}

		fitness += Time.deltaTime;

		//Debug.Log (outputs [0] + " " + outputs [1]);
	}

	void OnDisable () {
		destroyPlayer -= DestroySelf;
	}

	float[] GetInputs () {
		float[] inputs = new float[3];

		float[] heights = GetPipeHeight ();
		float[] distances = GetPipeDistances ();

		inputs [0] = PlayerHeight ();
		inputs [1] = heights [0];
		inputs [2] = distances [0];

		return inputs;
	}

	void CustomGravity () {
		myRigidbody.AddForce (0f, gravity * Time.deltaTime, 0f);
	}

	public void CustomJump () {
		myRigidbody.velocity = new Vector3 (myRigidbody.velocity.x, CalculateVelocity () * 2f, 0f);
	}

	float CalculateVelocity () {
		float velocity = Mathf.Sqrt (-2 * gravity * jumpHeight);

		return velocity * Time.deltaTime;
	}

	float PlayerHeight () {
		float distance = transform.position.y - bottomBorder.transform.position.y - 1f;

		return distance;
	}

	float[] GetPipeHeight () {
		Transform[] pipes = gameController.GetFirstSecondPipes ();
		float[] distances = new float[2];

		distances[0] = pipes[0].position.y - bottomBorder.transform.position.y + 1.5f;
		if (pipes [1] == null) {
			distances [1] = 5;
		} else {
			distances [1] = pipes [1].position.y - bottomBorder.transform.position.y + 1.5f;
		}

		return distances;
	}

	float[] GetPipeDistances () {
		Transform[] pipes = gameController.GetFirstSecondPipes ();
		float[] distances = new float[2];

		distances[0] = pipes[0].position.x - transform.position.x;
		if (pipes [1] == null) {
			distances [1] = distances [0] + 3f;
		} else {
			distances [1] = pipes [1].position.x - transform.position.x;
		}

		return distances;
	}

	void DestroySelf () {
		Neuroevolution.instance.backupPopulation.Add (nn);
		Neuroevolution.instance.playerPopulation.Remove (this);
		Neuroevolution.instance.fitnessValues.Add (fitness * fitness);
		Destroy (gameObject);
	}
}
