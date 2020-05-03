using CarSimulation.DataStructures;
using CarSimulation.Helpers;
using System.IO;
using System.Linq;

namespace Arkanoid_SFML.Helpers
{
    public static class ParameterParser
    {
        public static Parameters GetSimulationParameters(string[] args)
        {
            var argsCount = args.Count();
            var parameters = new Parameters();
            var currentDirectory = Directory.GetCurrentDirectory();

            parameters.Map = LoadHelper.LoadMap($"{currentDirectory}\\{args[0]}\\Map");

            parameters.StartPosition = LoadHelper.LoadStartPosition($"{currentDirectory}\\{args[0]}\\StartPosition");

            parameters.Checkpoints = LoadHelper.LoadCheckpoints($"{currentDirectory}\\{args[0]}\\Checkpoints");
            
            if (argsCount > 1)
            {
                parameters.StartWeights = LoadHelper.LoadStartWeights($"{currentDirectory}\\{args[1]}");
            }

            return parameters;
        }
    }
}
