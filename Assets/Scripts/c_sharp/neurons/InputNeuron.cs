

public class InputNeuron : Neuron{

    public InputTypes type;
    private double data;

    public InputNeuron(int id, InputTypes type) : base(id){
        this.type = type;
    }

    public override void setData(double data){
        this.data = data;
    }

    public override double getData(){
        return data;
    }
}