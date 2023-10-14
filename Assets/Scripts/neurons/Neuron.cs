using System;
using UnityEngine;

abstract public class Neuron : MonoBehaviour
{
    public int id;

    protected Neuron(int id){
        this.id = id;
    }

    public void setData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class setData function");
    }
    public void addData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class addData function");
    }
    public double getData(){
        throw new InvalidOperationException("This class has not overwritten the base class getData function");
    }
    public void addSelfData(double data){
        throw new InvalidOperationException("This class has not overwritten the base class addSelfData function");
    }
}