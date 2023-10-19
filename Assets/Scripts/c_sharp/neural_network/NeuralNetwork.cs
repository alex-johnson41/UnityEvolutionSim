using System.Collections.Generic;
using System.Linq;

class NeuralNetwork
{
    public List<Synapse> synapses {get; private set;}
    private OutputNeuron[] outputNeurons;

    public NeuralNetwork(List<Synapse> synapses){
        this.synapses = sortSynapses(synapses);
        outputNeurons = getOutputNeurons(synapses);
    }

    public Dictionary<OutputTypes, double> getActions(){
        Dictionary<OutputTypes, double> actionsDict = new Dictionary<OutputTypes, double>();
        foreach (var outputNeuron in outputNeurons){
            if (!actionsDict.ContainsKey(outputNeuron.type)){
                actionsDict[outputNeuron.type] = outputNeuron.performAction();
            }
        }
        return actionsDict;
    }
    
    private List<Synapse> sortSynapses(List<Synapse> synapses){
        return synapses.OrderBy(synapse => synapse.getRank()).ToList();
    }
    
    private OutputNeuron[] getOutputNeurons(List<Synapse> synapses){
        return (
            from synapse in synapses 
            where synapse.output.GetType() == typeof(OutputNeuron) 
            select (OutputNeuron) synapse.output).ToArray();
    }
}