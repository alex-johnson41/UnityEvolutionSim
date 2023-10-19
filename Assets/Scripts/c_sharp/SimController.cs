using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimController
{
    [SerializeField] private Grid grid;
    [SerializeField]private int population;
    [SerializeField]public int generationSteps;
    [SerializeField]private int genomeLength;
    [SerializeField]private int internalNeuronCount;
    [SerializeField]private int xSize;
    [SerializeField]private int ySize;
    [SerializeField]private SurvivalConditions survivalCondition;
    [SerializeField]private double mutationChance;
    public List<Individual> individuals {get; private set;}
    private World world;

    public SimController(int population, int generationSteps, int genomeLength, int internalNeuronCount,
                         int xSize, int ySize, SurvivalConditions survivalCondition, double mutationChance, Grid grid){
        this.world = new World(xSize, ySize, grid);
        this.population = population;
        this.generationSteps = generationSteps;
        this.genomeLength = genomeLength;
        this.internalNeuronCount = internalNeuronCount;
        this.survivalCondition = survivalCondition;
        this.mutationChance = mutationChance;
        this.individuals = createIndividuals(population);
        this.grid = grid;
    }

    public void setupSimulation(){
        for (int i = 0; i < individuals.Count; i++)
        {
            Individual indiv = individuals[i];
            int xCoord, yCoord;
            this.addIndividualToWorld(indiv, out xCoord, out yCoord);
            var dataDict = this.getInputData(indiv, 0, xCoord, yCoord);
            indiv.spawn(dataDict, this.createRandomGenome(), xCoord, yCoord);
        }
    }

    public void runSimulation(int generations){
        for (int i = 0; i < generations; i++){
            runGeneration(i);
            setupNextGeneration();
        }
    }

    public void runGeneration(int generationId){
        for (int i = 0; i < generationSteps; i++){
            step(i);
        }
    }

    public void step(int generationStep){
        for (int i = 0; i < individuals.Count; i++){
            var individual = individuals[i];
            var actionsDict = individual.step();
            performActions(individual, actionsDict, out int? newX, out int? newY);
            updateInputData(individual, generationStep, newX, newY);
        }
    }

    public void setupNextGeneration(){
        int original = individuals.Count;
        individuals = findSurvivors();
        Debug.Log(individuals.Count);
        Console.WriteLine((double)individuals.Count / original * 100);
        var newIndivs = newIndividuals();
        individuals.AddRange(newIndivs);
        world.clearMap();
        for (int i = 0; i < individuals.Count; i++){
            addIndividualToWorld(individuals[i], out int xCoord, out int yCoord);
            individuals[i].setInputData(getInputData(individuals[i], 0, xCoord, yCoord));
        }
    }

    private List<Individual> newIndividuals(){
        var indivs = new List<Individual>();
        for (int i = 0; i < population - individuals.Count; i++){
            double mutation = new System.Random().NextDouble();
            string genomeHex = individuals[new System.Random().Next(individuals.Count)].Genome;
            if (mutation < mutationChance){
                char[] genomeChars = genomeHex.ToCharArray();
                int randomIndex = new System.Random().Next(genomeChars.Length);
                genomeChars[randomIndex] = "0123456789ABCDEF"[new System.Random().Next(16)];
                genomeHex = new string(genomeChars);
            }
            var indiv = new Individual(internalNeuronCount);
            indiv.createNnet(genomeHex);
            indivs.Add(indiv);
        }
        return indivs;
    }

    private List<Individual> findSurvivors(){
        List<Individual> survivors = new List<Individual>();
        switch (survivalCondition){
            case SurvivalConditions.RIGHT_SIDE:
                // foreach (Individual indiv in individuals){
                //     int x;
                //     int y;
                //     world.findIndividual(indiv, out x, out y);
                //     if (x >= world.Width / 2){survivors.Add(indiv);}
                // }
                survivors.AddRange(individuals.Where(indiv =>{
                    world.findIndividual(indiv, out int x, out int y);
                    return x >= world.Width / 2;
                }));
                break;
            case SurvivalConditions.LEFT_SIDE:
                survivors.AddRange(individuals.Where(indiv =>{
                    world.findIndividual(indiv, out int x, out int y);
                    return x <= world.Width / 2;
                }));
                break;
        }
        return survivors;
    }

    private void performActions(Individual individual, Dictionary<OutputTypes, double> actionsDict, out int? newX, out int? newY){
        double x_offset = actionsDict.ContainsKey(OutputTypes.MOVE_X) ? actionsDict[OutputTypes.MOVE_X] : 0;
        double y_offset = actionsDict.ContainsKey(OutputTypes.MOVE_Y) ? actionsDict[OutputTypes.MOVE_Y] : 0;
        if (actionsDict.ContainsKey(OutputTypes.MOVE_FORWARD)){
            (int, int) indivDirection = MoveDirectionsHelper.getCoordinates(individual.Forward);
            var level = actionsDict[OutputTypes.MOVE_FORWARD];
            x_offset += indivDirection.Item1 * level;
            y_offset += indivDirection.Item2 * level;
        }
        if (actionsDict.ContainsKey(OutputTypes.MOVE_RANDOM)){
            (int, int) indivDirection = (new System.Random().Next(3) - 1, new System.Random().Next(3) - 1);
            var level = actionsDict[OutputTypes.MOVE_RANDOM];
            x_offset += indivDirection.Item1 * level;
            y_offset += indivDirection.Item2 * level;
        }
        double probability_x_offset = Math.Abs(Math.Tanh(x_offset));
        double probability_y_offset = Math.Abs(Math.Tanh(y_offset));
        int x_offset_int = (new System.Random().NextDouble() < probability_x_offset) ? Math.Sign(x_offset) : 0;
        int y_offset_int = (new System.Random().NextDouble() < probability_y_offset) ? Math.Sign(y_offset) : 0;
        if (x_offset_int != 0 || y_offset_int != 0){
            world.tryMoveIndividual(individual, x_offset_int, y_offset_int, out newX, out newY);
            individual.Forward = MoveDirectionsHelper.getMoveDirection(x_offset_int, y_offset_int);
            return;
        }
        newX = null;
        newY = null;
    }

    private void updateInputData(Individual individual, int generationStep, int? newX, int? newY)
    {
        var dataDict = getInputData(individual, generationStep, newX, newY);
        individual.updateData(dataDict, newX, newY);
    }

    private Dictionary<InputTypes, double> getInputData(Individual individual, int generationStep, int? nullableX, int? nullableY)
    {
        int x, y;
        if (nullableX.HasValue && nullableY.HasValue){
            x = nullableX.Value;
            y = nullableY.Value;
        }
        else{
            world.findIndividual(individual, out x, out y);
        }
        Dictionary<InputTypes, double> dataDict = new Dictionary<InputTypes, double>{
            { InputTypes.X_POSITION, x / (double)world.Width },
            { InputTypes.Y_POSITION, y / (double)world.Height },
            { InputTypes.AGE, generationStep },
            { InputTypes.RANDOM, new System.Random().NextDouble() },
            { InputTypes.CLOSEST_X_BORDER, Math.Round(x / (double)world.Width) },
            { InputTypes.CLOSEST_Y_BORDER, Math.Round(y / (double)world.Height) }
        };
        return dataDict;
    }

    private void addIndividualToWorld(Individual indiv, out int x, out int y){
        world.randomOpenCell(out x, out y);
        world.addIndividual(indiv, x, y);
    }

    private List<Individual> createIndividuals(int population){
        var indivs = new List<Individual>();
        for (int i = 0; i < population; i++){
            var indiv = new Individual(internalNeuronCount);
            indivs.Add(indiv);
        }
        return indivs;
    }
    
    public string createRandomGenome() => string.Concat(Enumerable.Range(0, genomeLength).Select(_ => Guid.NewGuid().ToString("N").Substring(0, 8)));
}
