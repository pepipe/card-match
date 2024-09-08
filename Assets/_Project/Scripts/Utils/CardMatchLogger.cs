using UnityEngine;

namespace CardMatch.Utils
{
    public static class CardMatchLogger
    {
        public static bool LoggingEnabled; 
        
        public static void Log(string message)
        {
            if (!LoggingEnabled) return;
            Debug.Log(message);
        }
        
        public static void LogWarning(string message)
        {
            if (!LoggingEnabled) return;
            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!LoggingEnabled) return;
            Debug.LogError(message);
        }
    }
}