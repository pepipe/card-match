using TMPro;
using UnityEngine;

namespace CardMatch.UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] GameManager Manager;
        [Header("Score Panel Fields")]
        [SerializeField] TextMeshProUGUI TimerText;
        [SerializeField] TextMeshProUGUI ScoreText;
        [SerializeField] TextMeshProUGUI MultiplierText;
        
        void OnEnable()
        {
            Manager.OnScoreChanged += ScoreChangedHandler;
            Manager.OnTimeUpdate += TimeUpdateHandler;
            Manager.OnGameOver += GameOverHandler;
        }

        void OnDisable()
        {
            Manager.OnScoreChanged -= ScoreChangedHandler;
            Manager.OnTimeUpdate -= TimeUpdateHandler;
            Manager.OnGameOver -= GameOverHandler;
        }

        void ScoreChangedHandler()
        {
            ScoreText.text = Manager.GetScore().ToString();
            MultiplierText.text = Manager.GetScoreMultiplier().ToString();
        }

        void TimeUpdateHandler()
        {
            TimerText.text = FormatTime(Manager.GetGameTime());
        }

        void GameOverHandler()
        {
            gameObject.SetActive(false);
        }

        static string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}