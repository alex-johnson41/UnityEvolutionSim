using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Individual : MonoBehaviour
{
    private Dictionary<int, InputNeuron> inputNeuronsDict;
    private Dictionary<int, OutputNeuron> outputNeuronsDict;
    private Dictionary<int, InternalNeuron> internalNeuronsDict;
    private MoveDirections forward;
    private NeuralNetwork nnet;
    private string genome;

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
    public void spawn(Dictionary<InputTypes, double> inputData, string genomeHex){
        setInputData(inputData);
        createNnet(genomeHex);
    }

    public void updateData(Dictionary<InputTypes, double> inputData){
        setInputData(inputData);
    }

    public Dictionary<OutputTypes, double> step(){
        foreach (Synapse synapse in nnet.getSynapses()){
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
        Synapse[] synapses = new Synapse[genomeHex.Length / 4];
        string binaryString = (from character in genomeHex select hexCharacterToBinary[character]).ToString();
        string[] binaryGenome = (string[]) binaryString.Select((c, index) => new {c, index})
            .GroupBy(x => x.index/8)
            .Select(group => group.Select(elem => elem.c))
            .Select(chars => new string(chars.ToArray()));
        foreach(string binaryGene in binaryGenome){
            Neuron input;
            Neuron output; 
            int inputType = int.Parse(binaryGene[..1]);
            int inputId = int.Parse(binaryGene.Substring(1, 7));
            int outputType = int.Parse(binaryGene.Substring(8,1));
            int outputId = int.Parse(binaryGene.Substring(9,7));
            int weightSign = int.Parse(binaryGene.Substring(16,1));
            int unsignedWeight = int.Parse(binaryGene.Substring(17)) / 10000;
            int weight = unsignedWeight * weightSign;
            if (inputType == 0){input = inputNeuronsDict[inputId % inputNeuronsDict.Count];}
            else{input = internalNeuronsDict[inputId % internalNeuronsDict.Count];}
            if (outputType == 0){output = outputNeuronsDict[outputId % outputNeuronsDict.Count];}
            else{output = internalNeuronsDict[outputId % internalNeuronsDict.Count];}
            synapses.Append(new Synapse(input, output, weight));
        }
        return new NeuralNetwork(synapses);
    }

    private MoveDirections randomDirection(){
        Array values = Enum.GetValues(typeof(MoveDirections));
        System.Random random = new System.Random();
        return (MoveDirections)values.GetValue(random.Next(values.Length));
    }

    private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
        { '0', "0000" },
        { '1', "0001" },
        { '2', "0010" },
        { '3', "0011" },
        { '4', "0100" },
        { '5', "0101" },
        { '6', "0110" },
        { '7', "0111" },
        { '8', "1000" },
        { '9', "1001" },
        { 'a', "1010" },
        { 'b', "1011" },
        { 'c', "1100" },
        { 'd', "1101" },
        { 'e', "1110" },
        { 'f', "1111" }
    };
}