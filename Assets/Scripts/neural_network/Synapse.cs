public class Synapse
{
    public Neuron input;
    public Neuron output;
    private double weight;
    private int rank;

    public Synapse(Neuron input, Neuron output, double weight){
        this.input = input;
        this.output = output;
        this.weight = weight;
        rank = calculateRank();
    }

    public void sendData(){
        double weightedData = input.getData() * weight;
        if (input == output){
            output.addSelfData(weightedData);
        }
        else{
            output.addData(weightedData);
        }
    }

    public int getRank(){
        return rank;
    }

    private int calculateRank(){
        if (input.GetType() == typeof(InputNeuron) & output.GetType() == typeof(InternalNeuron)){
            return 0;
        }
        if (input == output){
            return 2;
        }
        if (input.GetType() == typeof(InternalNeuron) & output.GetType() == typeof(InternalNeuron)){
            return 1;
        }
        return 3;
    }
}