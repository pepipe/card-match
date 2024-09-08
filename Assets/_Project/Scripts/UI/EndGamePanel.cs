using TMPro;
using UnityEngine;

namespace CardMatch.UI
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] GameManager Manager;
        [SerializeField] GameObject Panel;
        [SerializeField] TextMeshProUGUI ScoreText;

        void Awake()
        {
            Panel.SetActive(false);
        }

        void OnEnable()
        {
            Manager.OnGameOver += GameOverHandler;
        }

        void OnDisable()
        {
            Manager.OnGameOver -= GameOverHandler;
        }

        void GameOverHandler()
        {
            ScoreText.text = Manager.GetScore().ToString();
            Panel.SetActive(true);
        }
    }
}