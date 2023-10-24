using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class SimControllerCreator : MonoBehaviour{

    [SerializeField] private GameObject indivPrefab;
    [SerializeField] private GameObject world;
    private CustomGrid grid;
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
        world = Instantiate(world);
    }

    public void saveInputs(Dictionary<string, string> inputDict){
        try{
            Application.targetFrameRate = int.Parse(inputDict["FrameRateInput"]);
            population = int.Parse(inputDict["PopulationInput"]);
            generationSteps = int.Parse(inputDict["GenerationStepsInput"]);
            genomeLength = int.Parse(inputDict["GenomeLengthInput"]);
            internalNeuronCount = int.Parse(inputDict["InternalNeuronCountInput"]);
            xSize = int.Parse(inputDict["XSizeInput"]);
            ySize = int.Parse(inputDict["YSizeInput"]);
            survivalCondition = (SurvivalConditions) Enum.Parse(typeof(SurvivalConditions), inputDict["SurvivalConditionDropdown"]);
            generations = int.Parse(inputDict["GenerationsInput"]);
            mutationChance = double.Parse(inputDict["MutationChanceInput"]);
            inputSaved = true;
        }
        catch{
            throw new ArgumentException("Inputs must be the correct type");
        }
        loadSimulation();
    }

    public void loadSimulation(){
        clearSimulation();
        if (inputSaved){
            grid = initializeGrid();
            sim = new SimController(population, generationSteps, genomeLength, internalNeuronCount, xSize, ySize, survivalCondition, mutationChance);
            sim.setupSimulation();
            foreach (Individual indiv in sim.individuals){
                GameObject indivObject = Instantiate(indivPrefab, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
                IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
                indivWrapper.grid = grid;
                indivWrapper.indiv = indiv;
            }
            simulationLoaded = true;
        }
    }

    private CustomGrid initializeGrid(){
        Transform worldTransform = world.GetComponent<Transform>();
        CustomGrid grid = worldTransform.Find("Grid").GetComponent<CustomGrid>();
        grid.initializeGrid(xSize, ySize);
        return grid;
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

    private void clearSimulation(){
        GameObject[] instances = GameObject.FindGameObjectsWithTag("Individual");
        foreach (GameObject target in instances){Destroy(target);}
    }

    private void simulationStep(){
        if (runSimulation && simulationLoaded){
            if (currentGeneration == generations){
                clearSimulation();
                currentStep = 0;
                currentGeneration = 0;
                simulationLoaded = false;
                runSimulation = false;
            }
            if (currentStep == generationSteps){
                clearSimulation();
                sim.setupNextGeneration();
                currentStep = 0;
                currentGeneration ++;

                foreach (Individual indiv in sim.individuals){
                    GameObject indivObject = Instantiate(indivPrefab, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
                    IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
                    indivWrapper.grid = grid;
                    indivWrapper.indiv = indiv;
                    indivWrapper.name = "Test";
                }
            }
            else{
                sim.step(currentStep);
                currentStep ++;
            }
        }

    }

}