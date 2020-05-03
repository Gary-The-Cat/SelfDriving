using Arkanoid_SFML;
using Arkanoid_SFML.Agents.Interfaces;
using Arkanoid_SFML.DataStructures;
using GeneralGA;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarSimulation.Agents
{
    public class CarAi : ICarAI
    {
        public CarAi(int[] networkStructure, Random random, List<float> initialWeights)
        {
            this.networkStructure = networkStructure;

            this.Network = new MLPNeuralNetwork(networkStructure);

            if (initialWeights != null)
            {
                this.Network.UpdateNetworkWeights(initialWeights);
            }

            this.Random = random;
        }

        public double Fitness { get; set; } = -1;

        public double GetFitness() => Fitness;

        public MLPNeuralNetwork Network { get; set; }

        public Random Random { get; set; }

        private int[] networkStructure { get; }

        private Vector2f previousCheckpoint;

        private Vector2f previousCarPosition;
        
        public DrivingAction GetOutput(
            float[] rayCollisions,
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition)
        {
            var networkInput = new float[12];
            for (int i = 0; i < 7; i++)
            {
                networkInput[i] = rayCollisions[i];
            }

            var checkpointDelta = nextCheckpointPosition - carPosition;
            networkInput[7] = checkpointDelta.X / Configuration.Width;
            networkInput[8] = checkpointDelta.Y / Configuration.Height;

            var positionDelta = carPosition - previousCarPosition;
            networkInput[9] = positionDelta.X / Configuration.Width;
            networkInput[10] = positionDelta.Y / Configuration.Height;

            var angleToCheckpoint = Math.Atan2(checkpointDelta.Y, checkpointDelta.X);
            var angleDelta = (angleToCheckpoint - carHeading) / (2 * Math.PI);

            networkInput[11] = (float)angleDelta;

            previousCarPosition = carPosition;

            var networkOutput = Network.FeedForward(networkInput).Select(f => (f + 1) / 2).ToArray();

            return new DrivingAction
            {
                Acceleration = networkOutput[0],
                LeftTurnForce = networkOutput[1],
                RightTurnForce = networkOutput[2],
                BreakingForce = networkOutput[3]
            };
        }

        public INeighbour Clone()
        {
            return new CarAi(this.networkStructure, this.Random, this.Network.GetFlattenedWeights().ToList());
        }

        public void Mutate()
        {
            // Get the current weights
            var networkWeights = Network.GetFlattenedWeights();

            for(int i = 0; i < networkWeights.Length; i++)
            {
                if(Random.NextDouble() < 0.01)
                {
                    // Randomly select which one we are going to mutate.
                    var index = Random.Next(0, networkWeights.Length);

                    // Perform the mutation (this should probably include some annealing)
                    networkWeights[index] += (float)(Random.NextDouble() * 0.2 - 0.1);
                }
            }

            // Update the network weights to reflect the mutation.
            Network.UpdateNetworkWeights(networkWeights.ToList());
        }

        bool isTimeAlive = true;
        bool isTotalDistance = true;
        bool isCheckpointsPassed = true;

        public void IncrementFitness(Car car)
        {
            var timeAlive = isTimeAlive ? car.TimeAlive * 60 : 0;
            var distance = isTotalDistance ? car.TotalDistance * 0.2f : 0;
            var checkpoints = isCheckpointsPassed ? car.CheckpointsPassed * 1000 : 0;
            Fitness = timeAlive + distance + checkpoints;
        }

        public void Initalize(Car car)
        {
            previousCarPosition = car.Position;

            previousCheckpoint = car.CheckpointManager.CurrentWaypoint;
        }
    }
}
