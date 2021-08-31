using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    //Singleton do RaceManger
    static int generation = 0;
    static RaceManager instance;
    public const float Time_Cycle = 1000f; //seconds
    //public const float D_LEADERBOARD = 1f;
    public const int NUMBER_LEADERBOARD = 4;
    public IA_car_Run car;
    List<SortNetwork> bestNetworks = new List<SortNetwork>();
    float remainingTime = Time_Cycle;
    public Canvas canvas;
    public Text text;
    void Start()
    {
        instance = this;
    }
    //Verificação do reset
    void Update()
    {
        //vai diminuindo o tempo
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            //Quando Acaba o tempo ou collide (colisao poe o tempo a zero)
            new_cycle();
            generation++;
            remainingTime = Time_Cycle;
        }
    }

    //Escolher uma nova network
    void new_cycle()
    {
        //Cria uma list sorted para puder comparar
        var new_list = new List<SortNetwork>();
        //Instanciar a lista
        foreach (var n in bestNetworks)
        {
            new_list.Add(n);
            new_list[new_list.Count - 1].Set_Value(new_list[new_list.Count - 1].Get_Value());//* D_LEADERBOARD);
        }
        //Adiciona a ultima network que collidiu
        new_list.Add(new SortNetwork(car.Get_Efficiency(), car.Get_Network(), generation));
        car.Reset();
        //Ordena do maior pra o mais pequeno
        new_list = new_list.OrderBy(item => -item.Get_Value()).ToList();

        //Mantem a lista com 4 elementos
        if (bestNetworks.Count > NUMBER_LEADERBOARD)
            new_list.RemoveRange(new_list.Count - 1, 1);

        //Bredding da melhor com uma random da newlist que esteja no topo
        car.Set_Network(new Network(new_list[0].Get_Network(),
                                    new_list[UnityEngine.Random.Range(0, new_list.Count / 4)].Get_Network()));
        print("--Leader_board--");
        for (int i = 0; i < new_list.Count; i++)
        {
            print("Generation : " + new_list[i].Get_Gen() + " = " + new_list[i].Get_Value());
        }
        
        
            text.text = ("Best Generation : " + new_list[0].Get_Gen() + " = " + new_list[0].Get_Value().ToString()+
            "\n" + "Current Generation : " + generation + " = " + car.Get_Efficiency().ToString());
        
        print("----------------");
        //Passa a ser a nova lista de melhores resultados
        bestNetworks = new_list;
    }
    //reset do timer
    public static void ResetTime()
    {
        instance.remainingTime = 0;
    }
}
