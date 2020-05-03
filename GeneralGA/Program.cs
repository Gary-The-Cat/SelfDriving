using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GeneralGA
{
    class Program
    {
        static void Main(string[] args)
        {
            // Extract training data            
            var data = File.ReadAllText("C:\\Dev\\data.csv");

            var jobData = data.Split('\n').Select(d => d.Trim());
            jobData = jobData.Take(jobData.Count() - 1);
            // Holds the extracted flowchart data.
            var trainData = new List<float[]>();

            // Holds the known classification / reinforcement values.
            var reinforceValues = new List<float[]>();

            // We only want to include distinct jobs to not skew our NN.
            var existingJobs = new HashSet<Tuple<float, float, float, float, float>>();

            // Iterate over the jobs and create the training & reinforcement data.
            foreach (var job in jobData)
            {
                // We can safely parse all the values to floats.
                var values = job.Split(',').Select(v => float.Parse(v)).ToArray();

                // To mitigate skewing the NN from duplicate jobs, we need to use some indicator as a unique identifier.
                // Population, ParcelCount, ItemCount, ModelSolidCount, StockpileSolidCount
                var jobTuple = new Tuple<float, float, float, float, float>(values[0], values[1], values[2], values[3], values[6]);
                if (!existingJobs.Contains(jobTuple))
                {
                    existingJobs.Add(jobTuple);

                    // The last two values are our RAM & CPU usage, so dont include them.
                    trainData.Add(values.Take(values.Count() - 2).ToArray());

                    // Populate the reinforcenemt values by binning the RAM usage.
                    reinforceValues.Add(GetRamArray(values[14]));
                }
            }

            // Define the layour for our NN.
            // -- Input Nodes 13 
            // (population, parcelCount, itemCount, modelSolidCount, pitCount, stockpileCount, stockpileSolidCount, 
            //  diggerCount, blend, npv, isSeeded, periodCount, seedPeriods, RAM, CPU_Usage)
            // -- 1 Hidden Layer. 13 + 2/3 * 5 = 16 nodes
            // -- 1 Output Layer. 4 Nodes (0-16, 16-32, 32-64, 64-128)
            var inputNodes = 13;
            var outputNodes = 4;
            var innerLayers = 1;
            var hiddenLayerDepth = 16;

            // Inner Layer + Input + Output = 3
            var layers = new int[innerLayers + 2];

            layers[0] = inputNodes;
            for (int i = 1; i < innerLayers + 1; i++)
            {
                layers[i] = hiddenLayerDepth;
            }
            layers[innerLayers + 1] = outputNodes;

            // Create the NN.
            var net = new MLPNeuralNetwork(layers);

            // Train the network with our known good data.
            net.Train(trainData, reinforceValues);

            // Lets test out network with a known job.
            var newJob = new float[] { 300, 7, 13, 4083, 3, 0, 0, 3, 1, 0, 1, 25, 6 };
            var jobTest = net.FeedForward(newJob);

            // Classify the result into the correct bin.
            var classification = Classify(jobTest.ToList());
        }

        private static float[] GetRamArray(float v)
        {
            if(v < 16000)
            {
                return new float[] { 1, 0, 0, 0 };
            }

            if (v >= 16000 && v < 32000)
            {
                return new float[] { 0, 1, 0, 0 };
            }

            if (v >= 32000 && v < 64000)
            {
                return new float[] { 0, 0, 1, 0 };
            }

            if (v >= 64000)
            {
                return new float[] { 0, 0, 0, 1 };
            }

            throw new Exception("Job could not be classified.");
        }

        private static int Classify(List<float> results)
        {
            return results.IndexOf(results.Max());
        }
    }
}



