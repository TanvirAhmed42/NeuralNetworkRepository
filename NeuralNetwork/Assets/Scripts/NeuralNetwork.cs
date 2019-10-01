using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork {

	public int inputNodes;
	public int hiddenNodes;
	public int outputNodes;

	public Matrix weightsInputHidden;
	public Matrix weightsHiddenOutput;

	public Matrix hiddenBiases;
	public Matrix outputBiases;

	public float learningRate = .1f;

	public NeuralNetwork (int _inputNodes, int _hiddenNodes, int _outputNodes) {
		inputNodes = _inputNodes;
		hiddenNodes = _hiddenNodes;
		outputNodes = _outputNodes;

		weightsInputHidden = new Matrix (hiddenNodes, inputNodes);
		weightsHiddenOutput = new Matrix (outputNodes, hiddenNodes);

		hiddenBiases = new Matrix (hiddenNodes, 1);
		outputBiases = new Matrix (outputNodes, 1);
	}

	public NeuralNetwork (NeuralNetworkSaveData data) {
		inputNodes = data.inputNodes;
		hiddenNodes = data.hiddenNodes;
		outputNodes = data.outputNodes;

		weightsInputHidden = Matrix.FromArrayToMatrix (data.weightsInputHidden, hiddenNodes, inputNodes);
		weightsHiddenOutput = Matrix.FromArrayToMatrix (data.weightsHiddenOutput, outputNodes, hiddenNodes);

		hiddenBiases = Matrix.FromArrayToMatrix (data.hiddenBiases);
		outputBiases = Matrix.FromArrayToMatrix (data.outputBiases);
	}

	public void FixValues (NeuralNetwork a) {
		if (inputNodes != a.inputNodes || hiddenNodes != a.hiddenNodes || outputNodes != a.outputNodes) {
			Debug.LogError ("Number Of Neurons Don't Match!!!");
			return;
		}

		weightsInputHidden.FixValues (a.weightsInputHidden);
		weightsHiddenOutput.FixValues (a.weightsHiddenOutput);

		hiddenBiases.FixValues (a.hiddenBiases);
		outputBiases.FixValues (a.outputBiases);
	}

	public float[] PredictOutput (float[] inputs) {
		Matrix inputMatrix = Matrix.FromArrayToMatrix (inputs);

		Matrix hiddenMatrix = Matrix.Multiply (weightsInputHidden, inputMatrix);
		hiddenMatrix = Matrix.Add (hiddenMatrix, hiddenBiases);
		hiddenMatrix.Map (Sigmoid);

		Matrix outputMatrix = Matrix.Multiply (weightsHiddenOutput, hiddenMatrix);
		outputMatrix = Matrix.Add (outputMatrix, outputBiases);
		outputMatrix.Map (Sigmoid);

		float[] outputs = Matrix.ToArrayFromMatrix (outputMatrix);

		return outputs;
	}

	public void Train (float[] inputs, float[] targets) {
		Matrix inputMatrix = Matrix.FromArrayToMatrix (inputs);

		Matrix hiddenMatrix = Matrix.Multiply (weightsInputHidden, inputMatrix);
		hiddenMatrix = Matrix.Add (hiddenMatrix, hiddenBiases);
		hiddenMatrix.Map (Sigmoid);

		Matrix outputMatrix = Matrix.Multiply (weightsHiddenOutput, hiddenMatrix);
		outputMatrix = Matrix.Add (outputMatrix, outputBiases);
		outputMatrix.Map (Sigmoid);

		Matrix targetMatrix = Matrix.FromArrayToMatrix (targets);

		Matrix outputErrorMatrix = Matrix.Subtract (targetMatrix, outputMatrix);

		Matrix outputGradientMatrix = Matrix.Map (outputMatrix, Gradient);
		outputGradientMatrix.Multiply (outputErrorMatrix);
		outputGradientMatrix.Multiply (learningRate);

		Matrix hiddenMatrixTransposed = Matrix.Transpose (hiddenMatrix);
		Matrix weightsHiddenOutputDelta = Matrix.Multiply (outputGradientMatrix, hiddenMatrixTransposed);

		weightsHiddenOutput = Matrix.Add (weightsHiddenOutput, weightsHiddenOutputDelta);
		outputBiases = Matrix.Add (outputBiases, outputGradientMatrix);

		Matrix weightsHiddenOutputTransposed = Matrix.Transpose (weightsHiddenOutput);
		Matrix hiddenErrorMatrix = Matrix.Multiply (weightsHiddenOutputTransposed, outputErrorMatrix);

		Matrix hiddenGradientMatrix = Matrix.Map (hiddenMatrix, Gradient);
		hiddenGradientMatrix.Multiply (hiddenErrorMatrix);
		hiddenGradientMatrix.Multiply (learningRate);

		Matrix inputMatrixTransposed = Matrix.Transpose (inputMatrix);
		Matrix weightsInputHiddenDelta = Matrix.Multiply (hiddenGradientMatrix, inputMatrixTransposed);

		weightsInputHidden = Matrix.Add (weightsInputHidden, weightsInputHiddenDelta);
		hiddenBiases = Matrix.Add (hiddenBiases, hiddenGradientMatrix);
	}

	public static NeuralNetwork CrossOver (NeuralNetwork a, NeuralNetwork b) {
		NeuralNetwork c = new NeuralNetwork (a.inputNodes, a.hiddenNodes, a.outputNodes);

		Matrix inputHidden = Matrix.Map (a.weightsInputHidden, b.weightsInputHidden, Mean);
		Matrix outputHidden = Matrix.Map (a.weightsHiddenOutput, b.weightsHiddenOutput, Mean);
		Matrix biasHidden = Matrix.Map (a.hiddenBiases, b.hiddenBiases, Mean);
		Matrix biasOutput = Matrix.Map (a.outputBiases, b.outputBiases, Mean);

		c.weightsInputHidden.FixValues (inputHidden);
		c.weightsHiddenOutput.FixValues (outputHidden);

		c.hiddenBiases.FixValues (biasHidden);
		c.outputBiases.FixValues (biasOutput);

		return c;
	}

	public void Mutation (float mutationRate) {
		int randomMutation = Random.Range (0, 2);
		if (randomMutation == 0) {
			weightsInputHidden.Mutation (-.1f, mutationRate);
		} else if (randomMutation == 1) {
			weightsInputHidden.Mutation (.1f, mutationRate);
		}

		randomMutation = Random.Range (0, 2);
		if (randomMutation == 0) {
			weightsHiddenOutput.Mutation (-.1f, mutationRate);
		} else if (randomMutation == 1) {
			weightsHiddenOutput.Mutation (.1f, mutationRate);
		}

		randomMutation = Random.Range (0, 2);
		if (randomMutation == 0) {
			hiddenBiases.Mutation (-.1f, mutationRate);
		} else if (randomMutation == 1) {
			hiddenBiases.Mutation (.1f, mutationRate);
		}

		randomMutation = Random.Range (0, 2);
		if (randomMutation == 0) {
			outputBiases.Mutation (-.1f, mutationRate);
		} else if (randomMutation == 1) {
			outputBiases.Mutation (.1f, mutationRate);
		}
	}

	float Sigmoid (float n) {
		return 1 / (1 + Mathf.Exp (-n));
	}

	float Gradient (float f) {
		return f * (1 - f);
	}

	static float Mean (float a, float b) {
		return (a + b) / 2f;
	}

	static float SquareRoot (float a, float b) {
		return Mathf.Sqrt (a * b);
	}

	static float SquaredMean (float a, float b) {
		return Mathf.Sqrt ((a * a + b * b) / 2f);
	}
}
