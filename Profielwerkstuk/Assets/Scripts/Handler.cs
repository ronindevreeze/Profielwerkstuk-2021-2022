using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handler : MonoBehaviour {

    [Header("Genetic Algorithm settings")]
    public int generationSize; // The amount of cars spawned for each generation
    public int selectionSize; // The amount of cars the get to reproduce

    private List<GameObject> finishedAgents = new List<GameObject>(); // The list of car that finished

    [Header("Car settings")]
    public GameObject carPrefab; // Car prefab
    public float refreshTime; // The sensory update time of the cars
    public float moveSpeed; // The moveing speed of the cars
    public float steerSpeed; // The steering speed of the cars

    private NeuralNetwork currentNeuralNetwork;

    void Start() {
        // Create the first 
        int seed = Random.Range(0, 10000);
        currentNeuralNetwork = new NeuralNetwork(5, 8, 2, seed);

        createGeneration(false);
    }

    NeuralNetwork getBestConfigurationAndRemoveCurrent() { // Get the best car of the generation and return the configuration
        // Add all cars the list
        finishedAgents.AddRange(GameObject.FindGameObjectsWithTag("Car"));
        NeuralNetwork config = new NeuralNetwork();

        finishedAgents.Sort(SortByFitness);
        config = finishedAgents[finishedAgents.Count - 1].GetComponent<CarController>().brain;

        // Remove all the cars
        for(int i = 0; i < finishedAgents.Count; i++) {
            Destroy(finishedAgents[i]);   
        }
        finishedAgents.Clear();

        return config;
    }

    public void createGeneration(bool reproduce) {
        if(reproduce) { // If this generation should reproduct
            // Get the best cars of the generation
            currentNeuralNetwork = getBestConfigurationAndRemoveCurrent();
        }

        for (int i = 0; i < generationSize; i++) {
            // mutate local configuration
            createAgent(currentNeuralNetwork);
        }
    }

    static int SortByFitness(GameObject g1, GameObject g2) {
        // This function is used for sorting the car-list
        return g1.GetComponent<CarController>().fitness.CompareTo(g2.GetComponent<CarController>().fitness);
    }

    void createAgent(NeuralNetwork configuration) {
        // Create car with specified configuration and values
        GameObject car = GameObject.Instantiate(carPrefab);
        car.tag = "Car";
        car.transform.SetParent(transform);

        // Create car controller and initialize
        CarController controller = car.GetComponent<CarController>();
        controller.handler = this;
        controller.refreshTime = refreshTime;
        controller.moveSpeed = moveSpeed;
        controller.steerSpeed = steerSpeed;
        controller.brain = new NeuralNetwork(configuration);
    }
}
