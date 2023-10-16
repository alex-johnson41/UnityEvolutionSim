using System;

abstract public class Neuron
{
    public int id;

    protected Neuron(int id){
        this.id = id;
    }

    public virtual void setData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class setData function");
    }
    public virtual void addData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class addData function");
    }
    public virtual double getData(){
        throw new InvalidOperationException("This class has not overwritten the base class getData function");
    }
    public virtual void addSelfData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class addSelfData function");
    }
}