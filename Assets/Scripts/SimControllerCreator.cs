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
    [SerializeField]private GameObject indivPrefab;
    private bool runSimulation;
    private int currentGeneration;
    private int currentStep;


    public void Start(){
        Application.targetFrameRate = 40;
        world = Instantiate(world);
        Grid grid = world.GetComponentInChildren<Grid>();
        this.sim = new SimController(population, generationSteps, genomeLength, internalNeuronCount, xSize, ySize, survivalCondition, mutationChance, grid);
        this.sim.setupSimulation();
        foreach (Individual indiv in this.sim.individuals){
            GameObject indivObject = Instantiate(indivPrefab, new Vector3(0,0,0), Quaternion.identity, grid.GetComponent<Transform>());
            IndividualWrapper indivWrapper = indivObject.GetComponent<IndividualWrapper>();
            indivWrapper.grid = grid;
            indivWrapper.indiv = indiv;
        }
    }

    public void startSimulation(){
        runSimulation = true;
    }

    private void Update(){
        simulationStep();
    }

    private void simulationStep(){
        if (runSimulation){
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