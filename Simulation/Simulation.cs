using CarSimulation.Screens;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using CarSimulation.DataStructures;
using System;
using System.Collections.Generic;
using Arkanoid_SFML.Agents.Interfaces;
using Arkanoid_SFML.Screens;
using System.Threading;
using CarSimulation.ExtensionMethods;
using CarSimulation.Entities;

namespace CarSimulation
{
    public class Simulation
    {
        ScreenManager screenManager;
        CarSimulationScreen carSimScreen;
        Clock clock;
        RenderWindow window;

        List<Vertex[]> lineSegments;
        Vector2f startPosition;
        int gen = 1;

        public Simulation(Parameters parameters)
        {
            // Create the main window
            window = new RenderWindow(
                new VideoMode(Configuration.Width, Configuration.Height), 
                "Simulation", 
                Styles.Fullscreen, 
                new ContextSettings() { AntialiasingLevel = 8 });
            window.SetFramerateLimit(60);

            // Set callbacks for resize/close
            window.Closed += OnClose;
            window.Resized += OnResize;

            // Copy the map
            lineSegments = parameters.Map;

            // Set our starting position (initial heading is always straight down)
            this.startPosition = parameters.StartPosition.HasValue 
                ? parameters.StartPosition.Value 
                : new Vector2f(100,100);

            screenManager = new ScreenManager(window);

            var configuration = Configuration.SinglePlayer;

            carSimScreen = new CarSimulationScreen(window, configuration, lineSegments, startPosition);

            screenManager.AddScreen(carSimScreen);

            // Used for frametimes
            clock = new Clock();

            Thread.Sleep(3000);
        }

        public void EvaluateAgents(IEnumerable<ICarAI> agents, List<Parameters> maps)
        {
            this.Initalise(agents);
            
            foreach(var map in maps)
            {
                // Copy the map
                lineSegments = map.Map;
                carSimScreen.SetLineSegments(lineSegments);

                // Set our starting position (initial heading is always straight down)
                this.startPosition = map.StartPosition.Value;

                carSimScreen.InitaliseCars(map.Checkpoints, this.startPosition, 0);

                this.EvaluateAgents();

                window.Clear();
                window.Display();

                Thread.Sleep(100);
            }


            window.Clear();

            var text = new FontText(new Font("SimData\\Fonts\\Calibri.ttf"), $"Training Generation {gen++}...", Color.White) { Scale = 2 };
            var cameraPosition = carSimScreen.Camera.GetView().Center;
            window.DrawString(text, cameraPosition);
            window.Display();
        }

        public void Initalise(IEnumerable<ICarAI> agents)
        {
            // Initalise the sim
            carSimScreen.Initalise(agents);
        }

        public void EvaluateAgents()
        {
            int frame = 0;

            // While we have any cars alive, the window is open and we havent run for more than 2 minutes
            while (carSimScreen.IsRunning && window.IsOpen)
            {
                frame++;
                float deltaT = Configuration.IsDebugFrameTime 
                    ? Configuration.DebugFrameTime 
                    : clock.Restart().AsMicroseconds() / 1000000f;

                // Interaction with window: close, resize etc.
                window.DispatchEvents();

                // Update all the windows that are set to active (Just our sim)
                screenManager.Update(deltaT);

                // Draw all the screens that are set to active
                screenManager.Draw(deltaT);

                if (Keyboard.IsKeyPressed(Keyboard.Key.N))
                {
                    // Go to the next map
                    break;
                }
            }

            if (!Configuration.IsRace)
            {
                carSimScreen.WriteNetworkToFile();
            }

            if (!window.IsOpen)
            {
                Environment.Exit(0);
            }
        }

       

        private void OnResize(object sender, SizeEventArgs e)
        {
            // Do nothing
        }

        private static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}
