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

        List<CardView> _cards;
        CardView _lastCardFacedUp;

        void Awake()
        {
            Assert.IsTrue(Config && Config.Cards.Count > 0, $"{nameof(Config)} must be set to a valid configuration.");
            CardMatchLogger.LoggingEnabled = LoggingEnable;
        }

        void Start()
        {
            _cards = CreateGameCardList();
            Board.SetupBoard(_cards).Forget();
        }

        void OnEnable()
        {
            Board.OnCardClicked += CardClickHandler;
        }

        void OnDisable()
        {
            Board.OnCardClicked -= CardClickHandler;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        List<CardView> CreateGameCardList()
        {
            var cards = GetDifferentCards();
            var result = new List<CardView>();
            int value = 0;
            foreach (var card in cards)
            {
                card.CardValue = ++value;
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

        void CardClickHandler(CardView card)
        {
            CardMatchLogger.Log($"Card clicked: {card} | val: {card.CardValue}");
            if (!_lastCardFacedUp)
            {
                _lastCardFacedUp = card;
                return;
            }
            DoCardLogic(card);
        }

        void DoCardLogic(CardView card)
        {
            if (card.CardValue == _lastCardFacedUp.CardValue)
            {
                CardMatchLogger.Log("Cards Match");
                _lastCardFacedUp.gameObject.SetActive(false);
                card.gameObject.SetActive(false);
                //TODO: do matching stuff: score, sound
            }
            else
            {
                CardMatchLogger.Log("Cards don't match");
                _lastCardFacedUp.Missmatch();
                card.Missmatch();
                //TODO: do mismatching stuff: reset multiplier, sound
            }
            
            CardMatchLogger.Log($"ResetCard: {_lastCardFacedUp.name}");
            _lastCardFacedUp = null;
        }
    }
}