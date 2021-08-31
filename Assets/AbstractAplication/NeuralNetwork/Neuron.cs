using System;
using UnityEngine;

public class Neuron
{
    //Acesso static ao id do neuronio
    static int id = 0;
    static System.Random rand = new System.Random();

    //listas de neuronios da layer que precede e da que vem aseguir para computar todos os neuronios
    Neuron[] prev;
    Neuron[] next;
    //pesos
    double[] links_prev;

    double value = 0;
    int index;
    //Id interno
    int id_;

    public Neuron(int size_prev, int size_next, int index, bool random_init = true)
    {
        //Id do neuronio criado global static
        id_ = id;
        //Numero de neuronios
        id++;
        //Arrays de layer de neuronios anteriores e posteriores
        prev = new Neuron[size_prev];
        next = new Neuron[size_next];
        //Ligações à layer anterior
        links_prev = new double[size_prev];
        //Links anteriores à layer anterior
        if (random_init)
            for (int i = 0; i < size_prev; i++)
                links_prev[i] = rand.Next(-10, 10) / 10f;

        //indice para os pesos
        this.index = index;
    }
    //Set da layer anterior
    public void Set_Prev(Neuron[] neurons, int start_index)
    {
        for (int i = 0; i < prev.Length; i++)
            prev[i] = neurons[i + start_index];
    }
    //Set da nest layer
    public void Set_Next(Neuron[] neurons, int start_index)
    {
        for (int i = 0; i < next.Length; i++)
            next[i] = neurons[i + start_index];
    }
    //Obter o valor actual do neuronio
    public double Get_Value()
    {
        return value;
    }

    public void Set_Value(double d)
    {
        value = d;
    }
    //Get set pesos
    public double Get_Link(int index)
    {
        return links_prev[index];
    }

    public void Set_Link(int index_prev, float value)
    {
        links_prev[index_prev] = value;
    }

    public int Get_Id()
    {
        return id_;
    }
    //Soma todos os valores que estao na layer anterieor e multiplica pelo peso de cada uma
    public void Compute()
    {
        double sum = 0;
        for (int i = 0; i < prev.Length; i++)
            sum += prev[i].Get_Value() * links_prev[i];
        value = ToolsAi.Sigmoide(sum);
    }

    public void Print()
    {
        var s = "(" + id_ + ") = " + value;
        for (int i = 0; i < next.Length; i++)
            s += "\n    " + id_ + " -> " + next[i].Get_Id() + " : " + next[i].Get_Link(index);
        Debug.Log(s);
    }
}