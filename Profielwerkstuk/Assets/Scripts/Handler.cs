using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handler : MonoBehaviour {

    public enum ControllerType {
        manual,
        automatic,
    }

    // Singleton
    private static Handler _instance;
    [HideInInspector]
    public static Handler Instance { get { return _instance; } }

    [Header("Genetic Algorithm settings")]
    public int generationSize; // The amount of cars spawned for each generation
    public ControllerType controllerType;

    [Header("Car settings")]
    public GameObject carPrefab; // Car prefab

    public float refreshTime; // The sensory update time of the cars
    public float moveSpeed; // The moveing speed of the cars
    public float steerSpeed; // The steering speed of the cars

    // Singleton design pattern
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        // Create the first 
        int seed = Random.Range(0, 10000);

        createGeneration();
    }

    public void createGeneration() {
        for (int i = 0; i < generationSize; i++) {
            createAgent();
        }
    }

    void createAgent() {
        // Create car with specified configuration and values
        GameObject car = GameObject.Instantiate(carPrefab) as GameObject;
        car.tag = "Car";
        car.transform.SetParent(transform);

        // Create car controller and initialize
        CarController controller = car.AddComponent<CarController>();

        NeuralNetwork carBrain = new NeuralNetwork(5, 3, 2, Random.Range(0, 999));
        controller.brain = carBrain;
        car.name += " | BrainID: #" + carBrain.GetHashCode();
    }
}
