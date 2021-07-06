using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Matrix {
    [SerializeField]
    public float[][] data;
    public int rows, cols;

    public Matrix(int _rows,int _cols) {
        data = new float[_rows][];
        for (int i=0; i<data.Length; i++) {
            data[i] = new float[_cols];
        }

        this.rows = _rows;
        this.cols = _cols;

        for(int i = 0; i < rows; i++) {
            for(int j = 0;j < cols; j++) {
                data[i][j] = Random.Range(-1f, 1f);
            }
        }
    }

    public Matrix add(Matrix m) {
        if(cols != m.cols || rows != m.rows) {
            Debug.Log("Matrix shape doesn't match");
            return null;
        }

        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                this.data[i][j] += m.data[i][j];
            }
        }

        return this;
    }

    public static Matrix subtract(Matrix a, Matrix b) {
        Matrix temp = new Matrix(a.rows, a.cols);

        for(int i = 0; i < a.rows; i++) {
            for(int j = 0; j < a.cols; j++) {
                temp.data[i][j] = a.data[i][j] - b.data[i][j];
            }
        }

        return temp;
    }

    public static Matrix transpose(Matrix a) {
        Matrix temp = new Matrix(a.cols, a.rows);

        for(int i = 0; i < a.rows; i++) {
            for(int j = 0; j < a.cols; j++) {
                temp.data[j][i] = a.data[i][j];
            }
        }

        return temp;
    }

    public string print(bool print) {
        string message = "";
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                message += data[i][j] + " | ";
            }
            
            message += "\n"; 
        }

        if(print) { Debug.Log(message); }
        return message;
    }

    public static Matrix multiply(Matrix a, Matrix b) {
        Matrix temp = new Matrix(a.rows, b.cols);

        for(int i = 0; i < temp.rows; i++) {
            for(int j = 0; j < temp.cols; j++) {
                float sum = 0;

                for(int k=0;k<a.cols;k++) {
                    sum += a.data[i][k] * b.data[k][j];
                }

                temp.data[i][j] = sum;
            }
        }
        return temp;
    }

    public void multiply(Matrix a) {
        for(int i = 0; i < a.rows; i++) {
            for(int j = 0; j < a.cols; j++) {
                this.data[i][j] *= a.data[i][j];
            }
        }
    }

    public Matrix multiply(float a) {
        //Debug.Log("Before: " + this.data[0,0]);
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                this.data[i][j] *= a;
            }
        }
        //Debug.Log("After: " + this.data[0,0]);

        return this;
    }

    public void sigmoid() {
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                this.data[i][j] = 1 / (1 + Mathf.Exp(-this.data[i][j]));
            }
        }
    }

    public Matrix dsigmoid() {
        Matrix temp = new Matrix(rows, cols);

        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                temp.data[i][j] = this.data[i][j] * (1 - this.data[i][j]);
            }
        }

        return temp;
    }

    public static Matrix fromArray(float[] x) {
        Matrix temp = new Matrix(x.Length, 1);

        for(int i = 0; i < x.Length; i++) {
            temp.data[i][0] = x[i];
        }

        return temp;
    }

    public List<float> toArray() {
        List<float> temp = new List<float>();

        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                temp.Add(data[i][j]);
            }
        }

        return temp;
    }
}
