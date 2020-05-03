using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkanoid_SFML.Agents.Interfaces;
using Arkanoid_SFML.DataStructures;
using CarSimulation;
using CarSimulation.Agents;
using GeneralGA;
using SFML.System;
using SFML.Window;

namespace Arkanoid_SFML.Agents
{
    public class HumanCar : ICarAI
    {
        public MLPNeuralNetwork Network { get; set; }

        public void Mutate()
        {
            throw new NotImplementedException();
        }

        public double GetFitness() => this.Fitness;

        public INeighbour Clone()
        {
            throw new NotImplementedException();
        }

        public DrivingAction GetOutput(
            float[] rayCollisions, 
            Vector2f carPosition,
            float carHeading,
            Vector2f nextCheckpointPosition)
        {
            var output = new DrivingAction();

            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                output.Acceleration = 1;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                output.BreakingForce = 1;
            }

            output.LeftTurnForce = Keyboard.IsKeyPressed(Keyboard.Key.Left) ? 1 : 0;
            output.RightTurnForce = Keyboard.IsKeyPressed(Keyboard.Key.Right) ? 1 : 0;

            return output;
        }

        public void Initalize(Car car)
        {

        }

        public void IncrementFitness(Car car)
        {

        }

        public double Fitness { get; set; }
    }
}
