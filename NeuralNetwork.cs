using System;
using System.Collections.Generic;

namespace AI
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }

        public double[] Inputs
        {
            get
            {
                return Layers[0].Outputs;
            }
            set
            {
                Layers[0].Outputs = value;
            }
        }

        public double[] Outputs
        {
            get
            {
                return Layers[Layers.Count - 1].Outputs;
            }
            set
            {
                Layers[Layers.Count - 1].Outputs = value;
            }
        }

        public NeuralNetwork(List<Layer> layers = null)
        {
            layers ??= new List<Layer>();
            Layers = layers;
        }

        public NeuralNetwork(List<int> structure, ActivationTypes activationType)
        {
            List<ActivationTypes> _activationTypes = new();

            for (int i = 0; i < structure.Count; i++)
            {
                if (i == 0)
                {
                    _activationTypes.Add(ActivationTypes.LINEAR);
                }
                else
                {
                    _activationTypes.Add(activationType);
                }
            }

            Build(structure, _activationTypes);
        }

        public NeuralNetwork(List<int> structure, List<ActivationTypes> activationTypes)
        {
            Build(structure, activationTypes);
        }

        private void Build(List<int> structure, List<ActivationTypes> activationTypes)
        {
            if (activationTypes.Count != structure.Count)
            {
                throw new Exception("First dimension of activationTypes must be equal to the number of layers");
            }

            Layers = new List<Layer>();

            Layers.Add(new Layer(structure[0], 0, activationTypes[0]));

            for (int i = 1; i < structure.Count; i++)
            {
                Layers.Add(new Layer(structure[i], structure[i - 1], activationTypes[i]));
            }
        }

        public double[] Update()
        {
            // Skip input layer
            for (int i = 1; i < Layers.Count; i++)
            {
                Layers[i].Update(Layers[i - 1].Outputs);
            }

            return Outputs;
        }

        public NeuralNetwork BackPropagate(double[,][] patterns, int epoches, double learningRate)
        {
            for (int epoch = 0; epoch < epoches; epoch++)
            {
                for (int i = 0; i < Layers.Count; i++)
                {
                    Layers[i].DNodes.Clear();
                    Layers[i].DWeights.Clear();
                    Layers[i].DeltaNodes.Clear();
                }

                for (int pattern = 0; pattern < patterns.GetLength(0); pattern++)
                {
                    Inputs = patterns[pattern, 0];
                    double[] outputs = Update();

                    double[] errors = new double[outputs.Length];
                    double[] dErrors = new double[outputs.Length];

                    for (int i = 0; i < outputs.GetLength(0); i++)
                    {
                        errors[i] = Math.Pow(outputs[i] - patterns[pattern, 1][i], 2) / 2;
                        dErrors[i] = outputs[i] - patterns[pattern, 1][i];
                    }

                    for (int i = Layers.Count - 1; i >= 0; i--)
                    {
                        Layers[i].DWeights.Add(new double[Layers[i].NeuronsNumber, Layers[i].LastLayerNeuronsNumber]);
                        Layers[i].DeltaNodes.Add(new double[Layers[i].NeuronsNumber]);

                        for (int j = 0; j < Layers[i].NeuronsNumber; j++)
                        {
                            if (i == Layers.Count - 1)
                            {
                                Layers[i].DeltaNodes[pattern][j] = dErrors[j] * Layers[i].DNodes[pattern][j];
                            }
                            else if (i != 0)
                            {
                                double sum = 0.0;

                                for (int k = 0; k < Layers[i + 1].NeuronsNumber; k++)
                                {
                                    sum += Layers[i + 1].DeltaNodes[pattern][k] * Layers[i + 1].Weights[k, j];
                                }

                                Layers[i].DeltaNodes[pattern][j] = sum * Layers[i].DNodes[pattern][j];
                            }

                            if (i != 0)
                            {
                                for (int k = 0; k < Layers[i].LastLayerNeuronsNumber; k++)
                                {
                                    Layers[i].DWeights[pattern][j, k] = Layers[i].DeltaNodes[pattern][j] * Layers[i - 1].Outputs[k];
                                }
                            }
                        }
                    }
                }

                for (int pattern = 0; pattern < patterns.GetLength(0); pattern++)
                {
                    for (int i = 1; i < Layers.Count; i++)
                    {
                        for (int j = 0; j < Layers[i].NeuronsNumber; j++)
                        {
                            for (int k = 0; k < Layers[i].LastLayerNeuronsNumber; k++)
                            {
                                Layers[i].Weights[j, k] -= Layers[i].DWeights[pattern][j, k] * learningRate;
                            }

                            Layers[i].Biases[j] -= Layers[i].DeltaNodes[pattern][j] * learningRate;
                        }
                    }
                }

                if (epoch % 100 == 0)
                {
                    //Console.WriteLine(Outputs[0]);
                    //Console.WriteLine();
                }
            }

            Console.WriteLine("Finished");

            return this;
        }

        public override string ToString()
        {
            string str = "";

            foreach (Layer layer in Layers)
            {
                str += layer + "\n\n";
            }
            return str;
        }

        public class Layer
        {
            public double[,] Weights { get; set; }
            public double[] Biases { get; set; }
            public double[] Outputs { get; set; }
            public List<double[]> DNodes { get; set; }
            public List<double[,]> DWeights { get; set; }
            public List<double[]> DeltaNodes { get; set; }
            ActivationTypes ActivationType { get; set; }
            public int NeuronsNumber
            {
                get
                {
                    return Weights.GetLength(0);
                }
            }
            public int LastLayerNeuronsNumber
            {
                get
                {
                    return Weights.GetLength(1);
                }
            }

            public Layer(int neuronsNumber, int lastLayerNeuronsNumber, ActivationTypes activationType)
            {
                Weights = new double[neuronsNumber, lastLayerNeuronsNumber];
                Biases = new double[neuronsNumber];
                Outputs = new double[neuronsNumber];
                DNodes = new();
                DWeights = new();
                DeltaNodes = new();

                Random random = new Random();

                for (int i = 0; i < neuronsNumber; i++)
                {
                    Biases[i] = 0.0;
                    for (int j = 0; j < lastLayerNeuronsNumber; j++)
                    {
                        Weights[i, j] = random.NextDouble();
                    }
                }

                ActivationType = activationType;
            }

            public double[] Update(double[] inputs)
            {
                if (inputs.GetLength(0) != LastLayerNeuronsNumber)
                {
                    throw new Exception("Inputs number must be equal to LastLayerNeuronsNumber");
                }

                Array.Clear(Outputs, 0, Outputs.Length);

                DNodes.Add(new double[NeuronsNumber]);

                for (int i = 0; i < NeuronsNumber; i++)
                {
                    Outputs[i] = 0.0;
                    for (int j = 0; j < LastLayerNeuronsNumber; j++)
                    {
                        Outputs[i] += inputs[j] * Weights[i, j];
                    }
                    Outputs[i] += Biases[i];

                    Outputs[i] = Activate(Outputs[i]);

                    DNodes[DNodes.Count - 1][i] = Outputs[i] * (1 - Outputs[i]);
                }

                return Outputs;
            }

            public double Activate(double value)
            {
                switch (ActivationType)
                {
                    case ActivationTypes.LINEAR: return Activation.Linear.Default(value);
                    case ActivationTypes.SIGMOID: return Activation.Sigmoid.Default(value); ;
                    case ActivationTypes.TANH: return Activation.TanH.Default(value);
                    case ActivationTypes.RELU: return Activation.ReLU.Default(value);
                    default: throw new NotImplementedException();
                }
            }

            public void AddNoise()
            {
                Random random = new Random();

                for (int i = 0; i < NeuronsNumber; i++)
                {
                    Biases[i] += random.NextDouble() * 2 - 1;
                    for (int j = 0; j < LastLayerNeuronsNumber; j++)
                    {
                        Weights[i, j] += random.NextDouble() * 2 - 1;
                    }
                }
            }

            public override string ToString()
            {
                string str = "";

                for (int i = 0; i < NeuronsNumber; i++)
                {
                    string _weights = "(";
                    for (int j = 0; j < LastLayerNeuronsNumber; j++)
                    {
                        _weights += $"{Weights[i, j]}, ";
                    }
                    _weights += ")";

                    str += $"({_weights}, {Biases[i]}, {Outputs[i]})\n";
                }

                return str;
            }
        }

        public class Activation
        {
            public static class Linear
            {
                public static double Default(double value)
                {
                    return value;
                }

                public static double Derivative(double value)
                {
                    return 1.0;
                }
            }

            public static class Sigmoid
            {
                public static double Default(double value)
                {
                    return 1.0 / (1.0 + Math.Exp(-value));
                }

                public static double Derivative(double value)
                {
                    return Math.Exp(-value) / Math.Pow(1.0 + Math.Exp(-value), 2.0);
                }
            }

            public static class TanH
            {
                public static double Default(double value)
                {
                    return Math.Tanh(value);
                }

                public static double Derivative(double value)
                {
                    return 1.0 - Math.Pow(Math.Tanh(value), 2);
                }
            }

            public static class ReLU
            {
                public static double Default(double value)
                {
                    return Math.Max(0, value);
                }

                public static double Derivative(double value)
                {
                    if (value >= 0.0)
                    {
                        return 1.0;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            }
        }

        public enum ActivationTypes
        {
            LINEAR,
            SIGMOID,
            TANH,
            RELU
        }
    }
}
