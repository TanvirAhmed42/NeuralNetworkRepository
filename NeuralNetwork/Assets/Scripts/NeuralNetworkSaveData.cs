using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetworkSaveData {

	public int inputNodes;
	public int hiddenNodes;
	public int outputNodes;

	public float[] weightsInputHidden;
	public float[] weightsHiddenOutput;

	public float[] hiddenBiases;
	public float[] outputBiases;

	public NeuralNetworkSaveData (NeuralNetwork nn) {
		inputNodes = nn.inputNodes;
		hiddenNodes = nn.hiddenNodes;
		outputNodes = nn.outputNodes;

		weightsInputHidden = Matrix.ToArrayFromMatrix (nn.weightsInputHidden);
		weightsHiddenOutput = Matrix.ToArrayFromMatrix (nn.weightsHiddenOutput);

		hiddenBiases = Matrix.ToArrayFromMatrix (nn.hiddenBiases);
		outputBiases = Matrix.ToArrayFromMatrix (nn.outputBiases);
	}
}
