namespace CardMatch.Score
{
    public class ScoreSystem
    {
        public int CurrentScore { get; private set; }
        public int CurrentMultiplier { get; private set; }
        public float Time { get; private set; }

        readonly int _startingMultiplier;

        public ScoreSystem(int startingScore = 0, int startingMultiplier = 1, float time = 0f)
        {
            CurrentScore = startingScore;
            _startingMultiplier = CurrentMultiplier = startingMultiplier;
            Time = time;
        }

        public void AddScore(int scorePoints = 1) => CurrentScore += scorePoints * CurrentMultiplier;
        public void IncreaseMultiplier(int value = 1) => CurrentMultiplier += value;
        public void ResetMultiplier() => CurrentMultiplier = _startingMultiplier;

        public void Tick(float deltaTime)
        {
            Time += deltaTime;
        }
    }
}