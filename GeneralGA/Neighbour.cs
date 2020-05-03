using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralGA
{
    public class Neighbour : INeighbour
    {
        public MLPNeuralNetwork Network { get; set; }

        public static List<float[]> TrainData { get; set; }

        public static List<float[]> ReinforcementValues { get; set; }

        public static List<float[]> TestData { get; set; }

        public static List<float[]> ExpectedTestValues { get; set; }

        private Random random;
        private int inputNodes;
        private int outputNodes;
        private int maxLayers;
        private int maxInnerNodes;
        private int[] inputNodesEnabled;
        private int[] layers;

        public Neighbour(
            Random random,
            int inputNodes, 
            int outputNodes, 
            int maxLayers, 
            int maxInnerNodes)
        {
            this.random = random;
            this.inputNodesEnabled = new int[inputNodes];

            for(int i = 0; i < inputNodes; i++)
            {
                if(random.NextDouble() > 0.5)
                {
                    this.inputNodesEnabled[i] = 1;
                    this.inputNodes++;
                }
            }

            this.outputNodes = outputNodes;
            this.maxLayers = maxLayers;
            this.maxInnerNodes = maxInnerNodes;

            var innerLayers = random.Next(maxLayers);
            layers = new int[innerLayers + 2];

            layers[0] = this.inputNodes;
            for(int i = 1; i < innerLayers+1; i++)
            {
                layers[i] = random.Next(maxInnerNodes);
            }
            layers[innerLayers + 1] = outputNodes;

            Network = new MLPNeuralNetwork(layers);
        }

        public Neighbour(
            Random random,
            int inputNodes,
            int outputNodes,
            int maxLayers,
            int maxInnerNodes,
            int[] layers)
        {
            this.random = random;
            this.inputNodes = inputNodes;
            this.outputNodes = outputNodes;
            this.maxLayers = maxLayers;
            this.maxInnerNodes = maxInnerNodes;
            this.layers = layers;
            
            Network = new MLPNeuralNetwork(layers);
        }

        public void Mutate()
        {
            for(int i = 1; i < layers.Count() - 1; i++)
            {
                if (random.NextDouble() > 0.5)
                {
                    var increment = random.NextDouble() > 0.5 ? 1 : -1;
                    if(increment == -1 && layers[i] > 1)
                    {
                        layers[i] += increment;
                    }
                    else if(layers[i] < maxInnerNodes)
                    {
                        layers[i] += increment;
                    }
                }
            }
            
            //return new Neighbour(
            //    random,
            //    inputNodes,
            //    outputNodes,
            //    maxLayers,
            //    maxInnerNodes,
            //    layers);
        }

        public double GetFitness()
        {
            double error = 0;

            try
            {
                var tempTrainData = new List<float[]>();

                foreach (var data in TrainData)
                {
                    var tempTrainArray = new float[this.inputNodesEnabled.Where(n => n == 1).Count()];
                    int i = 0;

                    for (int c = 0; c < this.inputNodesEnabled.Length; c++)
                    {
                        if (this.inputNodesEnabled[c] == 1)
                        {
                            tempTrainArray[i] = data[c];
                            i++;
                        }
                    }
                    tempTrainData.Add(tempTrainArray);
                }

                var tempTestData = new List<float[]>();

                foreach (var data in TrainData)
                {
                    var tempTestArray = new float[this.inputNodesEnabled.Where(n => n == 1).Count()];
                    int i = 0;

                    for (int c = 0; c < this.inputNodesEnabled.Length; c++)
                    {
                        if (this.inputNodesEnabled[c] == 1)
                        {
                            tempTestArray[i] = data[c];
                            i++;
                        }
                    }
                    tempTestData.Add(tempTestArray);
                }

                // Perform training
                for (int i = 0; i < 2000; i++)
                {
                    for (int j = 0; j < tempTrainData.Count; j++)
                    {
                        Network.FeedForward(tempTrainData[j]);
                        Network.BackPropagate(ReinforcementValues[j]);
                    }
                }

                // Perform testing + additional correction
                for (int j = 0; j < tempTestData.Count; j++)
                {
                    var prediction = Network.FeedForward(tempTestData[j]);
                    error += GetPredictionError(prediction, ReinforcementValues[j]);
                    Network.BackPropagate(ReinforcementValues[j]);
                }
                return error;
            }

        
            catch
            {
                return 0;
            }
            
        }

        private double GetPredictionError(float[] prediction, float[] expectedValue)
        {
            double error = 0;

            for (int i = 0; i < prediction.Count(); i++)
            {
                if (expectedValue[i] == 1)
                {
                    error += expectedValue[i] - prediction[i];
                }
                else if (prediction[i] > 0)
                {
                    error += prediction[i];
                }
            }

            return error;
        }

        public INeighbour Clone()
        {
            throw new NotImplementedException();
        }
    }
}
