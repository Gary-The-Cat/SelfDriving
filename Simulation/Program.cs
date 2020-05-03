// Application Entry Point

using Arkanoid_SFML.Helpers;
using System.Linq;

namespace CarSimulation
{
    class Program
    {

        static void Main(string[] args)
        {
            var mapMaker = new MapMaker(ParameterParser.GetSimulationParameters(args));

            mapMaker.Run();
        }
    }
}
