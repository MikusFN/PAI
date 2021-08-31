using Assets.GeneralScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    [SerializeField]
    [HideInInspector]
    private float steering, throttle, lastSteering, lastThrottle;//, firstRight, firstLeft, firstFront, firstBack;
    [SerializeField]
    [HideInInspector]
    private bool autoPilot;
    [SerializeField]
    [HideInInspector]
    private bool balanca;
    CarController car;
    LoadXMLData data;
    private float[] directionValues, firstDirections;
    public float scaleSpeed;
    public float diferenceH, diferenceV;
    private RaycastHit hit;

    void Awake()
    {
        hit = new RaycastHit();
        directionValues = new float[4];
        firstDirections = new float[4];
        data = GetComponent<LoadXMLData>();
        car = GetComponent<CarController>();
        autoPilot = false;
        directionValues[0] = data.Frente;
        directionValues[1] = data.Direita;
        directionValues[2] = data.Tras;
        directionValues[3] = data.Esquerda;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < firstDirections.Length; i++)
        {
            firstDirections[i] = directionValues[i];
        }
        diferenceH = (Mathf.Abs(data.Direita - data.Esquerda) * scaleSpeed);
        diferenceV = (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed);
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        car.ActivateThrusters();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        directionValues[0] = data.Frente;
        directionValues[1] = data.Direita;
        directionValues[2] = data.Tras;
        directionValues[3] = data.Esquerda;
        int j = 0;

        if (Input.GetKeyDown(KeyCode.P))
            autoPilot = !autoPilot;

        for (int i = 0; i < directionValues.Length; i++)
        {
            if (directionValues[i] != firstDirections[i])
                j++;
        }

        if (data.SomaValores != 0 && j != 0)
        {
            balanca = true;
        }
        else
        {
            balanca = false;
        }

        if (!autoPilot && car.thrusterActivated )//&& !car.isInclined)
        {
            if (!balanca)
            {
                steering = Input.GetAxis("Horizontal");
                throttle = Input.GetAxis("Vertical");
                ////throttle = 1;
            }
            else
            {
                throttle = 1;

                if (Mathf.Abs(data.Direita - data.Esquerda) * scaleSpeed > diferenceH)
                {
                    if (data.Direita > data.Esquerda)
                    {
                        if (data.Direita * scaleSpeed < 1)
                            steering = data.Direita * scaleSpeed;
                    }
                    else
                    {
                        if (data.Esquerda * scaleSpeed < 1)
                            steering = -data.Esquerda * scaleSpeed;
                    }
                }
                else if (Mathf.Abs(data.Direita - data.Esquerda) * scaleSpeed < diferenceH)
                {
                    steering = 0;
                }

                //if (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed > diferenceV)
                //{
                //    if (data.Frente > data.Tras)
                //    {
                //        if (data.Frente * scaleSpeed < 1)
                //            throttle = data.Frente * scaleSpeed;
                //    }
                //    else
                //    {
                //        if (data.Tras * scaleSpeed < 1)
                //            throttle = -data.Tras * scaleSpeed;
                //    }
                //}
                //else if (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed < diferenceV)
                //{
                //    throttle = 0;
                //}
                //Debug.Log(Mathf.Abs(data.Direita - data.Esquerda) * scaleSpeed);
                //if (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed > 0.3f)
                //{
                //    if (data.Frente > data.Tras)
                //        throttle = data.Frente * scaleSpeed;
                //    else
                //    {
                //        throttle = -data.Tras * scaleSpeed;
                //    }
                //}
                //steering = (data.Direita > data.Esquerda && (data.Direita - data.Esquerda) > 0.1f ? data.Direita : -data.Esquerda)*0.1f ;
                //throttle = (data.Frente > data.Tras && (data.Frente - data.Tras) > 0.1f ? data.Frente : -data.Tras)*0.1f;
            }

        }
        else if (car.isInclined && balanca)
        {
            throttle = Input.GetAxis("Vertical");
            if (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed > diferenceV)
            {
                if (data.Frente > data.Tras)
                {
                    if (data.Frente * scaleSpeed < 1)
                        throttle = data.Frente * scaleSpeed;
                }
                else
                {
                    if (data.Tras * scaleSpeed < 1)
                        throttle = -data.Tras * scaleSpeed;
                }
            }
            else if (Mathf.Abs(data.Frente - data.Tras) * scaleSpeed < diferenceV)
            {
                throttle = 0;
            }
        }
        if (balanca)
        {
            car.Respawn(Input.GetKeyDown(KeyCode.R) || (Physics.Raycast(transform.position, transform.forward, out hit, 1f) && (hit.collider.tag == "LeftRail" || hit.collider.tag == "RightRail")));
            Debug.Log(Input.GetKeyDown(KeyCode.R) || (Physics.Raycast(transform.position, transform.forward, out hit, 1f) && (hit.collider.tag == "LeftRail" || hit.collider.tag == "RightRail")));
        }
        //else if((!car.isInclined))
        //{
        //    autoPilot = false;
        //}
        //Debug.Log("steering -> " + steering);
        //Debug.Log("throttle -> " + throttle);
        car.Move(Mathf.Lerp(steering, lastSteering, 0.7f), Mathf.Lerp(throttle, lastThrottle, 0.7f), autoPilot);
        lastSteering = steering;
        lastThrottle = throttle;

    }
}
