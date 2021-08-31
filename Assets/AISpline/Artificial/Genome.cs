using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    public List<float> m_weights;
    public float m_fitness;

    public Genome()
    {
        m_weights = new List<float>();
        m_fitness = 0;
    }

    public Genome(Genome copy)
    {
        m_weights = new List<float>(copy.m_weights);
        m_fitness = copy.m_fitness;
    }

    public Genome(List<float> weights, float fitness)
    {
        m_weights = weights;
        m_fitness = fitness;
    }
}

