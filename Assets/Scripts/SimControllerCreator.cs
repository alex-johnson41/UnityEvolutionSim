using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class SimControllerCreator : MonoBehaviour{

    [SerializeField] private GameObject indivPrefab;
    [SerializeField] private GameObject world;
    public TMP_InputField populationInput;
    public InputData generationStepsInput;
    public InputData genomeLengthInput;
    public InputData internalNeuronCountInput;
    public InputData xSizeInput;
    public InputData ySizeInput;
    public InputData survivalConditionInput;
    public InputData mutationChanceInput;
    public InputData generationsInput;
    private int population;
    private int generationSteps;
    private int genomeLength;
    private int internalNeuronCount;
    private int xSize;
    private int ySize;
    private SurvivalConditions survivalCondition;
    private double mutationChance;
    private int generations;
    private SimController sim;
    private bool runSimulation;
    private bool simulationLoaded;
    private bool inputSaved;
    private int currentGeneration;
    private int currentStep;


    public void Start(){
        Application.targetFrameRate = 40;
        world = Instantiate(world);
        loadSimulation();
    }

    public void saveInputs(Dictionary<string, string> inputDict){
        try{
            population = int.Parse(inputDict["PopulationInput"]);
            generationSteps = int.Parse(inputDict["GenerationStepsInput"]);
            genomeLength = int.Parse(inputDict["GenomeLengthInput"]);
            internalNeuronCount = int.Parse(inputDict["InternalNeuronCountInput"]);
            xSize = int.Parse(inputDict["XSizeInput"]);
            ySize = int.Parse(inputDict["YSizeInput"]);
            survivalCondition = (SurvivalConditions) Enum.Parse(typeof(SurvivalConditions), inputDict["SurvivalConditionInput"]);
            generations = int.Parse(inputDict["GenerationsInput"]);
            mutationChance = double.Parse(inputDict["MutationChanceInput"]);
            inputSaved = true;
        }
        catch{
            throw new ArgumentException("Inputs must be the correct type");
        }
    }

    public void loadSimulation(){
        if (inputSaved){
            Grid grid = world.GetComponentInChildren<Grid>();
            this.sim = new SimController(population, generationSteps, genomeLength, internalNeuronCount, xSize, ySize, survivalCondition, mutationChance, grid);
            this.sim.setupSimulation();
            foreach (Individual indiv in this.sim.individuals){
                GameObject indivObject = Instantiate(indivPrefab, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
                IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
                indivWrapper.grid = grid;
                indivWrapper.indiv = indiv;
            }
            simulationLoaded = true;
        }
    }

    public void playPauseSimulation(){
        runSimulation = !runSimulation;
    }

    public void oneStep(){
        runSimulation = true;
        simulationStep();
        runSimulation = false;
    } 

    private void Update(){
        if (!simulationLoaded){loadSimulation();}
        else{simulationStep();}
    }

    private void simulationStep(){
        if (runSimulation && simulationLoaded){
            if (currentGeneration == generations){return;}
            if (currentStep == generationSteps){
                GameObject[] instances = GameObject.FindGameObjectsWithTag("Individual");
                foreach (GameObject target in instances){Destroy(target);}
                this.sim.setupNextGeneration();
                currentStep = 0;
                currentGeneration ++;
                Grid grid = world.GetComponentInChildren<Grid>();

                foreach (Individual indiv in this.sim.individuals){
                    GameObject indivObject = Instantiate(indivPrefab, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
                    IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
                    indivWrapper.grid = grid;
                    indivWrapper.indiv = indiv;
                    indivWrapper.name = "Test";
                }
            }
            else{
                this.sim.step(currentStep);
                currentStep ++;
            }
        }

    }

}