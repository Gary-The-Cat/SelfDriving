using CarSimulation;
using CarSimulation.Agents;
using CarSimulation.Helpers;
using GeneralGA;
using CarSimulation.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using Arkanoid_SFML.Agents;
using Arkanoid_SFML.Agents.Interfaces;
using System.IO;
using Arkanoid_SFML.Helpers;

namespace NeuralNetwork
{
    class Program
    {
        public static Random Random = new Random();

        private static Simulation simulation;

        /// Arguments
        /// [0] - Map Info Path 
        /// [1] - Map starting position file path
        /// [1] - [Optional] Seed for initial individual state
        static void Main(string[] args)
        {
            var parameters = ParameterParser.GetSimulationParameters(args);

            var ga = new GeneralGA.GeneralGA();

            ga.CreateIndividual = CreateNeighbour;

            ga.CrossoverEnabled = true;

            ga.MutationEnabled = true;

            ga.MutateParentsAsChildren = true;

            ga.CrossoverIndividuals = DoCrossover;

            ga.PopulationCount = Configuration.IsRace ? 1 : 500;

            ga.SpawnPopulation();

            List<Parameters> maps = GetMaps();

            if (Configuration.IsRace)
            {
                var raceCompetitors = new List<ICarAI>()
                {
                    new HumanCar(),
                    new CarAi(new int[] { 12, 12, 12, 4 }, Random, null)
                };

                StartSimulation(parameters, raceCompetitors.OfType<ICarAI>());
                int generation = 0;
                while (true)
                {
                    EvaluateAgents(raceCompetitors, maps);
                    generation++;
                }
            }
            else
            {
                StartSimulation(parameters, ga.GetPopulation().OfType<ICarAI>());

                int generation = 0;
                while (true)
                {
                    EvaluateAgents(ga.GetPopulation().OfType<ICarAI>(), maps);
                    ga.DoGeneration();

                    if(generation % 20 == 0)
                    {
                        var existingFittest = ga.GetFittestIndividual();
                        var localOffspring = ga.PerformLocalSearch(500); 
                        EvaluateAgents(localOffspring.OfType<ICarAI>(), maps);
                        ga.ReplaceIndividual(existingFittest, localOffspring.OrderByDescending(o => o.GetFitness()).First());
                    }

                    generation++;
                }
            }
        }

        private static List<Parameters> GetMaps()
        {
            var parameters = new List<Parameters>();

            var simDataBase = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName) + "\\SimData";

            var directories = Directory.GetDirectories($"{simDataBase}\\Maps");

            foreach (var directory in directories.Where(d => !d.Contains('_')))
            {
                var mapName = directory.Split('\\').Last();
                parameters.Add(ParameterParser.GetSimulationParameters(new string[] { $"SimData\\Maps\\{mapName}" }));
            }


            return parameters;
        }

        private static void EvaluateAgents(IEnumerable<ICarAI> agents, List<Parameters> maps)
        {
            simulation.EvaluateAgents(agents, maps);
        }

        private static void StartSimulation(Parameters parameters, IEnumerable<ICarAI> agents)
        {
            simulation = new Simulation(parameters);

            if (parameters.StartWeights != null)
            {
                int index = 0;
                foreach (var ai in agents)
                {
                    if (parameters.StartWeights.Count() > index && ai.Network != null)
                    {
                        ai.Network.UpdateNetworkWeights(parameters.StartWeights[index]);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static INeighbour DoCrossover(INeighbour mother, INeighbour father)
        {
            var motherWeights = mother.Network.GetFlattenedWeights();
            var fatherWeights = mother.Network.GetFlattenedWeights();

            int crossoverPosition = Random.Next(1, motherWeights.Length-1);


            var offsprintWeights = new List<float>(motherWeights.Length);

            for (int i = 0; i < motherWeights.Length; i++)
            {
                offsprintWeights.Add(i < crossoverPosition ? motherWeights[i] : fatherWeights[i]);
            }

            var offspring = new CarAi(new int[] { 12, 12, 12, 4 }, Random, offsprintWeights);

            return offspring;
        }

        private static INeighbour CreateNeighbour()
        {
            return new CarAi(new int[] { 12, 12, 12, 4 }, Random, null);
        }
    }
}
