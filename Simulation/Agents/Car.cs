using Arkanoid_SFML.Agents.Interfaces;
using CarSimulation.CollisionData;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Arkanoid_SFML.DataStructures;
using SFML.Window;
using Arkanoid_SFML.Managers;
using Arkanoid_SFML;

namespace CarSimulation.Agents
{
    public class Car
    {
        private const int MaximumForwardSpeed = 1000;
        private const int MaximumReverseSpeed = -400;
        private const int MaximumRads = 5;
        private const float DragCoefficient = 0.015f;

        public Car(ICarAI ai)
        {
            Ai = ai;
            Ai.Fitness = -1;


            Sprite = new RectangleShape(new Vector2f(20, 45))
            {
                OutlineThickness = 2,
                OutlineColor = Color.White,
                Origin = new Vector2f(10, 22.5f)
            };


            if ((new Random().NextDouble()) > 0.95)
            {
                Sprite.Texture = new Texture(new Image("nascarDoge.png"));
            }
            else
            {
                Sprite.Texture = new Texture(new Image("nascar.png"));
            }
            
            GetRays = new Vertex[7][];
            rayLengths = new float[7] { 175, 175, 175, 230, 175, 175, 175 };

            for (int i = 0; i < 7; i++)
            {
                GetRays[i] = new Vertex[2];

                GetRays[i][0] = new Vertex(new Vector2f(0, 0), Color.Green);
                GetRays[i][1] = new Vertex(new Vector2f(0, 0), Color.Green);
            }


            CollisionPoints = new CircleShape[7];
            for (int i = 0; i < 7; i++)
            {
                CollisionPoints[i] = new CircleShape(10) { Origin = new Vector2f(5, 5) };
            }
        }

        public void ResetCar(List<Vector2f> checkpoints, Vector2f position, float heading)
        {
            StartPosition = position;
            Position = position;
            Heading = heading;
            IsRunning = true;
            PreviousSpeeds.Clear();
            ExitedStartArea = false;
            Sprite.OutlineColor = Color.White;
            CheckpointManager = new CheckpointManager(checkpoints);
            Ai.Initalize(this);
        }

        public ICarAI Ai { get; set; }

        public Vertex[][] GetRays { get; }

        public CircleShape[] CollisionPoints;

        public Vector2f Position;

        public Vector2f StartPosition;

        public bool ExitedStartArea = false;

        public float Speed { get; set; }

        public float Heading { get; set; }

        public RectangleShape Sprite { get; set; }

        public bool IsRunning { get; internal set; } = true;

        public float AverageSpeed => TotalDistance / TimeAlive;

        public float RecentAverageSpeed => PreviousSpeeds.Sum(s => s) / PreviousSpeeds.Count();

        private float[] rayLengths;

        public float TotalDistance = 0;

        public float TimeAlive = 0;

        public List<float> PreviousSpeeds { get; set; } = new List<float>();

        public CheckpointManager CheckpointManager { get; set; }

        public int CheckpointsPassed => CheckpointManager.CheckpointsPassed;
        
        private Vector2f previousPosition;

