using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CarController : MonoBehaviour {

    private List<GameObject> sensors = new List<GameObject>();

    public Handler handler;
    public float[] sensorInputs;
    public float learningRate = 20f;

    [SerializeField]
    public NeuralNetwork brain;

    public float[] output = new float[2];
    public float refreshTime;
    public float moveSpeed;
    public float steerSpeed;

    public float fitness = 0f;
    private Vector3 lastPosition;

    void Start() {
        int childeren = transform.childCount;
        for(int i = 0; i < childeren; i++) {
            GameObject sensorChild = transform.GetChild(i).gameObject;
            if(sensorChild.tag == "Sensor") {
                sensors.Add(sensorChild);
            }
        }

        // Mutate the assigned neural network
        Matrix temp = new Matrix(brain.weights_ho.rows, brain.weights_ho.cols).multiply(learningRate);
        brain.weights_ho = brain.weights_ho.add(temp);
        brain.weights_ih = brain.weights_ih.add(new Matrix(brain.weights_ih.rows, brain.weights_ih.cols).multiply(learningRate));
        brain.bias_h = brain.bias_h.add(new Matrix(brain.bias_h.rows, brain.bias_h.cols).multiply(learningRate));
        brain.bias_o = brain.bias_o.add(new Matrix(brain.bias_o.rows, brain.bias_o.cols).multiply(learningRate));

        // Something wrong with the add function

        StartCoroutine("getPrediction");
    }

    void Update() {
        // TODO: move according to output float array
        transform.Translate(Vector3.forward * moveSpeed * (output[0] + 1) * Time.deltaTime);
        transform.Rotate(Vector3.up * steerSpeed * output[0] * ((output[1] - 0.5f) * 2) * Time.deltaTime);

        // Update the fitness
        fitness += Vector3.Distance(transform.position, lastPosition) * Time.deltaTime;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other) {
        die();
        moveSpeed = steerSpeed = 0;
    }

    IEnumerator getPrediction() {
        for(;;) {
            float[] inputs = getSensorInputs();

            output = brain.predict(inputs).ToArray();



            yield return new WaitForSeconds(refreshTime);
        }
    }

    float[] getSensorInputs() {
        sensorInputs = new float[sensors.Count];

        for(int i = 0; i < sensorInputs.Length; i++) {
            RaycastHit hit;
            if(Physics.Raycast(sensors[i].transform.position, sensors[i].transform.forward, out hit, 5, ~(1 << 8))) {
                sensorInputs[i] = hit.distance;
            } else {
                sensorInputs[i] = 5;
            }
        }

        return sensorInputs;
    }

    void die() {
        transform.GetChild(0).gameObject.SetActive(false);
        moveSpeed = steerSpeed = 0;
    }

    void OnDrawGizmos() {
        for (int i = 0; i < sensors.Count; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensors[i].transform.position, sensors[i].transform.position + (sensors[i].transform.forward * sensorInputs[i]));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * output[0]);
            Gizmos.DrawLine(transform.position, transform.position + transform.right * (output[1] - 0.5f) * 2);
        }
    }
}
