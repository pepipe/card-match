﻿using System;
using System.Collections.Generic;
using CardMatch.Board;
using CardMatch.Card;
using CardMatch.SaveGame;
using CardMatch.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace CardMatch
{
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard,
        None
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")] 
        [Tooltip("Difficulties: Easy - 2 to 4 different cards | Medium - 5 to 8 different cards | Hard - 9 to 12 different cards.")]
        [SerializeField] GameDifficulty Difficulty;
        [SerializeField] BoardConfig Config;
        [Tooltip("How much more cards should be showed before flipping back/disappear. " +
                 "Note: There's always a bit of time to see the card that depends on the flip animation.")]
        [Range(0f, 2f)]
        [SerializeField] float CardShowDuration = 0f;
        [Range(1f, 5f)]
        [SerializeField] float InitialCardShowDuration = 1f;

        [Header("References")]
        [SerializeField] GameBoard Board;

        [Header("Debug Settings")]
        [SerializeField] bool LoggingEnable;

        static GameDifficulty _difficulty = GameDifficulty.None;
        List<CardView> _cards;
        CardView _lastCardFacedUp;

        void Awake()
        {
            Assert.IsTrue(Config && Config.Cards.Count > 0, $"{nameof(Config)} must be set to a valid configuration.");
            CardMatchLogger.LoggingEnabled = LoggingEnable;
            if(_difficulty == GameDifficulty.None) _difficulty = Difficulty;
        }

        async void Start()
        {
            var loadGame = SaveManager.LoadGame();
            if (loadGame != null)
            {
                _cards = await Board.SetupBoard(loadGame.CardStates, Config.Cards, InitialCardShowDuration);
                //LOAD score, score multiplier and timer
            }
            else
            {
                _cards = await Board.SetupBoard(CreateGameCardList(), InitialCardShowDuration);
            }
        }

        void OnEnable()
        {
            Board.OnCardShow += CardShowHandler;
        }

        void OnDisable()
        {
            Board.OnCardShow -= CardShowHandler;
        }

        void OnApplicationQuit()
        {
            SaveManager.SaveGame(_cards, 0, 0, 0);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NewGameEasy()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Easy).Forget();
            
        }

        public void NewGameMedium()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Medium).Forget();
            
        }

        public void NewGameHard()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Hard).Forget();
            
        }

        async UniTaskVoid StartNewGame(GameDifficulty difficulty)
        {
            _difficulty = difficulty;
            await LoadGameSceneAsync(difficulty);
        }
        
        async UniTask LoadGameSceneAsync(GameDifficulty difficulty)
        {
            // Load the scene asynchronously
            var asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            asyncOperation.allowSceneActivation = false;
            
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >= 0.9f)
                {
                    _difficulty = difficulty;
                    asyncOperation.allowSceneActivation = true;
                }

                await UniTask.NextFrame();
            }
        }
        
        static int GetDifficulty(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    return Random.Range(2, 5);
                case GameDifficulty.Medium:
                    return Random.Range(5, 9);
                case GameDifficulty.Hard:
                    return Random.Range(9, 13);
            }

            return 1;
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
            int differentCardsNumber = GetDifficulty(_difficulty);
            differentCardsNumber = Mathf.Min(differentCardsNumber, Config.Cards.Count);//Ensure that we don't try to get more cards that we have
            var usedIndices = new HashSet<int>();
            var result = new List<CardView>();
            while (usedIndices.Count < differentCardsNumber)
            {
                int randomIndex = Random.Range(0, Config.Cards.Count);
                if (!usedIndices.Add(randomIndex)) continue;
                
                var card = Config.Cards[randomIndex];
                card.CardIndex = randomIndex;
                result.Add(card);
            }

            return result;
        }

        void CardShowHandler(CardView card)
        {
            CardMatchLogger.Log($"Card clicked: {card} | val: {card.CardIndex}");
            if (!_lastCardFacedUp)
            {
                _lastCardFacedUp = card;
                return;
            }
            DoCardLogic(card);
        }

        void DoCardLogic(CardView card)
        {
            if (card.CardIndex == _lastCardFacedUp.CardIndex)
            {
                CardMatchLogger.Log("Cards Match");
                CardsMatch(_lastCardFacedUp, card).Forget();
                //TODO: do matching stuff: score, sound
            }
            else
            {
                CardMatchLogger.Log("Cards don't match");
                CardsMissMatch(_lastCardFacedUp, card).Forget();
                //TODO: do mismatching stuff: reset multiplier, sound
            }

            _lastCardFacedUp = null;
        }

        async UniTaskVoid CardsMatch(CardView cardA, CardView cardB)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(CardShowDuration));
            cardA.gameObject.SetActive(false);
            cardB.gameObject.SetActive(false);
        }

        async UniTaskVoid CardsMissMatch(CardView cardA, CardView cardB)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(CardShowDuration));
            cardA.Flip();
            cardB.Flip();
        }
    }
}