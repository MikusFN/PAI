using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Network para servir de comparação
public class SortNetwork : MonoBehaviour
{

    //Valor network e geração
    float eficiency;
    Network net;
    int gen;

    public SortNetwork(float eficiencia, Network net, int gen)
    {
        this.eficiency = eficiencia;
        this.net = net;
        this.gen = gen;
    }
    //Getters
    public int Get_Gen()
    {
        return gen;
    }

    public Network Get_Network()
    {
        return net;
    }

    public float Get_Value()
    {
        return eficiency;
    }
    public void Set_Value(float value)
    {
        eficiency = value;
    }
    //Oeradores entre listas onde se compara a eficiencia
    public static bool operator <(SortNetwork n1, SortNetwork n2)
    {
        return n1.eficiency < n2.eficiency;
    }
    public static bool operator >(SortNetwork n1, SortNetwork n2)
    {
        return n1.eficiency > n2.eficiency;
    }
    public static int operator -(SortNetwork n1, SortNetwork n2)
    {
        return n1.eficiency < n2.eficiency ? 1 : -1;
    }

}
