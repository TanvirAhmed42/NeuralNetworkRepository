using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Neuroevolution : MonoBehaviour {

	#region Singleton

	public static Neuroevolution instance;

	void Awake () {
		instance = this;
	}

	#endregion
	
	public int populationSize;
	[Range(0f, 1f)]
	public float mutationRate = .01f;

	public List<Player> playerPopulation = new List<Player> ();
	public List<NeuralNetwork> backupPopulation = new List<NeuralNetwork> ();
	public List<float> fitnessValues = new List<float> ();
	public List<float> scaledFitness = new List<float> ();
	public List<float> cumulativeFitness = new List<float> ();

	public GameObject playerPrefab;
	public Vector3 defaultPlayerTransform;

	public Text fitnessText;
	public Text generationText;

	int generationCount = 1;
	float fitness = 0f;
	float maxFitness = 0f;

	void Start () {
		for (int i = 0; i < populationSize; i++) {
			playerPopulation.Add (Instantiate (playerPrefab, defaultPlayerTransform, Quaternion.identity).gameObject.GetComponent<Player> ());
		}

		fitnessText.text = "Max Fitness: " + maxFitness;
		generationText.text = "Generation: " + generationCount;
	}

	void Update () {
		if (playerPopulation.Count == 0 && backupPopulation.Count > 0) {
			NextGeneration ();
		}
	}

	void NextGeneration () {
		generationCount++;
		generationText.text = "Generation: " + generationCount;
		ScaleFitness ();
		NeuralNetwork fitNeuralNetwork = GetFitChild ();
		for (int i = 0; i < populationSize; i++) {
			playerPopulation.Add (Instantiate (playerPrefab, defaultPlayerTransform, Quaternion.identity).gameObject.GetComponent<Player> ());
			playerPopulation [i].nn = new NeuralNetwork (3, 3, 2);
			playerPopulation [i].nn.FixValues (fitNeuralNetwork);
		}
		fitness = MaxFitness ();
		if (fitness > maxFitness) {
			maxFitness = fitness;
		}
		fitnessText.text = "Max Fitness: " + maxFitness;
		fitnessValues.Clear ();
		backupPopulation.Clear ();
		scaledFitness.Clear ();
		cumulativeFitness.Clear ();
		GameController.instance.RestartPipes ();
	}

	float MaxFitness () {
		float max = 0f;

		for (int i = 0; i < fitnessValues.Count; i++) {
			if (fitnessValues [i] > max) {
				max = fitnessValues [i];
			}
		}

		return max;
	}

	NeuralNetwork GetFitChild () {
		//CrossOver
		/*NeuralNetwork parentA = PickParent ();
		NeuralNetwork parentB = PickParent ();

		NeuralNetwork child = NeuralNetwork.CrossOver (parentA, parentB);*/

		//Ignoring CrossOver
		float fitnessToPick = float.MinValue;
		int fitnessIndex = 0;

		for (int i = 0; i < fitnessValues.Count; i++) {
			if (fitnessValues [i] > fitnessToPick) {
				fitnessToPick = fitnessValues [i];
				fitnessIndex = i;
			}
		}

		NeuralNetwork child = new NeuralNetwork (3, 3, 2);
		child.FixValues (backupPopulation [fitnessIndex]);

		child.Mutation (mutationRate);

		return child;
	}

	float TotalFitness () {
		float sum = 0f;

		for (int i = 0; i < fitnessValues.Count; i++) {
			sum += fitnessValues [i];
		}

		return sum;
	}

	void ScaleFitness () {
		float totalFitness = TotalFitness ();

		for (int i = 0; i < fitnessValues.Count; i++) {
			scaledFitness.Add ((fitnessValues [i] / totalFitness) * 100f);
		}

		float sum = 0f;

		for (int i = 0; i < scaledFitness.Count; i++) {
			cumulativeFitness.Add (scaledFitness [i] + sum);
			sum = scaledFitness [i] + sum;
		}
	}

	NeuralNetwork PickParent () {
		float fitNeuralNetworkIndex = Random.Range (1f, 100f);

		if (fitNeuralNetworkIndex >= 1 && fitNeuralNetworkIndex <= cumulativeFitness [0]) {
			return backupPopulation [0];
		}

		for (int i = 1; i < cumulativeFitness.Count - 1; i++) {
			if (fitNeuralNetworkIndex > cumulativeFitness [i] && fitNeuralNetworkIndex <= cumulativeFitness [i + 1]) {
				return backupPopulation [i];
			}
		}

		return backupPopulation [backupPopulation.Count - 1];
	}
}
