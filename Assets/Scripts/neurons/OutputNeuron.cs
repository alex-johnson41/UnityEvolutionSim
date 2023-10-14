using System;


public class OutputNeuron : Neuron {

    private double data;
    public OutputTypes type;

    public OutputNeuron(int id, OutputTypes type) : base(id){
        data = 0;
        this.type = type;
    }

    public new void addData(double data){
        this.data += data;
    }

    public double performAction(){
        try{
            double sendData = data;
            data = 0;
            return Math.Tanh(sendData);
        }
        catch{
            throw new FieldAccessException("No data has been set");
        }
    }
}