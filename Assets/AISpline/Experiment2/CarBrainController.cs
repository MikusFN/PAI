using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


    [RequireComponent(typeof(CarControllerAi))]
    [RequireComponent(typeof(NeuralNetwork))]
    public class CarBrainController : MonoBehaviour
    {
        private CarControllerAi m_CarController;    // Reference to actual car controller we are controlling
        private NeuralNetwork m_BlackBox;
        private Rigidbody m_Rigidbody;
        private bool isReset = true;

        public BezierSpline track;
        public int nextPoint = 0;
        public float sight = 25f;
        public Transform sensor;
        public Transform forward;
        public bool ignoreCollision = false;
        public Text indicator;
        public Text speedIndicator;

        //    [DBG_Track()]
        public float Steering = 0f;
        //   [DBG_Track()]
        public float Gass = 0f;
        //    [DBG_Track()]
        public float Speed = 0f;

        private void Awake()
        {
            m_CarController = GetComponent<CarControllerAi>();
            m_BlackBox = GetComponent<NeuralNetwork>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            if (isReset)
            {
                // reset all forces
                m_Rigidbody = GetComponent<Rigidbody>();
                m_Rigidbody.isKinematic = true;
                m_CarController.Move(0f, 0f, false);

                var wheels = GetComponentsInChildren<WheelCollider>();
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = float.MaxValue;
                }
                isReset = false;
                return;
            }

            if (m_Rigidbody.isKinematic)
            {
                m_Rigidbody.isKinematic = false;
                m_CarController.Move(0f, 0f, false);
                var wheels = GetComponentsInChildren<WheelCollider>();
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = 0;
                }
            }

            //Corre um ponto a cada update
            var sqrDistanceToNextPoint = Vector3.SqrMagnitude(transform.position - track.points[nextPoint]);
            //Se inferior a 1000 entao avança-se para o proximo ponto até chegar ao ultimo
            if (sqrDistanceToNextPoint < 1000f)
            {
                ++nextPoint;
                if (nextPoint == track.points.Length) //Se chega ao fim a contagem vai pro inicio
                    nextPoint = 0;
            }

            List<float> inputs = new List<float>();

        //            var forwardTrackPos = track.GetClosestPoint(forward.position);
        //            var trackPos = track.GetClosestPoint(sensor.position);
        //            var direction = (forwardTrackPos - trackPos).normalized;
        Vector3 direction = Vector3.zero; ;
            //float distance;
        //track.GetPointInSpline(track.t, )
            //track.GetClosestPoint(forward.position, out direction, out distance);
            if (direction == Vector3.zero)
                Debug.Log("Warning, direction zero");
            Debug.DrawRay(transform.position, direction * 10f, Color.blue);
            direction = transform.InverseTransformDirection(direction);
            inputs.Add(direction.x);
            inputs.Add(direction.y);
            inputs.Add(direction.z);

            /*
            var point = transform.InverseTransformPoint(track.points[nextPoint].position).normalized;
            inputs.Add(point.x);
            inputs.Add(point.y);
            inputs.Add(point.z);
            Debug.DrawLine(transform.position, track.points[nextPoint].position, Color.green);

            var afterNextPoint = nextPoint + 1 == track.pointCount ? 0 : nextPoint + 1;
            point = transform.InverseTransformPoint(track.points[afterNextPoint].position).normalized;
            inputs.Add(point.x);
            inputs.Add(point.y);
            inputs.Add(point.z);
            Debug.DrawLine(transform.position, track.points[afterNextPoint].position, Color.blue);
            */

            //Speed = m_CarController.CurrentSpeed / m_CarController.MaxSpeed;
            //inputs.Add(Speed);

            var sensors = new[]
            {
                new { direction = sensor.forward, distance = 200f },
                new { direction = Quaternion.AngleAxis(5f, sensor.up) * sensor.forward, distance = 200f },
                new { direction = Quaternion.AngleAxis(-5f, sensor.up) * sensor.forward, distance = 200f },
                new { direction = Quaternion.AngleAxis(15f, sensor.up) * sensor.forward, distance = 130f },
                new { direction = Quaternion.AngleAxis(-15f, sensor.up) * sensor.forward, distance = 130f },
                new { direction = Quaternion.AngleAxis(30f, sensor.up) * sensor.forward, distance = 50f },
                new { direction = Quaternion.AngleAxis(-30f, sensor.up) * sensor.forward, distance = 50f }
            };

            RaycastHit hit = new RaycastHit();
            foreach (var s in sensors)
            {
                hit.distance = s.distance;
                Physics.Raycast(sensor.position, s.direction, out hit, s.distance);
                inputs.Add(hit.distance / s.distance);
                if (hit.collider)
                    Debug.DrawRay(sensor.position, s.direction * hit.distance, Color.red);
            }

            List<float> outputs = m_BlackBox.Process(inputs);

            Steering = outputs[0];
            Gass = outputs[1];
            m_CarController.Move((float)outputs[0], 1, false);

            string text = "Input : ";
            foreach (float i in inputs)
                text += "\n" + i;
            text += "\nOutput : ";
            foreach (float o in outputs)
                text += "\n" + o;

            text += "\nFitness : " + LearnManager.Instance.CurrentFitness;
            //indicator.text = text;

            //speedIndicator.text = (int)m_CarController.CurrentSpeed + " MPH";
        }

        public void Reset()
        {
            isReset = true;
            nextPoint = 0;
            m_CarController.Move(0f, 0f,false);
            StartCoroutine(NeedReset());
        }

        IEnumerator NeedReset()
        {
            yield return new WaitForSeconds(0.5f);
            if (Speed < 0.01f && Gass > 0.5f)
            {
                isReset = true;
//                gameObject.SetActive(false);
//                gameObject.SetActive(true);
                LearnManager.Instance.Respawn();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!ignoreCollision)
                LearnManager.Instance.ReportCrash(collision.gameObject);
        }
    }
