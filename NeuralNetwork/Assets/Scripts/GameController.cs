using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	#region Singleton

	public static GameController instance;

	void Awake () {
		instance = this;
	}

	#endregion

	public delegate void PlayerScore ();
	public PlayerScore playerScore;

	public GameObject pipeUnitPrefab;
	public GameObject pipes;

	[HideInInspector]
	public bool isGameOver = false;

	public int score = 0;

	bool isRestarting = false;

	void Start () {
		playerScore += ScorePoints;
		GeneratePipe ();
	}

	void Update () {
		if (pipes.transform.childCount > 0) {
			GameObject pipeParent = pipes.transform.GetChild (pipes.transform.childCount - 1).gameObject;
			if (pipeParent.transform.GetChild (0).transform.position.x <= 4.8f && !isRestarting) {
				GeneratePipe ();
			}
		}

		if (isGameOver && SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("FlappyBird")) {
			Debug.Log (score);
		}
	}

	void GeneratePipe () {
		int pipeLength1 = Random.Range (1, 6);
		int pipeLength2 = 7 - pipeLength1;

		GameObject pipeUnit1 = Instantiate (pipeUnitPrefab, Vector2.zero, Quaternion.identity) as GameObject;
		GameObject pipeUnit2 = Instantiate (pipeUnitPrefab, Vector2.zero, Quaternion.identity) as GameObject;

		pipeUnit1.transform.localScale = new Vector3 (1f, pipeLength1, 1f);
		pipeUnit2.transform.localScale = new Vector3 (1f, pipeLength2, 1f);

		pipeUnit1.transform.position = new Vector3 (10f, (10f - pipeLength1) / 2f, 0f);
		pipeUnit2.transform.position = new Vector3 (10f, -(10f - pipeLength2) / 2f, 0f);

		GameObject pipe = new GameObject ("Pipe");
		pipeUnit1.transform.SetParent (pipe.transform);
		pipeUnit2.transform.SetParent (pipe.transform);

		pipe.transform.SetParent (pipes.transform);
	}

	public void RestartPipes () {
		isRestarting = true;
		for (int i = pipes.transform.childCount - 1; i >= 0; i--) {
			Destroy (pipes.transform.GetChild (i).gameObject);
		}

		isRestarting = false;
		GeneratePipe ();
	}

	void ScorePoints () {
		if (!isGameOver) {
			score++;
		}
	}

	public Transform[] GetFirstSecondPipes () {
		Transform[] nextPipes = new Transform[2];

		if (pipes.transform.childCount == 0) {
			return nextPipes;
		}

		nextPipes[0] = pipes.transform.GetChild (0).transform.GetChild (1).transform;

		if (pipes.transform.childCount > 1) {
			nextPipes[1] = pipes.transform.GetChild (1).transform.GetChild (1).transform;
		}

		if (nextPipes [0].position.x < -8.05f) {
			nextPipes [0] = pipes.transform.GetChild (1).transform.GetChild (1).transform;
			nextPipes [1] = pipes.transform.GetChild (2).transform.GetChild (1).transform;
		}

		return nextPipes;
	}
}
