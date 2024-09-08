using System;
using System.Collections.Generic;
using CardMatch.Card;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup Container;

        public Action<CardView> OnCardClicked;
        List<CardView> _cardsInstances = new();

        void OnDestroy()
        {
            foreach (var cardInstance in _cardsInstances)
            {
                cardInstance.OnCardClicked -= OnCardClicked;
            }
        }

        public async UniTaskVoid SetupBoard(List<CardView> cardsPrefabs)
        {
            float cardWidth = cardsPrefabs[0].GetComponent<RectTransform>().rect.width;
            SetBoardConstraints(cardWidth);
            PlaceCards(cardsPrefabs);
            await UniTask.NextFrame();
            Container.enabled = false;
        }

        void SetBoardConstraints(float cardWidth)
        {
            float boardWidth = Container.GetComponent<RectTransform>().rect.width;
            int maxColumns = Mathf.FloorToInt(boardWidth / (cardWidth + Container.spacing.x));
            Container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            Container.constraintCount = maxColumns;
        }

        void PlaceCards(IEnumerable<CardView> cardsPrefabs)
        {
            foreach (var cardPrefab in cardsPrefabs)
            {
                var cardView = Instantiate(cardPrefab, Container.transform);
                cardView.CardValue = cardPrefab.CardValue;
                cardView.OnCardClicked += CardClickHandler;
                _cardsInstances.Add(cardView);
            }
        }

        void CardClickHandler(CardView card)
        {
            OnCardClicked?.Invoke(card);
        }
    }
}