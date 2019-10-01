using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	public NeuralNetwork nn;
	[HideInInspector]
	public float[] inputs;
	[HideInInspector]
	public float[] outputs;

	public InputField xInput;
	public InputField yInput;
	public Text outputText;

	int epoch = 0;

	void Start () {
		nn = new NeuralNetwork (2, 4, 1);
		inputs = new float[2];
		outputs = new float[1];
	}

	public void TrainXOR () {
		for (int i = 0; i < 10000; i++) {
			int randomIndex = Random.Range (1, 5);
			if (randomIndex == 1) {
				inputs [0] = 0;
				inputs [1] = 0;
				outputs [0] = 0;
			} else if (randomIndex == 2) {
				inputs [0] = 0;
				inputs [1] = 1;
				outputs [0] = 1;
			} else if (randomIndex == 3) {
				inputs [0] = 1;
				inputs [1] = 0;
				outputs [0] = 1;
			} else if (randomIndex == 4) {
				inputs [0] = 1;
				inputs [1] = 1;
				outputs [0] = 0;
			}
			nn.Train (inputs, outputs);
		}
		epoch++;
		Debug.Log ("Epoch: " + epoch + " completed.");
	}

	public void TestInput () {
		inputs [0] = float.Parse (xInput.text);
		inputs [1] = float.Parse (yInput.text);

		outputs = nn.PredictOutput (inputs);

		outputText.text = outputs [0].ToString ();
	}
}
