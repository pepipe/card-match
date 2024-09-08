using System.Collections.Generic;
using CardMatch.Card;
using UnityEngine;

namespace CardMatch.Board
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "CardMatch/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [Tooltip("Difficulties: Easy - 2 to 4 different cards | Medium - 5 to 8 different cards | Hard - 9 to 12 different cards.")]
        public Difficulty Difficulty;
        
        public List<CardView> Cards = new ();
        
        public static int GetDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return Random.Range(2, 5);
                case Difficulty.Medium:
                    return Random.Range(5, 9);
                case Difficulty.Hard:
                    return Random.Range(9, 13);
            }

            return 1;
        }
    }
}