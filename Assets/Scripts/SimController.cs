using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimController : MonoBehaviour
{
    private World world;
    private int population;
    private int generationSteps;
    private int genomeLength;
    private int internalNeuronCount;
    private int xSize;
    private int ySize;
    private SurvivalConditions survivalCondition;
    private double mutationChance;

    private List<Individual> individuals;

    public SimController(int population, int generationSteps, int genomeLength, int internalNeuronCount,
                         int xSize, int ySize, SurvivalConditions survivalCondition, double mutationChance){
        this.world = new World(xSize, ySize);
        this.population = population;
        this.generationSteps = generationSteps;
        this.genomeLength = genomeLength;
        this.internalNeuronCount = internalNeuronCount;
        this.survivalCondition = survivalCondition;
        this.mutationChance = mutationChance;
        this.individuals = createIndividuals(population);
    }

    public void setupSimulation(){
        for (int i = 0; i < individuals.Count; i++)
        {
            Individual indiv = individuals[i];
            int xCoord, yCoord;
            addIndividualToWorld(indiv, out xCoord, out yCoord);
            var dataDict = getInputData(indiv, 0, xCoord, yCoord);
            indiv.spawn(dataDict, createRandomGenome());
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

    private void setupNextGeneration(){
        int original = individuals.Count;
        individuals = findSurvivors();
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
            (int, int) indivDirection = MoveDirectionsHelper.getCoordinates((MoveDirections)new System.Random().Next(Enum.GetValues(typeof(MoveDirections)).Length));
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

    private void updateInputData(Individual individual, int generationStep, int? nullableX, int? nullableY)
    {
        var dataDict = getInputData(individual, generationStep, nullableX, nullableY);
        individual.updateData(dataDict);
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
        var dataDict = new Dictionary<InputTypes, double>{
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

    private string createRandomGenome(){
        return string.Format("%0{0}x", new System.Random().Next(16, (int)Math.Pow(16, genomeLength) - 1));
    }
}
