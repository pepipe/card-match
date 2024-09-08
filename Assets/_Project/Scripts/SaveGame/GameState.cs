using System.Collections.Generic;

namespace CardMatch.SaveGame
{
    [System.Serializable]
    public class GameState
    {
        public List<CardState> CardStates = new();
        public int Score;
        public int ScoreMultiplier;
        public float Time;
    }
}