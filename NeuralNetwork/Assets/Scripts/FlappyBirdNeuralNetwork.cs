using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FlappyBirdNeuralNetwork : MonoBehaviour {

	#region Singleton

	public static FlappyBirdNeuralNetwork instance;

	void Awake () {
		instance = this;
	}

	#endregion

	public enum PlayMode {
		SupervisedTraining,
		SupervisedTesting,
	};

	public PlayMode playMode;

	public NeuralNetwork nnSupervised;
	public NeuralNetwork nnReinforced;
	public NeuralNetworkSaveData data;

	GameObject player;
	GameController gameController;
	GameObject bottomBorder;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		bottomBorder = GameObject.FindGameObjectWithTag ("BottomBorder");

		if (playMode == PlayMode.SupervisedTesting || playMode == PlayMode.SupervisedTraining) {
			data = Load (true);
			if (data == null) {
				nnSupervised = new NeuralNetwork (3, 3, 1);
			} else {
				nnSupervised = new NeuralNetwork (data);
			}
		} /*else if (playMode == PlayMode.ReinforcementTraining || playMode == PlayMode.ReinforcementTesting) {
			data = Load (false);
			if (data == null) {
				nnReinforced = new NeuralNetwork (3, 3, 1);
			} else {
				nnReinforced = new NeuralNetwork (data);
			}
		}*/
	}

	void Update () {
		if (playMode == PlayMode.SupervisedTraining) {
			SupervisedTrain ();
		} else if (playMode == PlayMode.SupervisedTesting) {
			SupervisedTest ();
		}
	}

	void OnDisable () {
		if (playMode == PlayMode.SupervisedTraining) {
			Save (true);
		} /*else if (playMode == PlayMode.ReinforcementTraining) {
			Save (false);
		}*/
	}

	void SupervisedTrain () {
		if (!gameController.isGameOver) {
			float[] inputs = new float[3];

			float[] heights = GetPipeHeights ();
			float[] distances = GetPipeDistances ();

			inputs [0] = PlayerHeight ();
			inputs [1] = heights [0];
			inputs [2] = distances [0];

			float[] outputs = player.GetComponent<PlayerController> ().GetPlayerInput ();
			if (outputs [0] == 1f) {
				Debug.Log ("Training with 1");
			} else if (outputs [0] == 0f) {
				Debug.Log ("Training with 0");
			}

			for (int i = 0; i < 100; i++) {
				nnSupervised.Train (inputs, outputs);
			}
		}
	}

	void SupervisedTest () {
		if (!gameController.isGameOver) {
			float[] inputs = new float[3];

			float[] heights = GetPipeHeights ();
			float[] distances = GetPipeDistances ();

			inputs [0] = PlayerHeight ();
			inputs [1] = heights [0];
			inputs [2] = distances [0];

			float[] outputs = nnSupervised.PredictOutput (inputs);

			Debug.Log (outputs [0]);
			if (outputs [0] > .5f) {
				player.GetComponent<PlayerController> ().CustomJump ();
			}
		}
	}

	float PlayerHeight () {
		float distance = player.transform.position.y - bottomBorder.transform.position.y - 1f;

		return distance;
	}

	float[] GetPipeHeights () {
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

		distances[0] = pipes[0].position.x - player.transform.position.x;
		if (pipes [1] == null) {
			distances [1] = distances [0] + 3f;
		} else {
			distances [1] = pipes [1].position.x - player.transform.position.x;
		}

		return distances;
	}

	public void Save (bool isSupervised) {
		if (isSupervised) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/FlappyBirdsSupervised.txt");

			NeuralNetworkSaveData data2;

			if (nnSupervised != null) {
				data2 = new NeuralNetworkSaveData (nnSupervised);
				bf.Serialize (file, data2);
			}

			file.Close ();
		} else {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/FlappyBirdsReinforced.txt");

			NeuralNetworkSaveData data2;

			if (nnReinforced != null) {
				data2 = new NeuralNetworkSaveData (nnReinforced);
				bf.Serialize (file, data2);
			}

			file.Close ();
		}
	}

	public NeuralNetworkSaveData Load (bool isSupervised) {
		if (isSupervised) {
			if (File.Exists (Application.persistentDataPath + "/FlappyBirdsSupervised.txt")) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/FlappyBirdsSupervised.txt", FileMode.Open);

				NeuralNetworkSaveData data2 = (NeuralNetworkSaveData)bf.Deserialize (file);
				file.Close ();

				return data2;
			}
		} else {
			if (File.Exists (Application.persistentDataPath + "/FlappyBirdsReinforced.txt")) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/FlappyBirdsReinforced.txt", FileMode.Open);

				NeuralNetworkSaveData data2 = (NeuralNetworkSaveData)bf.Deserialize (file);
				file.Close ();

				return data2;
			}
		}

		return null;
	}
}