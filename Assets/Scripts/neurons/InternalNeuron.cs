using System;


public class InternalNeuron : Neuron {

    private double data;
    private double selfInput;

    public InternalNeuron(int id) : base(id){
        selfInput = 0;
    }
    
    public override void addData(double data){
        this.data += data;
    }

    public override double getData(){
        double data = this.data + selfInput;
        selfInput = 0;
        this.data = 0;
        return Math.Tanh(data);
    }

    public override void addSelfData(double data){
        selfInput = data;
    }
}