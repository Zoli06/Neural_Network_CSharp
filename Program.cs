﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AI
{
    class Program
    {
        static void Main(string[] args)
        {
            VectorBuilder<double> _v = Vector<double>.Build;

            #region
            //List<int> structure = new() { 2, 2, 4 };

            //// First output: xor
            //// Second output: xnor
            //// Third output: and
            //// Fourth output: or
            //Vector<double>[,] patterns =
            //{
            //    { _v.DenseOfArray(new double[] { 0, 1 }), _v.DenseOfArray(new double[] { 1, 0, 0, 1 }) },
            //    { _v.DenseOfArray(new double[] { 1, 0 }), _v.DenseOfArray(new double[] { 1, 0, 0, 1 }) },
            //    { _v.DenseOfArray(new double[] { 1, 1 }), _v.DenseOfArray(new double[] { 0, 1, 1, 1 }) },
            //    { _v.DenseOfArray(new double[] { 0, 0 }), _v.DenseOfArray(new double[] { 0, 1, 0, 0 }) },
            //};

            ////NeuralNetwork nn = new("D:/Users/zolix/Downloads/export.nns");
            //NeuralNetwork nn = new(structure, NeuralNetwork.ActivationType.TANH);

            //Stopwatch sw = Stopwatch.StartNew();
            ////nn.BackPropagateOffline(patterns, .2, 0, 2000);
            //nn.BackPropagateOnline(patterns, .2, 2, .2, 100000);
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            //Console.WriteLine("Outputs\n");

            //for (int i = 0; i < patterns.GetLength(0); i++)
            //{
            //    nn.Update(patterns[i, 0]);
            //    Console.WriteLine("Input:");
            //    for (int j = 0; j < patterns[i, 0].Count; j++)
            //    {
            //        Console.WriteLine(patterns[i, 0][j]);
            //    }
            //    Console.WriteLine("Output:");
            //    for (int j = 0; j < nn.Outputs.Count; j++)
            //    {
            //        Console.WriteLine(nn.Outputs[j]);
            //    }
            //    Console.WriteLine();
            //}

            //foreach (var image in MnistReader.ReadTestData())
            //{
            //    for (int i = 0; i < image.Data.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < image.Data.GetLength(1); j++)
            //        {
            //            if (image.Data[j, i] > 10)
            //            {
            //                Console.Write('@');
            //            }
            //            else
            //            {
            //                Console.Write('.');
            //            }
            //        }
            //        Console.WriteLine();
            //    }
            //    Console.WriteLine();
            //}
            #endregion

            //IEnumerable<Image> mnistTrainingSet = MnistReader.ReadTestData();

            //Console.WriteLine("Readed training set");

            //Vector<double>[,] mnistFormattedTrainingSet = new Vector<double>[mnistTrainingSet.Count(), 2];

            //int counter = 0;
            //foreach (var image in mnistTrainingSet)
            //{
            //    // Flatten array
            //    mnistFormattedTrainingSet[counter, 0] = _v.DenseOfArray(image.Data.Cast<double>().ToArray()) / 255;

            //    Vector<double> expectedOutput = _v.Dense(10);
            //    expectedOutput[image.Label] = 1.0;

            //    mnistFormattedTrainingSet[counter, 1] = expectedOutput;

            //    counter++;
            //}

            //Console.WriteLine("Formatted training set");

            //NeuralNetwork nn = new(new List<int> { 784, 128, 10 }, NeuralNetwork.ActivationType.SIGMOID);

            //Console.WriteLine(nn.Update(mnistFormattedTrainingSet[100, 0]));

            //nn.BackPropagateOnline(mnistFormattedTrainingSet, .05, 2500, 0, 12);

            //Console.WriteLine(nn.Update(mnistFormattedTrainingSet[100, 0]));

            //Console.WriteLine("Backpropagation Finished");

            //mnistTrainingSet = MnistReader.ReadTestData();

            //Console.WriteLine("Readed testing set");

            //int success = 0;
            //counter = 0;
            //foreach (var image in mnistTrainingSet)
            //{
            //    Vector<double> outputs = nn.Update(_v.DenseOfArray(image.Data.Cast<double>().ToArray()) / 255);
            //    int result = outputs.MaximumIndex();

            //    //Console.WriteLine(outputs);
            //    //Console.WriteLine(image.Label);
            //    //Console.WriteLine(result);
            //    //Console.WriteLine();

            //    if (result == image.Label)
            //    {
            //        success++;
            //    }

            //    counter++;
            //}

            //Console.WriteLine(counter + "/" + success);
            //Console.WriteLine("Finished");
            //nn.Export("c:/asd/export.nns");

            for (int i = 0; i < 8; i += 1)
            {
                Thread thr1 = new Thread(() => Run(i));
                Thread thr2 = new Thread(() => Run(i+1));
                thr1.Start();
                //thr2.Start();

                thr1.Join();
                //thr2.Join();

                Console.WriteLine(i + " finished");
            }
        }

        static public void Run(int num)
        {
            VectorBuilder<double> _v = Vector<double>.Build;

            IEnumerable<Image> mnistTrainingSet = MnistReader.ReadTrainingData();

            //Console.WriteLine("Readed training set");

            Vector<double>[,] mnistFormattedTrainingSet = new Vector<double>[mnistTrainingSet.Count(), 2];

            int counter = 0;
            foreach (var image in mnistTrainingSet)
            {
                // Flatten array
                mnistFormattedTrainingSet[counter, 0] = _v.DenseOfArray(image.Data.Cast<double>().ToArray()) / 255;

                Vector<double> expectedOutput = _v.Dense(10);
                expectedOutput[image.Label] = 1.0;

                mnistFormattedTrainingSet[counter, 1] = expectedOutput;

                counter++;
            }

            //Console.WriteLine("Formatted training set");

            NeuralNetwork nn = new(new List<int> { 784, 128, 10, 10 }, new List<NeuralNetwork.Layer.ActivationType> { NeuralNetwork.Layer.ActivationType.LINEAR, NeuralNetwork.Layer.ActivationType.TANH, NeuralNetwork.Layer.ActivationType.TANH, NeuralNetwork.Layer.ActivationType.SIGMOID });

            //Console.WriteLine(nn.Update(mnistFormattedTrainingSet[100, 0]));

            nn.BackPropagateOnline(mnistFormattedTrainingSet, 3, 2000, 0, 160);

            //Console.WriteLine(nn.Update(mnistFormattedTrainingSet[100, 0]));

            //Console.WriteLine("Backpropagation Finished");

            mnistTrainingSet = MnistReader.ReadTestData();

            //Console.WriteLine("Readed testing set");

            int success = 0;
            counter = 0;
            foreach (var image in mnistTrainingSet)
            {
                Vector<double> outputs = nn.Update(_v.DenseOfArray(image.Data.Cast<double>().ToArray()) / 255);
                int result = outputs.MaximumIndex();

                //Console.WriteLine(outputs);
                //Console.WriteLine(image.Label);
                //Console.WriteLine(result);
                //Console.WriteLine();

                if (result == image.Label)
                {
                    success++;
                }

                counter++;
            }

            Console.WriteLine(num + ": " + counter + "/" + success);
            //Console.WriteLine("Finished");
            nn.Export("c:/asd/export" + num + ".nns");
        }
    }
}