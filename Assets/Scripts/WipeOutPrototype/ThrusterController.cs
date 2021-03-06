using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour
{
    public float strenght, distanceMax, maxStreghtStart, maxStrenght;
    public Transform[] thrusters;
    [HideInInspector]
    public bool inPlace = true;
    //[SerializeField]
    //private ParticleSystem particles;

    public new Rigidbody rigidbody;

    Vector3 downForce;
    public float distancePercent;

    //private void Awake()
    //{
    //    foreach (var item in thrusters)
    //    {
    //        particles.transform.position = item.transform.position;
    //        particles.Play();
    //    }
    //}

    void FixedUpdate()
    {

        RaycastHit hit;
        int j = 0;
        for (int i = 0; i < thrusters.Length; i++)
        {
            downForce = Vector3.zero;
            distancePercent = 0;

            if (Physics.Raycast(thrusters[i].position, -thrusters[i].up, out hit, distanceMax))
            {
                distancePercent = (1 - (hit.distance / distanceMax)) * 2;

                downForce = thrusters[i].up * strenght * distancePercent;
                //colliders[i].position = hit.collider.ClosestPoint(thrusters[i].position);
                //Debug.Log(hit.collider.tag);
                if (rigidbody)
                {
                    downForce = downForce * Time.fixedDeltaTime * rigidbody.mass;
                    Debug.DrawRay(thrusters[i].position, -downForce.normalized);
                    rigidbody.AddForceAtPosition(downForce, thrusters[i].position);
                }
                if (hit.collider.tag == "CorrectLine")
                {
                    j++;
                }
            }
        }
        if (j > 0)
        {
            inPlace = true;
        }
        else
        {
            inPlace = false;
        }
    }
}