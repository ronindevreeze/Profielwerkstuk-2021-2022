using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetwork {
    
    public Matrix weights_ih, weights_ho, bias_h, bias_o;
    // The configuration of the network, meaning the values of the biases and synapses

    public NeuralNetwork(int i, int h, int o) {
        // Create matrixes for the synapses
        weights_ih = new Matrix(h, i);
        weights_ho = new Matrix(o, h);

        // Create matrixes for the biases
        bias_h = new Matrix(h, 1);
        bias_o = new Matrix(o, 1);
    }

    public NeuralNetwork(NeuralNetwork newConfiguration) {
        // Create a new network from a existing template configuration
        weights_ih = newConfiguration.weights_ih;
        weights_ho = newConfiguration.weights_ho;

        bias_h = newConfiguration.bias_h;
        bias_o = newConfiguration.bias_o;
    }

    public List<float> predict(float[] inputs) {
        // Propagate forward to predict output based in inputs
        Matrix input = Matrix.fromArray(inputs);
        Matrix hidden = Matrix.multiply(weights_ih, input);

        hidden.add(bias_h);
        hidden.sigmoid();

        Matrix output = Matrix.multiply(weights_ho, hidden);
        output.add(bias_o);
        output.sigmoid();

        return output.toArray();
    }

    // public void train(float[] x, float[] y) {
    //     Matrix input = Matrix.fromArray(x);
    //     Matrix hidden = Matrix.multiply(configuration.weights_ih, input);

    //     hidden.add(configuration.bias_h);
    //     hidden.sigmoid();

    //     Matrix output = Matrix.multiply(configuration.weights_ho, hidden);
    //     output.add(configuration.bias_o);
    //     output.sigmoid();

    //     Matrix target = Matrix.fromArray(y);
    //     Matrix error = Matrix.subtract(target, output);
    //     Matrix gradient = output.dsigmoid();
    //     gradient.multiply(error);
    //     gradient.multiply(l_rate);

    //     Matrix hidden_T = Matrix.transpose(hidden);
    //     Matrix who_delta = Matrix.multiply(gradient, hidden_T);

    //     configuration.weights_ho.add(who_delta);
    //     configuration.bias_o.add(gradient);

    //     Matrix who_T = Matrix.transpose(configuration.weights_ho);
    //     Matrix hidden_errors = Matrix.multiply(who_T, error);

    //     Matrix h_gradient = hidden.dsigmoid();
    //     h_gradient.multiply(hidden_errors);
    //     h_gradient.multiply(l_rate);

    //     Matrix i_T = Matrix.transpose(input);
    //     Matrix wih_delta = Matrix.multiply(h_gradient, i_T);

    //     configuration.weights_ih.add(wih_delta);
    //     configuration.bias_h.add(h_gradient);
    // }

    // public void fit(float[,] x, float[,] y, int epochs) {
    //     for(int i = 0; i < epochs; i++) {
    //         int sampleN = (int)(Random.Range(0f, x.GetLength(1)));

    //         float[] inputRow = new float[2];
    //         float[] outputRow = new float[1];

    //         for (int j = 0; j < 2; j++) {
    //             inputRow[j] = x[sampleN, j];
    //         }

    //         outputRow[0] = y[sampleN, 0];

    //         this.train(inputRow, outputRow);
    //     }
    // }
}