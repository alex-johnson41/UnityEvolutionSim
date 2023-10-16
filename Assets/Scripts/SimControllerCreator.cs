using UnityEngine;

class SimControllerCreator : MonoBehaviour{

    [SerializeField] private Grid grid;
    [SerializeField]private int population;
    [SerializeField]private int generationSteps;
    [SerializeField]private int genomeLength;
    [SerializeField]private int internalNeuronCount;
    [SerializeField]private int xSize;
    [SerializeField]private int ySize;
    [SerializeField]private SurvivalConditions survivalCondition;
    [SerializeField]private double mutationChance;
    [SerializeField]private int generations;
    private SimController sim;


    public void Start(){
        this.sim = new SimController(population, generationSteps, genomeLength, internalNeuronCount, xSize, ySize, survivalCondition, mutationChance, grid);
        this.sim.setupSimulation();
    }

    public void startSimulation(){
        this.sim.runSimulation(generations);
    }

}