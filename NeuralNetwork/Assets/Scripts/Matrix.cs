using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix {

	public delegate float MapFunction (float f);
	public delegate float TwoInputMapFunction (float a, float b);

	public int rows;
	public int columns;
	public float[,] values;

	public Matrix (int _rows, int _columns) {
		rows = _rows;
		columns = _columns;

		values = new float[rows, columns];

		Randomize ();
	}

	public void Randomize () {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] = Random.Range (-1f, 1f);
			}
		}
	}

	public void Mutation (float delta, float mutationRate) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				float randomMutation = Random.Range (0f, 1f);
				if (randomMutation < mutationRate) {
					values [i, j] += delta;
				}
			}
		}
	}

	public void Map (MapFunction mapFunction) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] = mapFunction (values [i, j]);
			}
		}
	}

	public static Matrix Map (Matrix a, MapFunction mapFunction) {
		Matrix b = new Matrix (a.rows, a.columns);

		for (int i = 0; i < b.rows; i++) {
			for (int j = 0; j < b.columns; j++) {
				b.values [i, j] = mapFunction (a.values [i, j]);
			}
		}

		return b;
	}

	public static Matrix Map (Matrix a, Matrix b, TwoInputMapFunction mapFunction) {
		Matrix c = new Matrix (a.rows, a.columns);

		for (int i = 0; i < c.rows; i++) {
			for (int j = 0; j < c.columns; j++) {
				c.values [i, j] = mapFunction (a.values [i, j], b.values [i, j]);
			}
		}

		return c;
	}

	public void FixValues (Matrix a) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] = a.values [i, j];
			}
		}
	}

	public void Add (float n) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] += n;
			}
		}
	}

	public static Matrix Add (Matrix a, Matrix b) {
		if (a.rows != b.rows || a.columns != b.columns) {
			Debug.LogError ("Dimension Of Matrices Don't Match For Adding!!!");
			return null;
		}

		Matrix c = new Matrix (a.rows, a.columns);

		for (int i = 0; i < a.rows; i++) {
			for (int j = 0; j < a.columns; j++) {
				c.values [i, j] = a.values [i, j] + b.values [i, j];
			}
		}

		return c;
	}

	public void Subtract (float n) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] -= n;
			}
		}
	}

	public static Matrix Subtract (Matrix a, Matrix b) {
		if (a.rows != b.rows || a.columns != b.columns) {
			Debug.LogError ("Dimensions Of Matrices Don't Match For Subtracting!!!");
			return null;
		}

		Matrix c = new Matrix (a.rows, a.columns);

		for (int i = 0; i < c.rows; i++) {
			for (int j = 0; j < c.columns; j++) {
				c.values [i, j] = a.values [i, j] - b.values [i, j];
			}
		}

		return c;
	}

	public void Multiply (float n) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] *= n;
			}
		}
	}

	public void Multiply (Matrix a) {
		if (rows != a.rows || columns != a.columns) {
			Debug.LogError ("Dimensions Of Matrices Don't Match For Multiplying!!!");
			return;
		}

		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				values [i, j] *= a.values [i, j];
			}
		}
	}

	public static Matrix Multiply (Matrix a, Matrix b) {
		if (a.columns != b.rows) {
			Debug.LogError ("Dimensions Of Matrices Don't Match For Multiplying!!!");
			return null;
		}

		Matrix c = new Matrix (a.rows, b.columns);

		for (int i = 0; i < c.rows; i++) {
			for (int j = 0; j < c.columns; j++) {
				float sum = 0;
				for (int k = 0; k < a.columns; k++) {
					sum += a.values [i, k] * b.values [k, j];
				}
				c.values [i, j] = sum;
			}
		}

		return c;
	}

	public static Matrix Transpose (Matrix a) {
		Matrix b = new Matrix (a.columns, a.rows);

		for (int i = 0; i < a.rows; i++) {
			for (int j = 0; j < a.columns; j++) {
				b.values [j, i] = a.values [i, j];
			}
		}

		return b;
	}

	public static Matrix FromArrayToMatrix (float[] matrixArray) {
		Matrix a = new Matrix (matrixArray.Length, 1);

		for (int i = 0; i < matrixArray.Length; i++) {
			a.values [i, 0] = matrixArray [i];
		}

		return a;
	}

	public static Matrix FromArrayToMatrix (float[] matrixArray, int _rows, int _columns) {
		Matrix a = new Matrix (_rows, _columns);

		for (int i = 0; i < a.rows; i++) {
			for (int j = 0; j < a.columns; j++) {
				a.values [i, j] = matrixArray [i * _columns + j];
			}
		}

		return a;
	}

	public static float[] ToArrayFromMatrix (Matrix a) {
		float[] matrixArray = new float[a.rows * a.columns];

		int k = 0;
		for (int i = 0; i < a.rows; i++) {
			for (int j = 0; j < a.columns; j++) {
				matrixArray [k++] = a.values [i, j];
			}
		}

		return matrixArray;
	}

	public static void Print (Matrix a) {
		if (a == null) {
			return;
		}

		string row;

		for (int i = 0; i < a.rows; i++) {
			row = "";
			for (int j = 0; j < a.columns; j++) {
				row += " (" + i + ", " + j + ") " + a.values [i, j];
			}
			Debug.Log (row);
		}
	}
}
