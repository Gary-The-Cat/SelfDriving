using Arkanoid_SFML.DataStructures;
using CarSimulation.Agents;
using GeneralGA;
using SFML.System;

namespace Arkanoid_SFML.Agents.Interfaces
{
    public interface ICarAI : INeighbour
    {
        void Initalize(Car car);

        DrivingAction GetOutput(
            float[] rayCollisions,
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition);

        double Fitness { get; set; }

        void IncrementFitness(Car car);
    }
}
