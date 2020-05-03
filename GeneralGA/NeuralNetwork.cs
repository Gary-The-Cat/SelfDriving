using System;
using System.Linq;
using System.Collections.Generic;

namespace GeneralGA
{
    public class MLPNeuralNetwork
    {
        // Layer length information
        private int[] layer;

        // Layers in the network
        private Layer[] layers;

        /// <summary>
        /// MLP - Multi Later Perceptron Neural Network (very basic)
        /// Constructor setting up layers
        /// </summary>
        public MLPNeuralNetwork(int[] layer)
        {
            // Deep copy layers
            this.layer = new int[layer.Length];
            for (int i = 0; i < layer.Length; i++)
                this.layer[i] = layer[i];

            // Create neural layers
            layers = new Layer[layer.Length - 1];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i], layer[i + 1]);
            }
        }

        /// <summary>
        /// Feed the set of inouts through all layers.
        /// </summary>
        public float[] FeedForward(float[] inputs)
        {
            // Feed the input through each layer.
            layers[0].FeedForward(inputs);
            for (int i = 1; i < layers.Length; i++)
            {
                layers[i].FeedForward(layers[i - 1].GetOutputs());
            }

            // Return the classification made by the network.
            return layers[layers.Length - 1].GetOutputs();
        }

        /// <summary>
        /// High level back porpagation
        /// Back Propagation must be performed after a feed forward as nodes retain their feed forward values.
        /// </summary>
        /// <param name="expected">The expected output form the last feedforward</param>
        public void BackPropagate(float[] expected)
        {
            // Propogation is dont iteratively, backwards as we have known expected output nodes.
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1)
                {
                    // Back propogate the expected output value
                    layers[i].BackPropOutput(expected);
                }
                else
                {
                    // Back propogate the hidden layers based off the adjustment made from the expected output.
                    layers[i].BackPropHidden(layers[i + 1].GetGamma(), layers[i + 1].GetWeights());
                }
            }

            // Update weights
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].UpdateWeights();
            }
        }

        public void Train(List<float[]> inputData, List<float[]> expectedOutput)
        {
            // Using fixed number of training iterations is kind of a lazy approach, should really
            // implement some system that runs until we converge to an acceptable error value.
            for (int j = 0; j < inputData.Count; j++)
            {
                this.FeedForward(inputData[j]);
                this.BackPropagate(expectedOutput[j]);
            }
        }

        public float[] GetFlattenedWeights()
        {
            return layers.SelectMany(l => l.GetFlattenedWeights()).ToArray();
        }

        public void UpdateNetworkWeights(List<float> newWeights)
        {
            int currentPosition = 0;
            foreach(var layer in layers)
            {
                var totalLayerWeights = layer.GetFlattenedWeights().Count();
                var newLayerWeights = newWeights.GetRange(currentPosition, totalLayerWeights);
                layer.UpdateWeightsFromList(newLayerWeights);
            }
        }
    }
}
