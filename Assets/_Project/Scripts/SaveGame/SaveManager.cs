using System.Collections.Generic;
using System.IO;
using CardMatch.Card;
using CardMatch.Utils;
using UnityEngine;

namespace CardMatch.SaveGame
{
    public static class SaveManager
    {
        static string SaveFilePath => Path.Combine(Application.persistentDataPath, "game_save.json");
        
        public static void SaveGame(IEnumerable<CardView> cards)
        {
            var gameState = CreateGameState(cards);
            string json = JsonUtility.ToJson(gameState);
            File.WriteAllText(SaveFilePath, json);
            CardMatchLogger.Log($"Saved game state to {SaveFilePath}");
        }
        
        public static GameState LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning("No save file found!");
                return null;
            }

            string json = File.ReadAllText(SaveFilePath);
            CardMatchLogger.Log($"Game state loaded from {SaveFilePath}");
            return JsonUtility.FromJson<GameState>(json);
        }

        static GameState CreateGameState(IEnumerable<CardView> cards)
        {
            var gameState = new GameState
            {
                CardStates = new List<CardState>(),
                Score = 0,
                ScoreMultiplier = 0,
                Timer = 0f
            };
            
            foreach (var card in cards)
            {
                gameState.CardStates.Add(new CardState
                {
                    CardValue = card.CardValue,
                    IsFaceUp = card.IsCardFlipped(),
                    IsActive = card.gameObject.activeSelf
                });
            }

            return gameState;
        }
    }
}