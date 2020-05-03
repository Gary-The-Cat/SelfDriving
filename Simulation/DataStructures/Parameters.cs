using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace CarSimulation.DataStructures
{
    public struct Parameters
    {
        public Parameters(
            List<Vertex[]> map, 
            List<Vector2f> checkpoints, 
            Vector2f? startPosition, 
            List<List<float>> startWeights)
        {
            Map = map;
            StartWeights = startWeights;
            Checkpoints = checkpoints;
            StartPosition = startPosition;
        }

        public List<Vertex[]> Map { get; set; }

        public Vector2f? StartPosition { get; set; }

        public List<List<float>> StartWeights { get; set; }

        public List<Vector2f> Checkpoints { get; set; }
    }
}
