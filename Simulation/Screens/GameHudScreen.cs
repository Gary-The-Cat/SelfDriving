using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems;
using CarSimulation.Entities;
using CarSimulation.Managers;
using SFML.Graphics;
using SFML.System;
using System.Linq;

namespace CarSimulation.Screens
{
    class GameHudScreen : Screen
    {
        private ScoreManager scoreManager;
        private Font gameFont;
        private TextComponent scoreTextComponent;
        private TextComponent highScoreComponent;
        private Sprite background;

        public Vector2i MousePosition { get; internal set; }

        public GameHudScreen(RenderWindow window, ScoreManager scoreManager, FloatRect config) : base(window, config)
        {
            this.scoreManager = scoreManager;

            AddRenderSystem(new TextRenderSystem());
            var backgroundTexture = new Texture("Resources//background.png");
            background = new Sprite(backgroundTexture);
            background.Scale = new Vector2f(
                Configuration.Width / background.GetLocalBounds().Width,
                Configuration.Height / background.GetLocalBounds().Height);

            gameFont = new Font("Resources/Arkanoid.ttf");

            world.CreateEntityWith<TextComponent, PositionComponent>(out scoreTextComponent, out var scorePositionComponent);
            world.CreateEntityWith<TextComponent, PositionComponent>(out var highScoreTextTitleComponent, out var highScoreTitlePositionComponent);
            world.CreateEntityWith<TextComponent, PositionComponent>(out highScoreComponent, out var highScorePositionComponent);

            scoreTextComponent.Text = new FontText(gameFont, "0", Color.White);
            scorePositionComponent.Position = new Vector2f(60, 30);

            highScoreTextTitleComponent.Text = new FontText(gameFont, "High Score", Color.White);
            highScoreTextTitleComponent.Text.Scale = 1.5f;
            highScoreTitlePositionComponent.Position = new Vector2f(Configuration.Width / 2, 10);

            highScoreComponent.Text = new FontText(gameFont, "0", Color.White);
            highScorePositionComponent.Position = new Vector2f(Configuration.Width / 2, 60);

            base.Initialize();
        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            scoreTextComponent.Text.StringText = scoreManager.GetScore();
            var topScore = scoreManager.GetHighScores().FirstOrDefault();
            if(topScore != null)
            {
                highScoreComponent.Text.StringText = MousePosition.ToString();
            }
        }

        public override void Draw(float deltaT)
        {
            window.Draw(background);
            base.Draw(deltaT);
        }
    }
}
