using UnityEngine;


public class InputNeuron : Neuron{

    public InputTypes type;
    private double data;

    public InputNeuron(int id, InputTypes type) : base(id){
        this.type = type;
    }

    public new void setData(double data){
        this.data = data;
    }

    public new double getData(){
        return data;
    }
}