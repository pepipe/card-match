using System;
using System.Collections.Generic;
using CardMatch.Card;
using CardMatch.SaveGame;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.Board
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup Container;

        public CardView SavedCardFacedUp() => _savedCardFaceUp;

        public Action<CardView> OnCardShow;

        List<CardView> _cardsInstances = new();
        CardView _savedCardFaceUp;

        void OnDestroy()
        {
            foreach (var cardInstance in _cardsInstances)
            {
                cardInstance.OnCardShow -= CardShowHandler;
            }
        }

        public async UniTask<List<CardView>> SetupBoard(List<CardState> savedCards, List<CardView> cardsPrefabs, float cardsShowDuration)
        {
            float cardWidth = cardsPrefabs[0].GetComponent<RectTransform>().rect.width;
            SetBoardConstraints(cardWidth);
            PlaceCards(savedCards, cardsPrefabs);
            await UniTask.NextFrame();
            Container.enabled = false;
            LoadCardStates(savedCards);
            ShowCards(cardsShowDuration);
            return _cardsInstances;
        }
        
        public async UniTask<List<CardView>> SetupBoard(List<CardView> cardsPrefabs, float cardsShowDuration)
        {
            float cardWidth = cardsPrefabs[0].GetComponent<RectTransform>().rect.width;
            SetBoardConstraints(cardWidth);
            PlaceCards(cardsPrefabs);
            await UniTask.NextFrame();
            Container.enabled = false;
            ShowCards(cardsShowDuration);
            return _cardsInstances;
        }

        void SetBoardConstraints(float cardWidth)
        {
            float boardWidth = Container.GetComponent<RectTransform>().rect.width;
            int maxColumns = Mathf.FloorToInt(boardWidth / (cardWidth + Container.spacing.x));
            Container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            Container.constraintCount = maxColumns;
        }

        void PlaceCards(IEnumerable<CardState> cardsState, List<CardView> cardsPrefabs)
        {
            foreach (var cardState in cardsState)
            {
                var cardView = Instantiate(cardsPrefabs[cardState.CardIndex], Container.transform);
                cardView.CardId = cardState.CardId;
                cardView.CardIndex = cardState.CardIndex;
                cardView.OnCardShow += CardShowHandler;
                _cardsInstances.Add(cardView);
            }
        }
        
        void PlaceCards(IEnumerable<CardView> cardsPrefabs)
        {
            int cardId = 0;
            foreach (var cardPrefab in cardsPrefabs)
            {
                var cardView = Instantiate(cardPrefab, Container.transform);
                cardView.CardIndex = cardPrefab.CardIndex;
                cardView.CardId = cardId++;
                cardView.OnCardShow += CardShowHandler;
                _cardsInstances.Add(cardView);
            }
        }

        void LoadCardStates(List<CardState> savedCards)
        {
            for (int i = 0; i < _cardsInstances.Count; ++i)
            {
                var cardState = savedCards[i];
                _cardsInstances[i].gameObject.SetActive(cardState.IsActive);

                if (!savedCards[i].IsFaceUp) continue;
                _cardsInstances[i].MakeCardFaceUp();
                _savedCardFaceUp = _cardsInstances[i];
            }
        }

        void ShowCards(float cardsShowDuration)
        {
            foreach (var card in _cardsInstances)
            {
                if(card.IsCardFlipped()) continue;
                card.InitialShowCard(cardsShowDuration).Forget();
            }
        }

        void CardShowHandler(CardView card)
        {
            OnCardShow?.Invoke(card);
        }
    }
}