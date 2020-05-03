using GeneralGA;
using System;
using System.Linq;

namespace NeuralNetwork
{
    public class Neighbour : INeighbour
    {
        public Neighbour(int[] networkStructure, Random random)
        {
            this.NetworkStructure = networkStructure;

            this.Network = new MLPNeuralNetwork(networkStructure);

            this.Random = random;
        }

        public int[] NetworkStructure { get; }

        public double Fitness { get; set; }

        public MLPNeuralNetwork Network { get;set; }

        public Random Random { get; set; }

        public INeighbour Clone()
        {
            throw new NotImplementedException();
        }

        public double GetFitness()
        {
            return Fitness;
        }

        public void Mutate()
        {
            // Get the current weights
            var networkWeights = Network.GetFlattenedWeights();

            // Randomly select which one we are going to mutate.
            var index = Random.Next(0, networkWeights.Length);

            // Perform the mutation (this should probably include some annealing)
            networkWeights[index] += (float)(Random.NextDouble() * 0.2 - 0.1);

            // Update the network weights to reflect the mutation.
            Network.UpdateNetworkWeights(networkWeights.ToList());
        }
    }
}
