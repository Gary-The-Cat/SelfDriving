using Arkanoid_SFML.Agents;
using Arkanoid_SFML.Agents.Interfaces;
using Arkanoid_SFML.Helpers;
using CarSimulation;
using CarSimulation.Agents;
using CarSimulation.CollisionData;
using CarSimulation.Screens;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid_SFML.Screens
{
    class CarSimulationScreen : Screen
    {
        private Vector2f startPosition;

        private RenderTexture texture;

        private int frame;

        private List<Car> cars;

        List<Vertex[]> lineSegments;

        public bool IsRunning => Configuration.IsRace 
            ? cars?.Where(c => c.Ai is HumanCar).First().IsRunning ?? true
            : cars.Any(c => c.IsRunning) && frame < Configuration.MaxRuntime;

        public CarSimulationScreen(
            RenderWindow window, 
            FloatRect configuration,
            List<Vertex[]> lineSegments,
            Vector2f startPosition) : base(window, configuration)
        {
            this.lineSegments = lineSegments;
            this.startPosition = startPosition;

            // Rendering to a texture before we render to the screen allows us to save the rendered image to file.
            texture = new RenderTexture(Configuration.Width, Configuration.Height);
        }

        public void Initalise(IEnumerable<ICarAI> agents)
        {
            // Create our cars
            cars = new List<Car>();
            foreach (var agent in agents)
            {
                cars.Add(new Car(agent));
            }

            // As we are using set frame times (60fps), we count frames for durations.
            frame = 0;
        }

        public void InitaliseCars(List<Vector2f> checkpoints, Vector2f position, float heading)
        {
            foreach (var car in cars)
            {
                car.ResetCar(checkpoints, position, heading);
            }

            this.frame = 0;
        }

        internal void SetLineSegments(List<Vertex[]> lineSegments)
        {
            this.lineSegments = lineSegments;
        }

        public override void Update(float deltaT)
        {
            var maxHeading = cars.Where(c => c.IsRunning).Select(c => c.Heading).Min();
            Camera.Update(deltaT);
            var followCar = Configuration.IsRace ? cars.Where(c => c.Ai is HumanCar).First() : cars.Where(c => c.IsRunning).OrderByDescending(c => c.Ai.Fitness).First();
            Camera.SetCentre(followCar.Position, 0.05f);
            RemovePoorlyPerformingCars();

            Parallel.ForEach(cars.Where(c => c.IsRunning), car =>
            {
                var rayCollisions = CheckCollisions(car);

                // Check car collisions with wall
                if (car.CheckMapCollision(lineSegments))
                {
                    car.IsRunning = false;
                }
                
                if (car.IsRunning)
                {
                    car.Update(rayCollisions, deltaT);
                }
            });
        }

        private void RemovePoorlyPerformingCars()
        {
            if (Configuration.IsRace)
            {
                return;
            }

            if(frame == 150)
            {
                foreach(var car in cars)
                {
                    if (!car.ExitedStartArea || car.AverageSpeed < 25)
                    {
                        car.IsRunning = false;
                        car.Ai.Fitness -= 1000;
                    }
                }
            }
            if (frame > 300)
            {
                foreach (var car in cars.Where(c => c.IsRunning))
                {
                    if (car.RecentAverageSpeed < 100)
                    {
                        car.IsRunning = false;
                    }
                }
            }
        }

        public override void Draw(float deltaT)
        {
            if (Configuration.RecordToFile)
            {
                DrawToFile();
            }
            else
            {
                DrawToWindow();
            }

            frame++;
            window.SetView(Camera.GetView());
            window.Display();
        }

        private void DrawToWindow()
        {
            window.Clear();
            foreach (var car in cars.OrderBy(c => c.IsRunning))
            {
                if (car.IsRunning)
                {
                    car.DrawWaypoints(window);
                }
            }

            foreach (var car in cars.OrderBy(c => c.IsRunning))
            {
                car.Draw(window);
            }

            foreach (var lineSegment in lineSegments)
            {
                window.Draw(SFMLHelper.GetLine(lineSegment[0].Position, lineSegment[1].Position, 3));
            }
        }

        private void DrawToFile()
        {
            texture.Clear(new Color(0x1e, 0x1e, 0x1e));
            window.Clear();

            foreach (var car in cars.OrderBy(c => c.IsRunning))
            {
                car.Draw(texture);
            }

            foreach (var lineSegment in lineSegments)
            {
                texture.Draw(lineSegment, 0, 2, PrimitiveType.Lines);
            }

            texture.Display();

            if (Configuration.RecordToFile)
            {
                if (!Directory.Exists($"C:\\Simulation\\Captures\\"))
                {
                    Directory.CreateDirectory($"C:\\Simulation\\Captures\\");
                }

                texture.Texture.CopyToImage().SaveToFile($"C:\\Simulation\\Captures\\{frame.ToString("000000")}.png");
            }

            var sprite = new Sprite(texture.Texture);
            window.Draw(sprite);
        }

        public void WriteNetworkToFile()
        {
            var neuralNetworks = new StringBuilder();

            foreach (var car in cars.OrderByDescending(c => c.Ai.Fitness))
            {
                car.Ai.Network.GetFlattenedWeights().ToList().ForEach(w => neuralNetworks.Append($"{w},"));
                neuralNetworks.AppendLine(string.Empty);
            }

            var simDataBase = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName) + "\\SimData";

            if (!Directory.Exists($"{simDataBase}\\Individuals"))
            {
                Directory.CreateDirectory($"{simDataBase}\\Individuals");
            }

            File.WriteAllText($"{simDataBase}\\Individuals\\Weights.nn", neuralNetworks.ToString());
        }

        private float[] CheckCollisions(Car car)
        {
            var colliding = new HashSet<Vertex[]>();
            var rayCollisions = new float[7] { 1, 1, 1, 1, 1, 1, 1 };

            for (int i = 0; i < car.GetRays.Length; i++)
            {
                var ray = car.GetRays[i];
                var p2 = ray[0].Position;
                var p3 = ray[1].Position;
                foreach (var line in lineSegments)
                {
                    var p0 = line[0].Position;
                    var p1 = line[1].Position;
                    var collisionPoint = CollisionManager.CheckCollision(p0, p1, p2, p3);
                    if (collisionPoint != null)
                    {
                        ray[0].Color = Color.Red;
                        ray[1].Color = Color.Red;
                        colliding.Add(ray);
                        var rayLength = Math.Pow(p2.X - p3.X, 2) + Math.Pow(p2.Y - p3.Y, 2);
                        var hitLength = Math.Pow(p2.X - collisionPoint.Value.X, 2) + Math.Pow(p2.Y - collisionPoint.Value.Y, 2);
                        var hitDistance = (float)(hitLength / rayLength);
                        if (hitDistance < rayCollisions[i])
                        {
                            rayCollisions[i] = hitDistance;
                            car.CollisionPoints[i].Position = collisionPoint.Value;
                        }
                    }
                }

                if (ray[0].Color != Color.Red)
                {
                    car.CollisionPoints[i].Position = new Vector2f();
                }
            }

            foreach (var ray in car.GetRays.Where(r => !colliding.Contains(r)))
            {
                ray[0].Color = Color.Green;
                ray[1].Color = Color.Green;
            }

            return rayCollisions;
        }
    }
}
