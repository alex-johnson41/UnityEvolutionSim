using System.Collections.Generic;
using System.Linq;

class NeuralNetwork
{
    private List<Synapse> synapses;
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

    public List<Synapse> getSynapses(){ return synapses; }
    
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