using CarSimulation.Events;
using CarSimulation.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarSimulation.Events.Events;

namespace CarSimulation.Managers
{
    public class ScoreManager
    {
        private int score;

        private int lives = 3;

        private FireBaseClient client;

        private List<HighScore> leaderboard;

        private bool newHighScore = false;

        public Action ScoresLoaded;

        public HighScore CurrentHighscore;

        public ScoreManager()
        {
            leaderboard = new List<HighScore>(5);
            EventManager.Listen<BrickDestroyedEvent>(BrickDestroyed);
            EventManager.Listen<DeathOccuredEvent>(DeathOccurred);
            _ = InitialLoadHighScores();
        }

        private void DeathOccurred(DeathOccuredEvent obj)
        {
            TakeLife();
        }

        private void BrickDestroyed(BrickDestroyedEvent obj)
        {
            score += 100;
        }

        public async Task InitialLoadHighScores()
        {
            await LoadHighScores();
            ScoresLoaded.Invoke();
        }

        public async Task LoadHighScores()
        {
            client = new FireBaseClient("Y2Fsthu5fpLVWirLW0jQjbXZImy7F7pfzn0grooR", "https://arkanoid-87b95.firebaseio.com/");

            var highscores = new List<Task<HighScore>>();
            for (int i = 1; i < 6; i++)
            {
                highscores.Add(client.LoadAsync<HighScore>($"Highscores/{i}"));
            }

            await Task.WhenAll(highscores);

            leaderboard = highscores.Select(s => s.Result).OrderBy(s => s.Position).ToList();
        }

        public void TakeLife()
        {
            lives--;
        }
        
        public string GetScore()
        {
            return score.ToString();
        }

        public int GetLives()
        {
            return lives;
        }

        public bool AnyLives()
        {
            return lives != 0;
        }

        public void AddScore(string name, int score)
        {
            var highscore = new HighScore()
            {
                Name = name,
                Score = score
            };

            leaderboard.Add(highscore);

            leaderboard = leaderboard.OrderByDescending(s => s.Score).Take(5).ToList();

            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Position = i + 1;
            }

            newHighScore = leaderboard.Contains(highscore);

            if (newHighScore)
            {
                CurrentHighscore = highscore;
            }
        }

        public bool IsNewHighScore()
        {
            return newHighScore;
        }

        public void SaveLeaderboard()
        {
            newHighScore = false;
            leaderboard.Add(CurrentHighscore);
            leaderboard = leaderboard.OrderByDescending(s => s.Score).Take(5).ToList();
            for (int i = 1; i < 6; i++)
            {
                var highscore = leaderboard[i - 1];
                highscore.Position = i;
                _ = client.SaveAsync<HighScore>($"Highscores/{i}", highscore);
            }
        }

        public List<HighScore> GetHighScores()
        {
            return leaderboard;
        }
    }
}
