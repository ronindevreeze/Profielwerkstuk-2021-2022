using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class Handler : MonoBehaviour {

    public List<GameObject> agents = new List<GameObject>();
    public InputField SizeInput;
    public InputField TopInput;
    public InputField MutationInput;
    private System.Random random;

    // Singleton
    private static Handler _instance;
    [HideInInspector]
    public static Handler Instance { get { return _instance; } }

    [Header("Genetic Algorithm settings")]
    public int generationSize; // The amount of cars spawned for each generation

    [Header("Car settings")]
    public GameObject carPrefab; // Car prefab

    public float refreshTime; // The sensory update time of the cars
    public float moveSpeed; // The moving speed of the cars
    public float steerSpeed; // The steering speed of the cars
    public bool collectData;

    private string logs;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        random = new System.Random();

        // Create the first 
        int seed = random.Next(0, 10000);
    }

    public void createGeneration() {
        int size = int.Parse(SizeInput.text);
        int top = int.Parse(TopInput.text);

        //NeuralNetwork bestNet = getAverageNeuralNet(top);
        NeuralNetwork[] bestNets = getTopCars(top);

        // Clear and remove all agents from list
        foreach(GameObject g in agents) {
            Destroy(g);
        }
        agents.Clear();

        for (int i = 0; i < size; i++) {
            createAgent(bestNets[i % top]);
        }
        
    }

    public void createAgent(NeuralNetwork brain) {
        // Create car with specified configuration and values
        GameObject car = GameObject.Instantiate(carPrefab) as GameObject;
        car.tag = "Car";
        car.transform.SetParent(transform);
        car.transform.position = new Vector3(random.Next(-50, 50), 0, random.Next(-50, 50));

        // Create car controller and initialize
        CarController controller = car.AddComponent<CarController>();
        controller.brain = brain;
        car.name += " | BrainID: #" + brain.GetHashCode();
    }

    public NeuralNetwork[] getTopCars(int top) {
        Debug.Log("Getting top " + top + " cars");

        NeuralNetwork[] list = new NeuralNetwork[top];
        agents.Sort(SortByFitness);

        if(agents.Count < top) {
            for(int i = 0; i < top; i++) {
                list[i] = new NeuralNetwork(5, 3, 2);
            }
        } else {
            for(int i = 0; i < top; i++) {
                list[i] = agents[i].GetComponent<CarController>().brain;
            }
        }

        return list;
    }

    public static int SortByFitness(GameObject p1, GameObject p2) {
        return p2.GetComponent<CarController>().fitness.CompareTo(p1.GetComponent<CarController>().fitness);
    }

    public NeuralNetwork getAverageNeuralNet(int size) {
        if(agents.Count < size) {
            return new NeuralNetwork(5, 3, 2);
        } else {
            NeuralNetwork[] list = getTopCars(size); // Get the best scoring cars according to their fitness
            NeuralNetwork average = new NeuralNetwork(5, 3, 2);

            // Clear the average neural net to contain only zero's
            average.weights_ho.multiply(0f);
            average.weights_ih.multiply(0f);
            average.bias_h.multiply(0f);
            average.bias_o.multiply(0f);
            
            // Add the neural net matrices of all the individual best scoring cars
            for(int i = 0; i < size; i++) {
                average.weights_ho.add(list[i].weights_ho);
                average.weights_ih.add(list[i].weights_ih);
                average.bias_h.add(list[i].bias_h);
                average.bias_o.add(list[i].bias_o);
            }

            // Divide by 1 / size to get the average
            average.weights_ho.multiply(1f / size);
            average.weights_ih.multiply(1f / size);
            average.bias_h.multiply(1f / size);
            average.bias_o.multiply(1f / size);

            return average;
        }        
    }

    public void AddLog(string personalLog) {
        logs += personalLog;
        logs += "\n";
        Debug.Log("Added");
    }

    void OnDestroy() {
        if(collectData) {
            writeStringToFile(logs);
        }
    }

    void writeStringToFile(string content) {
        string path = "Assets/Logs/Log " + System.DateTime.Now.Ticks + ".txt";

        File.WriteAllText(path, content);

        AssetDatabase.ImportAsset(path); 
    }
}
