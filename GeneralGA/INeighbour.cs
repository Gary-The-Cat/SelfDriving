using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralGA
{
    public interface INeighbour
    {
        MLPNeuralNetwork Network { get; set; }

        void Mutate();

        double GetFitness();

        INeighbour Clone();
    }
}
