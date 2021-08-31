using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronLayer
{
    public int m_neuronsCount;
    public List<NeuronSpline> m_neurons;

    public NeuronLayer(int neuronsCount, int inputPerNeuronsCount)
    {
        m_neuronsCount = neuronsCount;
        m_neurons = new List<NeuronSpline>();
        for (int i = 0; i < m_neuronsCount; ++i)
        {
            m_neurons.Add(new NeuronSpline(inputPerNeuronsCount));
        }
    }
}
