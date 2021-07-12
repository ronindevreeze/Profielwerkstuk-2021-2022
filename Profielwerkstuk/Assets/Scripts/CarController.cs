using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CarController : MonoBehaviour {

    [Header("Simulation Settings")]
    public bool collectData;

    [Header("Controller settings")]
    public float refreshTime; // Time between fetching the data from the neural network
    public float moveSpeed;
    public float steerSpeed;

    [Header("Algoithm settings")]
    public float[] sensorInputs;
    public float fitness = 0f;
    private Vector3 lastPosition;

    [SerializeField, HideInInspector]
    public NeuralNetwork brain;

    public float[] output = new float[2];
    private List<GameObject> sensors = new List<GameObject>();
    private string personalLog = "";
    public string config;

    void Start() {
        Debug.Log("==== New Car ====" + 
        "\n\n---- Weights_HO ----\n\n" + 
        brain.weights_ho.print(false) + 
        "\n\n---- Weights_IH ----\n\n" 
        + brain.weights_ih.print(false) + 
        "\n\n---- Bias_H ----\n\n" + 
        brain.bias_h.print(false) + 
        "\n\n---- Bias_O ----\n\n" + 
        brain.bias_o.print(false));

        // Add all the sensors to the list
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).tag == "Sensor") {
                sensors.Add(transform.GetChild(i).gameObject);
            }
        }

        collectData = Handler.Instance.collectData;
        refreshTime = Handler.Instance.refreshTime;
        moveSpeed = Handler.Instance.moveSpeed;
        steerSpeed = Handler.Instance.steerSpeed;

        // Start getting the predictions
        StartCoroutine("getPrediction");

        // collect data is neccecary
        if(collectData) {
            StartCoroutine("writeData");
        }
    }

    void Update() {
        // Move according to output float array
        transform.Translate(Vector3.forward * moveSpeed * output[0] * Time.deltaTime);
        transform.Rotate(Vector3.up * steerSpeed * output[0] * ((output[1] - 0.5f) * 2) * Time.deltaTime);

        // Update the fitness
        fitness += Vector3.Distance(transform.position, lastPosition) * Time.deltaTime;
        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other) {
        Die();
    }

    IEnumerator getPrediction() {
        for(;;) {
            float[] inputs = GetSensorInputs();
            output = brain.predict(inputs).ToArray();

            yield return new WaitForSeconds(refreshTime);
        }
    }

    IEnumerator writeData() {
        for(;;) {
            personalLog += name + " | "; // write name

            for (int i = 0; i < sensors.Count; i++) { // write sensor data
                personalLog += sensorInputs[i] + ", ";
            }

            personalLog += " | " + output[0] + ", "; // write vertical axis
            personalLog += output[1]; // write horizontal axis 

            Handler.Instance.AddLog(personalLog);

            personalLog = "";
            
            yield return new WaitForSeconds(refreshTime);
        }
    }

    float[] GetSensorInputs() {
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

    void Die() {
        transform.GetChild(0).gameObject.SetActive(false);
        moveSpeed = steerSpeed = 0;
        Debug.Log(name + " died with a fitness of " + fitness);
        name += " | Fitness: " + fitness;
        Handler.Instance.agents.Add(this.gameObject);
    }

    void OnDrawGizmos() {
        for (int i = 0; i < sensors.Count; i++) {
            // Draw sensors
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensors[i].transform.position, sensors[i].transform.position + (sensors[i].transform.forward * sensorInputs[i]));
        }
        
        // Draw output vector of car
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * output[0]);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * (output[1] - 0.5f) * 2);
    }
}
