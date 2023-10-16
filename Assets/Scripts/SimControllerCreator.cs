using UnityEngine;

class SimControllerCreator : MonoBehaviour{

    [SerializeField]private GameObject world;
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
    [SerializeField]private GameObject indivObject;


    public void Start(){
        world = Instantiate(world);
        Grid grid = world.GetComponentInChildren<Grid>();
        this.sim = new SimController(population, generationSteps, genomeLength, internalNeuronCount, xSize, ySize, survivalCondition, mutationChance, grid);
        this.sim.setupSimulation();
        foreach (Individual indiv in this.sim.individuals){
            indivObject = Instantiate(indivObject, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
            IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
            indivWrapper.grid = grid;
            indivWrapper.indiv = indiv;
        }
    }

    public void startSimulation(){
        this.sim.runSimulation(generations);
    }

}