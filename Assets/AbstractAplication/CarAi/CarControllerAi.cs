using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class CarControllerAi : MonoBehaviour
{
    
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    Rigidbody rigidbody;
    public BezierSpline spline;

    [SerializeField]
    bool manual;

    float motor;
    float steering;

    void Start()
    {
        Vector3 pos = spline.GetPointInSpline(0.02f);
        pos.y = pos.y + 0.3f;
        //pos.z = pos.z + 5;
        //pos.x = pos.x + 5;

        transform.position = pos;
        transform.LookAt(pos + spline.GetDirection(0.001f));
        rigidbody = GetComponent<Rigidbody>();
    }

    //private void Reset()
    //{
    //    Vector3 pos = spline.GetPointInSpline(0.02f);
    //    pos.y = pos.y + 0.3f;
    //    //pos.z = pos.z + 5;
    //    //pos.x = pos.x + 5;

    //    transform.position = pos;
    //    transform.LookAt(pos + spline.GetDirection(0.001f));
    //    //rigidbody = GetComponent<Rigidbody>();
    //}

    public void FixedUpdate()
    {
        if (manual)
        {
            motor = maxMotorTorque * (Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0));
            steering = maxSteeringAngle * (Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0));
        }
        float brake = (Mathf.Abs(motor) < maxMotorTorque / 10 ) ? maxMotorTorque : 0;
        //Debug.Log("steering -> "+steering);
        //Debug.Log("motor -> "+motor);
        Move(steering, motor, false);
    }

    public void Set_motor(float m)
    {
        if (!manual)
            motor = m * maxMotorTorque;
    }

    public void Set_steering(float s)
    {
        if (!manual)
            steering = s * maxSteeringAngle;
    }
    public void Move(float sterring, float throttle, bool autoPilot)
    {
        Vector3 fowardForce = Vector3.zero;
        
            rigidbody.drag = 5f;// Para calcular a resitencia do ar no objecto que tem o rigidBody (decellarate)
                                //Turn into a function that computes add force based on input, gets a vector an adds force
            fowardForce = transform.forward * 10 * throttle;
            fowardForce = fowardForce * Time.fixedDeltaTime * rigidbody.mass;
            rigidbody.AddForce(fowardForce, ForceMode.Force);

        Vector3 turnForce = transform.up * 10;

        if ((throttle) >= 0)
            turnForce = turnForce * sterring;
        else
        {
            turnForce = turnForce * -sterring;
        }
       
        turnForce = turnForce * Time.fixedDeltaTime * rigidbody.mass;
        rigidbody.AddTorque(turnForce, ForceMode.Force);
    }
}