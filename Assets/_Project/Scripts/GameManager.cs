using System;
using System.Collections.Generic;
using System.Linq;
using CardMatch.Board;
using CardMatch.Card;
using CardMatch.SaveGame;
using CardMatch.Score;
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
        [SerializeField] SoundClip MatchSound;
        [SerializeField] SoundClip MissMatchSound;
        [SerializeField] SoundClip GameOverSound;
        
        [Header("Debug Settings")]
        [SerializeField] bool LoggingEnable;
        [SerializeField] bool StartWithoutSave;

        public event Action OnScoreChanged;
        public event Action OnTimeUpdate;
        public event Action OnGameOver;

        public int GetScore() => _scoreSystem.CurrentScore;
        public int GetScoreMultiplier() => _scoreSystem.CurrentMultiplier;
        public float GetGameTime() => _scoreSystem.Time;

        static GameDifficulty _difficulty = GameDifficulty.None;

        ScoreSystem _scoreSystem;
        List<CardView> _cards;
        CardView _lastCardFacedUp;
        float _timer;
        bool _isRunning;

        void Awake()
        {
            PrimeTween.PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            Assert.IsTrue(Config && Config.Cards.Count > 0, $"{nameof(Config)} must be set to a valid configuration.");
            CardMatchLogger.LoggingEnabled = LoggingEnable;
            if(_difficulty == GameDifficulty.None) _difficulty = Difficulty;
        }

        void OnEnable()
        {
            Board.OnCardShow += CardShowHandler;
        }

        async void Start()
        {
            if(StartWithoutSave) SaveManager.DeleteSaveFile();

            var loadGame = SaveManager.LoadGame();
            if (loadGame != null)
            {
                _cards = await Board.SetupBoard(loadGame.CardStates, Config.Cards, InitialCardShowDuration);
                _lastCardFacedUp = Board.SavedCardFacedUp();
                _scoreSystem = new ScoreSystem(loadGame.Score, loadGame.ScoreMultiplier, loadGame.Time);
            }
            else
            {
                _cards = await Board.SetupBoard(CreateGameCardList(), InitialCardShowDuration);
                _scoreSystem = new ScoreSystem();
            }

            _isRunning = true;
            OnScoreChanged?.Invoke();
            OnTimeUpdate?.Invoke();
            PeriodicScoreUpdate().Forget();
        }

        void Update()
        {
            if(_isRunning) _scoreSystem?.Tick(Time.deltaTime);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            CardMatchLogger.Log("OnApplicationPause");
            if (pauseStatus && _isRunning && _scoreSystem != null)
            {
                SaveManager.SaveGame(_cards, _scoreSystem.CurrentScore, _scoreSystem.CurrentMultiplier, _scoreSystem.Time);
            }
        }

        void OnApplicationQuit()
        {
            CardMatchLogger.Log("OnApplicationQuit");
            if (_isRunning)
            {
                SaveManager.SaveGame(_cards, _scoreSystem.CurrentScore, _scoreSystem.CurrentMultiplier, _scoreSystem.Time);
            }
        }

        void OnDisable()
        {
            CardMatchLogger.Log("OnDisable");
            Board.OnCardShow -= CardShowHandler;
        }

        public static void NewGameEasy()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Easy).Forget();
            
        }

        public static void NewGameMedium()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Medium).Forget();
            
        }

        public static void NewGameHard()
        {
            SaveManager.DeleteSaveFile();
            StartNewGame(GameDifficulty.Hard).Forget();
        }

        static async UniTaskVoid StartNewGame(GameDifficulty difficulty)
        {
            _difficulty = difficulty;
            await LoadGameSceneAsync(difficulty);
        }
        
        static async UniTask LoadGameSceneAsync(GameDifficulty difficulty)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            if (asyncOperation != null)
            {
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
        }

        async UniTask PeriodicScoreUpdate()
        {
            while (_isRunning)
            {
                OnTimeUpdate?.Invoke();
                await UniTask.Delay(1000);
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
            DoCardLogic(card).Forget();
        }

        async UniTaskVoid DoCardLogic(CardView card)
        {
            if (card.CardIndex == _lastCardFacedUp.CardIndex)
            {
                CardMatchLogger.Log("Cards Match");
                _scoreSystem.AddScore();
                _scoreSystem.IncreaseMultiplier();
                await CardsMatch(_lastCardFacedUp, card);
                CheckGameOver().Forget();
            }
            else
            {
                CardMatchLogger.Log("Cards don't match");
                CardsMissMatch(_lastCardFacedUp, card).Forget();
                _scoreSystem.ResetMultiplier();
                //TODO: do mismatching stuff: sound
            }

            OnScoreChanged?.Invoke();
            _lastCardFacedUp = null;
        }
        
        async UniTask CardsMatch(CardView cardA, CardView cardB)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(CardShowDuration));
            MatchSound.PlayOneShot();
            cardA.MakeCardFaceDown();
            cardA.gameObject.SetActive(false);
            cardB.MakeCardFaceDown();
            cardB.gameObject.SetActive(false);
        }

        async UniTaskVoid CardsMissMatch(CardView cardA, CardView cardB)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(CardShowDuration));
            MissMatchSound.PlayOneShot();
            cardA.Flip();
            cardB.Flip();
        }

        async UniTaskVoid CheckGameOver()
        {
            await UniTask.NextFrame();
            if (_cards.Any(card => card.gameObject.activeSelf)) return;

            await UniTask.NextFrame();
            CardMatchLogger.Log("Game Over");
            GameOverSound.PlayOneShot();
            _isRunning = false;
            SaveManager.DeleteSaveFile();
            OnGameOver?.Invoke();
        }
    }
}