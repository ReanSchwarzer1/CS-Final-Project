﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Structure: MonoBehaviour, IComparable<Structure>
{
    private int[] LAYER_SIZES = new int[] { 4, 7, 4 };
    private int GENOME_LENGTH = 71;
    private const string WEIGHTS_PATH = "Assets/Scripts/NN-Weights/";
    private string SAVE_PATH = WEIGHTS_PATH + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + ".txt";

    private List<float> genome;
    private CarController car;
    private NeuralNetwork neuralNetwork;

    public Structure(List<float> _genome)
    {
        genome = _genome;
        GameObject instance = Resources.Load("HatchBack") as GameObject;
        car = (Instantiate(instance)).GetComponent<CarController>();
        GameObject startingLine = GameObject.Find("Starting Line");
        car.transform.position = startingLine.gameObject.transform.position;
        neuralNetwork = new NeuralNetwork(LAYER_SIZES);

        if (genome.Count != GENOME_LENGTH)
        {
            for (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(UnityEngine.Random.Range(-0.5f, 0.5f));
            }
        }
    }

    #region Getters/Setters
    public CarController GetCar()
    {
        return car;
    }

    public bool IsAlive()
    {
        if (car.hitWall)
        {
            return false;
        }

        return true;
    }

    public int GetFitness()
    {
        return car.GetFitness();
    }
    #endregion

    public void Mutate(int mutationRate, float mutationRadius)
    {
        for (int i = 0; i < genome.Count; i++)
        {
            genome[i] = (UnityEngine.Random.Range(0, 100) <= mutationRate) ? genome[i] += UnityEngine.Random.Range(-mutationRadius, mutationRadius) : genome[i];
        }
    }

    public List<float> deepCopyGenome()
    {
        List<float> genomeCopy = new List<float>();
        for (int i = 0; i < genome.Count; i++)
        {
            genomeCopy.Add(genome[i]);
        }

        return genomeCopy;
    }

    public void Evaluate()
    {
        neuralNetwork.ConfigureNeuralNetwork(genome);
        car.SetNeuralNetwork(neuralNetwork);
    }

    public int CompareTo(Structure other)
    {
        if (car.GetFitness() > other.car.GetFitness())
        {
            return 1;
        }
        else if (car.GetFitness() < other.car.GetFitness())
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }


    #region Load/Save genomes from a file
    public void LoadGenomeFromFile(string fileName)
    {
        string filePath = WEIGHTS_PATH + fileName;

        TextReader tr = new StreamReader(filePath);
        int NumberOfLines = (int)new FileInfo(filePath).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();

        genome = new List<float>();
        if (new FileInfo(filePath).Length > 0)
        {
            for  (int i = 0; i < GENOME_LENGTH; i++)
            {
                genome.Add(float.Parse(ListLines[index]));
                index++;
            }
        }
    }

    public void SaveGenomeToFile()
    {
        File.Create(SAVE_PATH).Close();
        StreamWriter writer = new StreamWriter(SAVE_PATH, true);

        for (int i = 0; i < GENOME_LENGTH; i++)
        {
            writer.WriteLine(genome[i]);
        }
        
        writer.Close();
    }
    #endregion
}
