using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_car_Run : I_IA
{
    public RaceManager raceManager;
    public BezierSpline spline;
    //static int count_car = 0;
    static int count_collided = 0;

    public EndPoint endPoint;

    InfoCar car;
    CarControllerAi controller;

    Vector3 pos_init;
    Quaternion rot_init;
    Vector3 pos;

    float time_Alive = 0;
    bool collided = false;

    void Start()
    {
        //Start do carro com uma network
        pos = spline.GetPointInSpline(0.02f);
        pos.y = pos.y + 0.3f;
        //pos.z = pos.z + 5;
        //pos.x = pos.x + 5;

        //count_car++;
        pos_init = transform.position;
        rot_init = transform.rotation;
        network = new Network(3, 5, 2);
        car = GetComponent<InfoCar>();
        controller = GetComponent<CarControllerAi>();
    }

    void Update()
    {
        //Verifica senao colideu processa os inputs
        if (!collided)
        {
            time_Alive += Time.deltaTime;
            network.Compute(car.Get_Dist_Front(),
                            car.Get_Dist_Left(),
                            car.Get_Dist_Right());
            var output = network.Get_Output();
            controller.Set_steering((output[0] - 0.5f) * 2);
            //Para puder obter valores negativos (esquerda)
            //Debug.Log(output[0] - 0.5f);
            //controller.Set_motor(1);
            //network.Print();
            controller.Set_motor((output[1]) * 2);
        }
    }

    //Reset da posicao para inicial
    override public void Reset()
    {

        transform.position = pos;
        transform.rotation = rot_init;
        transform.LookAt(pos + spline.GetDirection(0.001f));

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        time_Alive = 0;
        collided = false;
        count_collided = 0;
    }

    //Distancia ao alvo
    public override float Get_Efficiency()
    {
        return 1 / (transform.position - endPoint.transform.position).magnitude;// time_Alive;
    }
    //Colisao poe o tempo  a zero para reiniciar
    void OnCollisionEnter(Collision other)
    {
        if (!collided)
        {
            collided = true;
            count_collided++;
            RaceManager.ResetTime();
            raceManager.text.text = "";
        }
    }
}
