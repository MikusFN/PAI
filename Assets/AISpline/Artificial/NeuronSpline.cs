using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NeuronSpline
{
    public int m_inputsCount;
    public List<float> m_weights;

    public NeuronSpline(int count)
    {
        m_inputsCount = count + 1;
        m_weights = new List<float>();
        for (int i = 0; i < m_inputsCount; ++i)
        {
            m_weights.Add(Random.Range(-1f, 1f));
        }
    }
}

