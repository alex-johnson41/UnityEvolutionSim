using System.Collections.Generic;
using System.Linq;

class NeuralNetwork
{
    private Synapse[] synapses;
    private OutputNeuron[] outputNeurons;

    public NeuralNetwork(Synapse[] synapses){
        this.synapses = sortSynapses(synapses);
        outputNeurons = getOutputNeurons(synapses);
    }

    public Dictionary<OutputTypes, double> getActions(){
        return outputNeurons.ToDictionary(outputNeuron => outputNeuron.type, outputNeuron => outputNeuron.performAction());
    }

    public Synapse[] getSynapses(){ return synapses; }
    
    private Synapse[] sortSynapses(Synapse[] synapses){
        return synapses.OrderBy(synapse => synapse.getRank()).ToArray();
    }
    
    private OutputNeuron[] getOutputNeurons(Synapse[] synapses){
        return (OutputNeuron[])(
            from synapse in synapses 
            where synapse.output.GetType() == typeof(OutputNeuron) 
            select synapse.output).ToHashSet().ToArray();
    }
}