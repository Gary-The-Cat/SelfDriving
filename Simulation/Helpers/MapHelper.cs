using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarSimulation.Helpers
{
    public static class LoadHelper
    {
        public static List<Vertex[]> LoadMap(string map)
        {
            var segmentVertices = new List<Vertex[]>();
            if (!File.Exists(map))
            {
                return segmentVertices;
            }

            var segments = File.ReadAllText(map).Split('\n');

            foreach (var segment in segments.Select(s => s.Split(',')))
            {
                var x1 = float.Parse(segment[0]);
                var y1 = float.Parse(segment[1]);
                var x2 = float.Parse(segment[2]);
                var y2 = float.Parse(segment[3]);

                segmentVertices.Add(new Vertex[] { new Vertex(new Vector2f(x1, y1)), new Vertex(new Vector2f(x2, y2)) });
            }

            return segmentVertices;
        }

        public static List<Vector2f> LoadCheckpoints(string checkpointFile)
        {
            var checkpoints = new List<Vector2f>();

            if (!File.Exists(checkpointFile))
            {
                return checkpoints;
            }

            var checkpointLines = File.ReadAllText(checkpointFile).Split('\n');

            foreach(var line in checkpointLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var points = line.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => float.Parse(s.Trim())).ToArray();
                var x = points[0];
                var y = points[1];

                checkpoints.Add(new Vector2f(x, y));
            }

            return checkpoints;
        }

        public static Vector2f? LoadStartPosition(string startPositionFile)
        {
            if (!File.Exists(startPositionFile))
            {
                return null;
            }

            var startPosition = 
                File.ReadAllText(startPositionFile).Split(',')
                .Where(f => float.TryParse(f, out _))
                .Select(f => float.Parse(f)).ToArray();

            return new Vector2f(startPosition[0], startPosition[1]);
        }

        public static List<List<float>> LoadStartWeights(string startWeightsFile)
        {
            if (!File.Exists(startWeightsFile))
            {
                return null;
            }

            var individualWeights = new List<List<float>>();

            var individuals = File.ReadAllText(startWeightsFile).Split('\n');

            foreach(var individual in individuals)
            {
                if(individual == individuals.Last())
                {
                    continue;
                }

                individualWeights.Add(individual.Split(',')
                    .Where(v => float.TryParse(v, out _)).Select(v => float.Parse(v)).ToList());
            }

            return individualWeights;
        }
    }
}