        public void Update(float[] rayCollisions, float deltaT)
        {
            IsRunning = CheckpointManager.Update(Position);

            var output = Ai.GetOutput(rayCollisions, Position, Heading, CheckpointManager.CurrentWaypoint);

            var acceleration = Math.Min(output.Acceleration, 1);
            acceleration = Math.Max(acceleration, -1);

            var braking = Math.Min(output.BreakingForce, 1);
            braking = Math.Max(braking, -1);

            acceleration *= CarConfiguration.AccelerationCoefficient;
            braking *= CarConfiguration.BrakeCoefficient;

            // Apply acceleration
            Speed += acceleration - braking;

            // Apply drag
            Speed -= Speed * DragCoefficient;

            if(Speed < 0)
            {
                IsRunning = false;
            }

            // Cap max speed
            Speed = Math.Min(Speed, MaximumForwardSpeed);
            Speed = Math.Max(Speed, MaximumReverseSpeed);

            var turningForce = -output.LeftTurnForce + output.RightTurnForce;

            turningForce = Math.Min(turningForce, 1);
            turningForce = Math.Max(turningForce, -1);

            Heading += MaximumRads * deltaT * turningForce;

            Position.X += (float)Math.Cos(Heading + 1.5708) * Speed * deltaT;
            Position.Y += (float)Math.Sin(Heading + 1.5708) * Speed * deltaT;

            Sprite.Position = Position;
            Sprite.Rotation = Heading;

            if (GetDistance(StartPosition, Position) > 300)
            {
                ExitedStartArea = true;
            }

            if (IsRunning)
            {
                var distance = GetDistance(Position, previousPosition);

                TotalDistance += distance;
                TimeAlive += deltaT;
                PreviousSpeeds.Add(distance / deltaT);
                if (PreviousSpeeds.Count() > 180)
                {
                    PreviousSpeeds.Remove(PreviousSpeeds.First());
                }

                Ai.IncrementFitness(this);

                previousPosition = Position;
            }

            UpdateRayCasts();
        }

        private float GetDistance(Vector2f a, Vector2f b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private void UpdateRayCasts()
        {
            for (int i = 0; i < 7; i++)
            {
                var rayStart = new Vector2f(
                    Position.X + 20 * (float)Math.Cos(Heading + ConvertToRadians(30) * i),
                    Position.Y + 20 * (float)Math.Sin(Heading + ConvertToRadians(30) * i));

                var rayEnd = new Vector2f(
                    Position.X + rayLengths[i] * (float)Math.Cos(Heading + ConvertToRadians(30) * i),
                    Position.Y + rayLengths[i] * (float)Math.Sin(Heading + ConvertToRadians(30) * i));

                GetRays[i][0].Position = rayStart;
                GetRays[i][1].Position = rayEnd;
            }
        }

        public void DrawWaypoints(RenderTarget window)
        {
            window.Draw(new CircleShape(100) { Position = CheckpointManager.CurrentWaypoint, Origin = new Vector2f(100, 100) });

            window.Draw(new CircleShape(100) { Position = CheckpointManager.LastWaypoint, Origin = new Vector2f(100, 100) });
        }

        public void Draw(RenderTarget window)
        {
            Sprite.Position = Position;
            Sprite.Rotation = Heading * 180 / (3.14159265358f);

            if (!IsRunning)
            {
                Sprite.OutlineColor = new Color(0x1e, 0x1e, 0x1e);
            }

            window.Draw(Sprite);

            if (this.IsRunning && Configuration.ShowDebug)
            {
                foreach (var lineSegment in GetRays)
                {
                    window.Draw(lineSegment, 0, 2, PrimitiveType.Lines);
                }

                foreach (var collision in CollisionPoints.Where(c => c.Position.X != 0))
                {
                    window.Draw(collision);
                }
            }
        }

        public bool CheckMapCollision(List<Vertex[]> lineSegments)
        {
            foreach(var segment in lineSegments)
            {
                if(CheckCollision(segment[0].Position, segment[1].Position))
                {
                    return true;
                }
            }

            return false;
        }

        private float ConvertToRadians(float degrees)
        {
            return degrees * 3.14159f / 180;
        }

        private bool CheckCollision(Vector2f a, Vector2f b)
        {
            var p1 = Sprite.Transform.TransformPoint(Sprite.GetPoint(0));
            var p2 = Sprite.Transform.TransformPoint(Sprite.GetPoint(1));
            var p3 = Sprite.Transform.TransformPoint(Sprite.GetPoint(2));
            var p4 = Sprite.Transform.TransformPoint(Sprite.GetPoint(3));

            return 
                CollisionManager.CheckCollision(a, b, p1, p2) != null ||
                CollisionManager.CheckCollision(a, b, p2, p3) != null ||
                CollisionManager.CheckCollision(a, b, p3, p4) != null ||
                CollisionManager.CheckCollision(a, b, p4, p1) != null;
        }
    }
}
