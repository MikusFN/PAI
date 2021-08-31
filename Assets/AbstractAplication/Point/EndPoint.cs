using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public float initial_y;
    public BezierSpline spline;
    private bool flag = true;

    void Start()
    {
        initial_y = transform.position.y;
        transform.position = spline.GetPointInSpline(1.0f);

    }

    void Update()
    {
        //Colocar o ponto no fim da pista
        //if (flag)
        //{
        //    transform.position = spline.PointsMesh[spline.PointsMesh.Count - 3];
        //    flag = !flag;
        //}
        //Aplicar uma rotação ao endpoint constante
        transform.position = new Vector3(transform.position.x,
                             initial_y + Mathf.Cos(Time.time * 3) / 10,
                             transform.position.z);
        transform.eulerAngles += new Vector3(0, Time.deltaTime * 50, 0);
    }
}
