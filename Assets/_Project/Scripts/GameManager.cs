using System.Collections.Generic;
using CardMatch.Board;
using CardMatch.Card;
using CardMatch.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace CardMatch
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")] [SerializeField]
        BoardConfig Config;

        [Header("References")] [SerializeField]
        GameBoard Board;

        [Header("Debug Settings")] [SerializeField]
        bool LoggingEnable;

        void Awake()
        {
            Assert.IsTrue(Config && Config.Cards.Count > 0, $"{nameof(Config)} must be set to a valid configuration.");
            CardMatchLogger.LoggingEnabled = LoggingEnable;
        }

        void Start()
        {
            var cards = CreateGameCardList();
            Board.SetupBoard(cards);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        List<CardView> CreateGameCardList()
        {
            var cards = GetDifferentCards();
            var result = new List<CardView>();
            foreach (var card in cards)
            {
                result.Add(card);
                result.Add(card);
            }

            result.Shuffle();
            return result;
        }

        List<CardView> GetDifferentCards()
        {
            int differentCardsNumber = BoardConfig.GetDifficulty(Config.Difficulty);
            differentCardsNumber = Mathf.Min(differentCardsNumber, Config.Cards.Count);//Ensure that we don't try to get more cards that we have
            var usedIndices = new HashSet<int>();
            var result = new List<CardView>();
            while (usedIndices.Count < differentCardsNumber)
            {
                int randomIndex = Random.Range(0, Config.Cards.Count);
                if (usedIndices.Add(randomIndex))
                {
                    result.Add(Config.Cards[randomIndex]);
                }
            }

            return result;
        }
    }
}