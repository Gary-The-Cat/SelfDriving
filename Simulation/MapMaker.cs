using Arkanoid_SFML.Maths;
using CarSimulation.DataStructures;
using CarSimulation.Helpers;
using CarSimulation.Managers;
using CarSimulation.Screens;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarSimulation
{
    public class MapMaker
    {
        ScreenManager screenManager;
        Clock clock;
        RenderWindow window;
        Vector2f? startPosition = null;
        List<Vector2f> checkpoints { get; set; }

        bool isCreatingSegment = false;
        bool isCreatingCheckpoint = false;

        List<Vertex[]> lineSegments;
        List<Vertex> selectedPoints;

        
        public MapMaker(Parameters parameters)
        {
            // Create the main window
            window = new RenderWindow(new VideoMode(Configuration.Width, Configuration.Height), "Map Maker");
            window.SetFramerateLimit(60);
            window.Closed += OnClose;

            window.Resized += OnResize;
            window.SetMouseCursorVisible(false);

            screenManager = new ScreenManager(window);

            clock = new Clock();
            
            lineSegments = parameters.Map;
            startPosition = parameters.StartPosition ?? new Vector2f(100, 100);
            checkpoints = parameters.Checkpoints;
            selectedPoints = new List<Vertex>();
        }

        private void OnResize(object sender, SizeEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            screenManager.OnResize(window.Size.X, window.Size.Y);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        float transitionTimer = 0;

        public void Run()
        {
            window.SetMouseCursorVisible(true);

            window.MouseButtonPressed += MouseButtonPressed;

            window.KeyPressed += KeyPressed;
            window.KeyReleased += KeyReleased;

            while (window.IsOpen)
            {
                float deltaT = Configuration.IsDebugFrameTime ? Configuration.DebugFrameTime : clock.Restart().AsMicroseconds() / 1000000f;
                transitionTimer += deltaT;

                TimeTracker.Time.TimeStep(deltaT);

                window.SetMouseCursorVisible(true);

                var position = Mouse.GetPosition();

                // Process events
                window.DispatchEvents();

                if (transitionTimer > 0.1)
                {
                    screenManager.Update(deltaT);
                }

                screenManager.Draw(deltaT);

                if (isCreatingSegment)
                {
                    UpdateLineEnd(CheckExistingPoints(GetMousePosition()));
                }

                if (isCreatingCheckpoint)
                {
                    checkpoints[checkpoints.Count() - 1] = GetMousePosition();
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.M))
                {
                    var mousePosition = GetMousePosition();
                    foreach (var line in lineSegments)
                    {
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (MathHelper.GetDistance(line[i].Position, mousePosition) < 20)
                            {
                                line[i] = new Vertex(mousePosition);
                            }
                        }
                    }

                    for(int j = 0; j < checkpoints.Count(); j++)
                    {
                        if (MathHelper.GetDistance(checkpoints[j], mousePosition) < 20)
                        {
                            checkpoints[j] = GetMousePosition();
                        }
                    }


                    if (MathHelper.GetDistance(startPosition.Value, mousePosition) < 20)
                    {
                        startPosition = mousePosition;
                    }
                }
                else
                {
                    selectedPoints.Clear();
                }

                foreach (var lineSegment in lineSegments)
                {
                    window.Draw(lineSegment, 0, 2, PrimitiveType.Lines);
                }

                int k = 0;
                int step = 255 / (!checkpoints.Any() ? 1 : checkpoints.Count());
                foreach(var checkpoint in checkpoints)
                {
                    var value = (byte)(k * step);
                    window.Draw(new CircleShape(100) 
                    { 
                        OutlineColor = new Color((byte)(value), (byte)(value), (byte)(value)), 
                        FillColor = Color.Transparent,
                        OutlineThickness = 5,
                        Position = checkpoint,
                        Origin = new Vector2f(100, 100)
                    });
                    k++;
                }

                if (startPosition.HasValue)
                {
                    window.Draw(new CircleShape(10) 
                    {
                        Position = startPosition.Value,
                        FillColor = Color.Green
                    });
                }

                // Update the window
                window.Display();
            }

        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if(e.Code == Keyboard.Key.C)
            {
                isCreatingCheckpoint = false;
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.P))
            {
                string lineSegmentString = string.Empty;

                foreach(var line in lineSegments)
                {
                    lineSegmentString += $"{line[0].Position.X},{line[0].Position.Y},{line[1].Position.X},{line[1].Position.Y}\n";
                }

                var simDataBase = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName) + "\\SimData";

                var mapName = GetNextAvailableMapName(simDataBase);

                File.WriteAllText($"{simDataBase}\\Maps\\{ mapName }\\Map", lineSegmentString.Trim('\n'));

                if (startPosition.HasValue)
                {
                    File.WriteAllText($"{simDataBase}\\Maps\\{ mapName }\\StartPosition", $"{startPosition.Value.X},{startPosition.Value.Y}");
                }

                string checkpointString = string.Empty;

                foreach (var checkpoint in checkpoints)
                {
                    checkpointString += $"{checkpoint.X}, {checkpoint.Y}\n";
                }

                File.WriteAllText($"{simDataBase}\\Maps\\{ mapName }\\Checkpoints", checkpointString.Trim('\n'));
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
                isCreatingSegment = false;
                if (lineSegments.Any())
                {
                    lineSegments.Remove(lineSegments.Last());
                }
            }
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                SetStartPosition();
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.C) && !isCreatingCheckpoint)
            {
                isCreatingCheckpoint = true;
                SetCheckpointPosition();
            }
        }

        private void SetCheckpointPosition()
        {
            checkpoints.Add(GetMousePosition());
        }

        private void SetStartPosition()
        {
            startPosition = GetMousePosition();
        }

        private string GetNextAvailableMapName(string simDataBase)
        {
            int mapNumber = 0;
            while (Directory.Exists($"{simDataBase}\\Maps\\{mapNumber}"))
            {
                mapNumber++;
            }

            Directory.CreateDirectory($"{simDataBase}\\Maps\\{mapNumber}");

            return $"{mapNumber}";
        }

        private void UpdateLineEnd(Vector2f mousePosition)
        {
            var currentSSegment = lineSegments.Last();
            currentSSegment[currentSSegment.Length - 1] = new Vertex(mousePosition);
        }

        private void MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!isCreatingSegment)
            {
                // Snap to the end of a previous segment if it is within snapping distance.
                var mousePosition = CheckExistingPoints(GetMousePosition());

                lineSegments.Add(new Vertex[] 
                {
                    new Vertex(mousePosition),
                    new Vertex(mousePosition)
                });
            }
            else
            {
                // Snap end of segment to start if it is within the snapping distance.
                lineSegments[lineSegments.Count() - 1][1].Position = CheckExistingPoints(lineSegments[lineSegments.Count() - 1][1].Position);
            }

            isCreatingSegment = !isCreatingSegment;
        }

        private Vector2f CheckExistingPoints(Vector2f mousePos)
        {
            if (!lineSegments.Any())
            {
                return mousePos;
            }

            var lastSegment = lineSegments.Last();
            foreach (var lineSegment in lineSegments)
            {
                if (isCreatingSegment && lineSegment == lastSegment)
                {
                    continue;
                }

                var segmentEnd = lineSegment[1].Position;
                var segmentStart = lineSegment[0].Position;

                var distanceSquared = ((segmentStart.X - mousePos.X) * (segmentStart.X - mousePos.X)) + ((segmentStart.Y - mousePos.Y) * (segmentStart.Y - mousePos.Y));
                if (distanceSquared < 200)
                {
                    return segmentStart;
                }

                distanceSquared = ((segmentEnd.X - mousePos.X) * (segmentEnd.X - mousePos.X)) + ((segmentEnd.Y - mousePos.Y) * (segmentEnd.Y - mousePos.Y));
                if (distanceSquared < 200)
                {
                    return segmentEnd;
                }
            }

            return mousePos;
        }

        private Vector2f GetMousePosition()
        {
            var position = Mouse.GetPosition();

            var adjustedPosition = position - window.Position;

            return new Vector2f(adjustedPosition.X - 10, adjustedPosition.Y - 55);
        }

        private Vector2f GetOffsetPosition(Vector2f position, Vector2f offset)
        {
            return position + offset;
        }
    }
}
