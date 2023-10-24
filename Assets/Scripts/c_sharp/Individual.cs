using System;
using System.Collections.Generic;
using System.Linq;


public class Individual
{
    private Dictionary<int, InputNeuron> inputNeuronsDict;
    private Dictionary<int, OutputNeuron> outputNeuronsDict;
    private Dictionary<int, InternalNeuron> internalNeuronsDict;
    private MoveDirections forward;
    private NeuralNetwork nnet;
    private string genome;
    public int x {get; private set;}
    public int y {get; private set;}

    public string Genome => genome;
    public MoveDirections Forward{ get => forward; set => forward = value;}

    public Individual(int internalNeuronCount){
        forward = randomDirection();
        inputNeuronsDict = new Dictionary<int, InputNeuron>{};
        outputNeuronsDict = new Dictionary<int, OutputNeuron>{};
        internalNeuronsDict = new Dictionary<int, InternalNeuron>{};
        foreach (InputTypes value in (InputTypes[]) Enum.GetValues(typeof(InputTypes))){
            inputNeuronsDict.Add((int)value, new InputNeuron((int)value, value));
        }
        foreach (OutputTypes value in (OutputTypes[]) Enum.GetValues(typeof(OutputTypes))){
            outputNeuronsDict.Add((int)value, new OutputNeuron((int)value, value));
        }
        for (int i = 0; i < internalNeuronCount; i ++){
            internalNeuronsDict.Add(i, new InternalNeuron(i));
        }                  
    }

    public void spawn(Dictionary<InputTypes, double> inputData, string genomeHex, int x, int y){
        this.x = x;
        this.y = y;
        setInputData(inputData);
        createNnet(genomeHex);
    }

    public void updateData(Dictionary<InputTypes, double> inputData, int? newX, int? newY){
        if (newX != null && newY != null){
            this.x = newX.Value; 
            this.y = newY.Value;
        }
        setInputData(inputData);
    }

    public Dictionary<OutputTypes, double> step(){
        foreach (Synapse synapse in nnet.synapses){
            synapse.sendData();
        }
        return nnet.getActions();
    }

    public (double, double) getLocation(){
        try{
            double x = 0;
            double y = 0;
            foreach (InputNeuron neuron in inputNeuronsDict.Values){
                if (neuron.type == InputTypes.X_POSITION){
                    x = neuron.getData();
                }
                if (neuron.type == InputTypes.Y_POSITION){
                    y = neuron.getData();
                }
            }
            return (x, y);
        }
        catch{
            throw new AccessViolationException("Neural network hasn't been created - no data to pull from");
        }
    }

    public void createNnet(string genomeHex){
        genome = genomeHex;
        nnet = decodeGenomeHex(genomeHex);
    }

    public void setInputData(Dictionary<InputTypes, double> inputData){
        foreach(InputNeuron neuron in inputNeuronsDict.Values){
            neuron.setData(inputData[neuron.type]);
        }
    }

    private NeuralNetwork decodeGenomeHex(string genomeHex){
        List<Synapse> synapses = new List<Synapse>();
        List<string> binaryGenome = Enumerable.Range(0, genomeHex.Length / 8)
            .Select(i => genomeHex.Substring(i * 8, 8))
            .Select(chunk => string.Join(string.Empty, chunk.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')))).ToList();
        foreach(string binaryGene in binaryGenome){
            Neuron input;
            Neuron output; 
            int inputType = int.Parse(binaryGene.Substring(0,1));
            int inputId = int.Parse(binaryGene.Substring(1, 7));
            int outputType = int.Parse(binaryGene.Substring(8,1));
            int outputId = int.Parse(binaryGene.Substring(9,7));
            int weightSign = int.Parse(binaryGene.Substring(16,1)) == 0 ? -1 : 1;
            double unsignedWeight = (double) Convert.ToInt32(binaryGene.Substring(17), 2) / 10000;
            double weight = unsignedWeight * weightSign;
            if (inputType == 0){input = inputNeuronsDict[inputId % inputNeuronsDict.Count];}
            else{input = internalNeuronsDict[inputId % internalNeuronsDict.Count];}
            if (outputType == 0){output = outputNeuronsDict[outputId % outputNeuronsDict.Count];}
            else{output = internalNeuronsDict[outputId % internalNeuronsDict.Count];}
            synapses.Add(new Synapse(input, output, weight));
        }
        return new NeuralNetwork(synapses);
    }

    private MoveDirections randomDirection(){
        Array values = Enum.GetValues(typeof(MoveDirections));
        Random random = new Random();
        return (MoveDirections)values.GetValue(random.Next(values.Length));
    }
}