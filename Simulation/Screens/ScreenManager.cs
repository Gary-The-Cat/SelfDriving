using CarSimulation.Screens;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarSimulation.Screens
{
    public class ScreenManager
    {
        private List<Screen> screens;

        private RenderWindow window;

        public ScreenManager(RenderWindow window)
        {
            this.window = window;

            screens = new List<Screen>();
        }

        public void AddScreen(Screen screen)
        {
            screens.Insert(0, screen);
        }

        public void RemoveScreen(Screen screen)
        {
            screens.Remove(screen);
        }

        public void OnResize(float width, float height)
        {
            foreach(var screen in screens)
            {
                screen.Camera.ScaleToWindow(width, height);
            }
        }

        public void Update(float deltaT)
        {
            foreach (var screen in screens.Where(s => s.IsUpdate))
            {
                screen.Update(deltaT);
            }
        }

        private int count = 0;
        public void Draw(float deltaT)
        {
            window.Clear(Configuration.Background);
            
            foreach (var screen in screens.Where(s => s.IsDraw))
            {
                window.SetView(screen.Camera.GetView());
                screen.Draw(deltaT);
            }
        }

        public  void Initalise()
        {
            foreach(var screen in screens)
            {
                screen.Initialize();
            }
        }
    }
}
