using System.Collections.Generic;
using CardMatch.Card;
using CardMatch.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup Container;

        public void SetupBoard(List<CardView> cards)
        {
            float cardWidth = cards[0].GetComponent<RectTransform>().rect.width;
            SetBoardConstraints(cardWidth);
            PlaceCards(cards);
        }

        void SetBoardConstraints(float cardWidth)
        {
            float boardWidth = Container.GetComponent<RectTransform>().rect.width;
            int maxColumns = Mathf.FloorToInt(boardWidth / (cardWidth + Container.spacing.x));
            Container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            Container.constraintCount = maxColumns;
        }

        void PlaceCards(IEnumerable<CardView> cards)
        {
            CardMatchLogger.Log("Placing cards");
            foreach (var card in cards)
            {
                Instantiate(card, Container.transform);
            }
        }
    }
}