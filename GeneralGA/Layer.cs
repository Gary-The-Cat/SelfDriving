using System;
using System.Collections.Generic;

namespace GeneralGA
{
    /// <summary>
    /// Represents a single 1D layer in a MLP Neural Network
    /// </summary>
    public class Layer
    {
        // Number of neurons in the previous layer feeding into this layer.
        private int numberOfInputs;  

        // Number of neurons in the current layer.
        private int numberOfOuputs;

        // Float is well and truely precice enough.
        private float[] outputs;
        private float[] inputs;
        private float[,] weights;
        private float[,] weightsDelta; 
        private float[] gamma;
        private float[] error;

        // Used for initial random weights.
        private static Random random = new Random();

        /// <summary>
        /// Constructor initilizes all layer data structures.
        /// </summary>
        /// <param name="numberOfInputs">Number of neurons in the previous layer</param>
        /// <param name="numberOfOuputs">Number of neurons in the current layer</param>
        public Layer(int numberOfInputs, int numberOfOuputs)
        {
            this.numberOfInputs = numberOfInputs;
            this.numberOfOuputs = numberOfOuputs;

            // Initilize all structures
            outputs = new float[numberOfOuputs];
            inputs = new float[numberOfInputs];
            weights = new float[numberOfOuputs, numberOfInputs];
            weightsDelta = new float[numberOfOuputs, numberOfInputs];
            gamma = new float[numberOfOuputs];
            error = new float[numberOfOuputs];

            // Initilize random weights
            InitilizeWeights();
        }

        /// <summary>
        /// Initilize weights between -0.5 and 0.5
        /// </summary>
        public void InitilizeWeights()
        {
            for (int i = 0; i < numberOfOuputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] = (float)random.NextDouble() - 0.5f;
                }
            }
        }

        /// <summary>
        /// Feedforward this layer with a given input
        /// </summary>
        /// <param name="inputs">The output values of the previous layer</param>
        /// <returns></returns>
        public float[] FeedForward(float[] inputs)
        {
            // Update current inputs as 'memory' so they can be used for back propagation
            this.inputs = inputs;

            // Feed forward.
            for (int i = 0; i < numberOfOuputs; i++)
            {
                outputs[i] = 0;
                for (int j = 0; j < numberOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j];
                }

                // TanH is used (purely for laziness and simplicity), 
                // could easily be switched out for any other sigmoid, e.g. (1+e^-v)^-1).
                outputs[i] = (float)Math.Tanh(outputs[i]);
            }

            return outputs;
        }

        /// <summary>
        /// TanH derivate 
        /// </summary>
        /// <param name="value">An already computed TanH value</param>
        public float TanHDer(float value)
        {
            // value = tanH(x)
            // dg/dx = 1 - tanH^2(x)
            return 1 - (value * value);
        }

        /// <summary>
        /// Back propagation for the output layer
        /// </summary>
        /// <param name="expected">The expected output</param>
        public void BackPropOutput(float[] expected)
        {
            // Error dervative of the cost function
            for (int i = 0; i < numberOfOuputs; i++)
            {
                error[i] = outputs[i] - expected[i];
            }

            // Gamma calculation
            for (int i = 0; i < numberOfOuputs; i++)
            {
                gamma[i] = error[i] * TanHDer(outputs[i]);
            }

            // Caluclating detla weights
            for (int i = 0; i < numberOfOuputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        /// <summary>
        /// Back propagation for the hidden layers
        /// </summary>
        /// <param name="gammaForward">the gamma value of the forward layer</param>
        /// <param name="weightsFoward">the weights of the forward layer</param>
        public void BackPropHidden(float[] gammaForward, float[,] weightsFoward)
        {
            // Caluclate new gamma using gamma sums of the forward layer
            for (int i = 0; i < numberOfOuputs; i++)
            {
                gamma[i] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsFoward[j, i];
                }

                gamma[i] *= TanHDer(outputs[i]);
            }

            //Caluclating detla weights
            for (int i = 0; i < numberOfOuputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        /// <summary>
        /// Updating weights
        /// </summary>
        public void UpdateWeights()
        {
            for (int i = 0; i < numberOfOuputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    // Training rate capped at 0.03. Not entirely arbitrary, but could make improvements 
                    // by adding simulated annealing.
                    weights[i, j] -= weightsDelta[i, j] * 0.03f;
                }
            }
        }

        /// <summary>
        /// Get the output later values.
        /// </summary>
        /// <returns></returns>
        public float[] GetOutputs()
        {
            return this.outputs;
        }

        /// <summary>
        /// Get the gamma values for each node in the layer.
        /// </summary>
        /// <returns></returns>
        public float[] GetGamma()
        {
            return this.gamma;
        }

        /// <summary>
        /// Get the weights of the connections between the two layers.
        /// </summary>
        /// <returns></returns>
        public float[,] GetWeights()
        {
            return this.weights;
        }

        public float[] GetFlattenedWeights()
        {
            var weights = new List<float>();
            foreach(var weight in this.weights)
            {
                weights.Add(weight);
            }

            return weights.ToArray();
        }

        public void UpdateWeightsFromList(List<float> newLayerWeights)
        {
            int position = 0;
            for (int x = 0; x < this.weights.GetLength(0); x++)
            {
                for (int y = 0; y < this.weights.GetLength(1); y++)
                {
                    this.weights[x, y] = newLayerWeights[position];
                    position++;
                }
            }
        }
    }
}