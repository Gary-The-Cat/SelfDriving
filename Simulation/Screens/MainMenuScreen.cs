using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems;
using CarSimulation.Entities;
using SFML.Graphics;
using SFML.System;

namespace CarSimulation.Screens
{
    public class MainMenuScreen : Screen
    {
        Font gameFont;
        
        public MainMenuScreen(RenderWindow window, FloatRect config) : base(window, config)
        {
            AddRenderSystem(new TextRenderSystem());
            gameFont = new Font("Resources/Arkanoid.ttf");

            world.CreateEntityWith<TextComponent, PositionComponent>(out var titleTextComponent, out var titlePositionComponent);

            titleTextComponent.Text = new FontText(gameFont, "Arkanoid", Color.White);
            titleTextComponent.Text.Scale = 2;
            titlePositionComponent.Position = new Vector2f(Configuration.Width / 2, 40);


            world.CreateEntityWith<TextComponent, PositionComponent>(out var textComponent, out var positionComponent);

            textComponent.Text = new FontText(gameFont, "Press Spacebar To Start", Color.White);
            textComponent.Text.Scale = 1.5f;
            positionComponent.Position = new Vector2f(Configuration.Width / 2, Configuration.Height / 8 * 6.5f);

            base.Initialize();
        }
    }
}
