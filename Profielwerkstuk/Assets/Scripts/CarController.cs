using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CarController : MonoBehaviour {

    public enum ControllerType {
        manual,
        automatic,
    }

    [Header("Simulation Settings")]
    public bool collectData;
    public ControllerType controllerType; // The type of input the car is controlled by

    [Header("Controller settings")]
    public float refreshTime; // Time between fetching the data from the neural network
    public float moveSpeed;
    public float steerSpeed;

    [Header("Algoithm settings")]
    public float[] sensorInputs;
    public float fitness = 0f;
    private Vector3 lastPosition;

    [SerializeField]
    private NeuralNetwork brain;
    private float[] output = new float[2];
    private List<GameObject> sensors = new List<GameObject>();

    public CarController(float test) {
        Debug.Log("CarController was created with argument: " + test);
    }

    void Start() {
        foreach(GameObject c in transform) {
            if(c.tag == "Sensor") {
                sensors.Add(c);
            }
        }

        StartCoroutine("getPrediction");

        if(collectData) {
            StartCoroutine("writeData");
        }
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

    IEnumerator writeData() {
        for(;;) {
            // Write to file
            
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
