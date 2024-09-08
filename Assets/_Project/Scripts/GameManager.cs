using CardMatch.Utils;
using UnityEngine;

namespace CardMatch
{
    public class GameManager : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] bool LoggingEnable;

        void Awake()
        {
            CardMatchLogger.LoggingEnabled = LoggingEnable;
        }
    }
}