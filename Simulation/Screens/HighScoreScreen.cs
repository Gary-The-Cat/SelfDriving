using System;
using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems;
using CarSimulation.Entities;
using CarSimulation.Managers;
using SFML.Graphics;
using SFML.System;

namespace CarSimulation.Screens
{
    class HighScoreScreen : Screen
    {
        private ScoreManager scoreManager;

        private Font gameFont;

        public HighScoreScreen(RenderWindow window, ScoreManager scoreManager, FloatRect config) : base(window, config)
        {
            this.scoreManager = scoreManager;
            scoreManager.ScoresLoaded = ScoresLoaded;

            gameFont = new Font("Resources/Arkanoid.ttf");

            AddRenderSystem(new TextRenderSystem());

            base.Initialize();
        }

        private void ScoresLoaded()
        {
            var highScores = scoreManager.GetHighScores();

            world.CreateEntityWith<TextComponent, PositionComponent>(out var textComponent, out var positionComponent);
            textComponent.Text = new FontText(gameFont, "Leaderboard", Color.Blue);
            positionComponent.Position = new Vector2f(Configuration.Width / 2, (Configuration.Height / 16) * 6);

            for (int i = 0; i < highScores.Count; i++)
            {
                world.CreateEntityWith<TextComponent, PositionComponent>(out var nameTextComponent, out var namePositionComponent);

                nameTextComponent.Text = new FontText(gameFont, highScores[i].Name, Color.White);
                namePositionComponent.Position = new Vector2f(Configuration.Width / 2 - 200, (Configuration.Height / 16) * (7 + i));

                world.CreateEntityWith<TextComponent, PositionComponent>(out var scoreTextComponent, out var scorePositionComponent);

                scoreTextComponent.Text = new FontText(gameFont, highScores[i].Score.ToString(), Color.White);
                scorePositionComponent.Position = new Vector2f(Configuration.Width / 2 + 200, (Configuration.Height / 16) * (7 + i));
            }
        }
    }
}
