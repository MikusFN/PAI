using System;
using UnityEngine;

public class Network
{
    //Probabilidade de mutaçoes
    public const float proba_mutation = 0.1f;
    //Numero nueronios
    Neuron[] neurons;
    //numero de layers
    int[] layers;
    //array com a soma de a cada layer com as anteriores
    int[] sum_layers;
    //Total de layer
    int total = 0;

    //Mutação
    public Network(Network n1, Network n2)
    {
        //So pode comparar quando tem o mesmo tamanho
        if (n1.total != n2.total && n1.layers.Length != n2.layers.Length)
            throw new Exception("Networks don't have the same size");
        //Cria um array com as layers e o seu somatorio para saber como percorrrer os neuronios totais
        layers = new int[n1.layers.Length];
        sum_layers = new int[layers.Length];
        n1.layers.CopyTo(layers, 0);
        //Esta Network passa a ter o total de n1
        total = n1.total;
        //Define-se a primeira layer de inputs com o index do valor da primeira layer
        sum_layers[0] = layers[0];
        //Define-se as outras com o somatorio das restantes para ter o index certo de neuronios
        for (int i = 1; i < layers.Length; i++)
            sum_layers[i] = sum_layers[i - 1] + layers[i];
        //Novo numero de neuronios para esta network
        neurons = new Neuron[total];
        //Inicializa-se
        init(false);

        int countMutation = 0;
        int countNoneMutation = 0;

        //Corre-se cada layer e cada neuronio da layer a seguir a do inpoout
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                //Vai se buscar todos os links 
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    //indexado pela sum_layers
                    float f1 = (float)n1.neurons[j + sum_layers[i - 1]].Get_Link(k);
                    float f2 = (float)n2.neurons[j + sum_layers[i - 1]].Get_Link(k);
                    //Caso haja grande prob de mutação 
                    if (UnityEngine.Random.Range(0f, 1f) < proba_mutation)
                    {
                        //define-se um novo random para cada link
                        countMutation++;
                        neurons[j + sum_layers[i - 1]].Set_Link(k, f1 + UnityEngine.Random.Range(-5f, 5f) / 10f);
                    }
                    //caso nao soma-se a probabilidade ja defineida
                    else
                    {
                        countNoneMutation++;
                        float f = f1 + (f1 > f2 ? 0.1f : -0.1f);
                        neurons[j + sum_layers[i - 1]].Set_Link(k, f);
                    }
                }
            }
        }
    }

    //Define o numero de neuronios
    public Network(params int[] layers)
    {
        //Para ser um network tem que ter duas layers minimo
        if (layers.Length < 2)
            throw new Exception("Network must have at least 2 layers");
        //Define as layers de entreda como as da network
        this.layers = layers;
        //Cria a soma com mesmo tamanho
        sum_layers = new int[layers.Length];
        //define o primeiro como o primeirp da soma que vao ser sempre o mesmo numero de neuronios
        sum_layers[0] = layers[0];
        //Começa pela segunda layer que e soma todos os valores
        for (int i = 1; i < layers.Length; i++)
            sum_layers[i] = sum_layers[i - 1] + layers[i];
        //Numero total de neuronios na rede neuronal
        total = sum_layers[sum_layers.Length - 1];
        neurons = new Neuron[total];
        init();
    }

    private void init(bool random_init = true)
    {
        int index = 0;
        //Inicia os neuronios da input layer
        for (int i = 0; i < layers[0]; i++)
            neurons[i] = new Neuron(0, layers[1], i, random_init);
        //Agora definimos os valores da anterior e proxima layer relativamente as hidden layers
        index += layers[0];
        for (int i = 1; i < layers.Length - 1; i++)
        {
            //De acordo com cada numero de neuronios  agora com random links para a layer anterior
            for (int j = 0; j < layers[i]; j++)
                neurons[index + j] = new Neuron(layers[i - 1], layers[i + 1], j, random_init);
            //Traz-se o indice ao valor mais recente do n de neuronios
            index += layers[i];
        }
        //Definimos a output layer
        for (int i = 0; i < layers[layers.Length - 1]; i++)
            neurons[index + i] = new Neuron(layers[layers.Length - 2], 0, i, random_init);

        index = 0;
        //Definimos quais sao os neuronios da hidden layer para a layer inicial
        for (int i = 0; i < layers[0]; i++)
            neurons[i].Set_Next(neurons, layers[0]);
        index += layers[0];

        //Definimos o array de neuronios das hidden layers que guardam a layer anterior e proxima
        for (int i = 1; i < layers.Length - 1; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                neurons[index + j].Set_Next(neurons, index + layers[i]);
                neurons[index + j].Set_Prev(neurons, index - layers[i - 1]);
            }
            index += layers[i];
        }
        //Define o array de neuronios que precedem a layer de output
        for (int i = 0; i < layers[layers.Length - 1]; i++)
            neurons[i + index].Set_Prev(neurons, index - layers[layers.Length - 2]);
    }

    //Define is inputs que tem que ser igual ao numero de neuronios na primeira layer
    private void set_input(params double[] inputs)
    {
        if (inputs.Length != layers[0])
            throw new Exception("incorrect number of inputs");
        for (int i = 0; i < layers[0]; i++)
            neurons[i].Set_Value(inputs[i]);
    }

    //Faz a computaçao dos inputs das distancias que os tres sensores obteram
    public void Compute(params double[] inputs)
    {
        set_input(inputs);
        //Todos os neuronios entram no compute
        for (int i = layers[0]; i < total; i++)
            neurons[i].Compute();
    }
    //Vai buscar os valores da layer de outputs
    public float[] Get_Output()
    {
        //Criar um array com os ultimos resultados
        var res = new float[layers[layers.Length - 1]];
        //Preencher com a layer de output
        for (int i = total - layers[layers.Length - 1]; i < total; i++)
            res[i - total + layers[layers.Length - 1]] = (float)neurons[i].Get_Value();

        return res;
    }
    //Print dos neuronios
    public void Print()
    {
        foreach (var n in neurons)
            n.Print();
    }
}
