using System;
using System.Collections.Generic;
using System.Linq;
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
        [Tooltip("If GameManager is set to use resizable cards this is the container that will be used.")]
        [SerializeField] Transform ResizableContainer;
        [SerializeField] GameObject RowPrefab;
        [SerializeField] GameObject InvisibleCardPrefab;

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

        public async UniTask<List<CardView>> SetupBoardWithResizableCards(List<CardState> savedCards, BoardConfig config, float cardsShowDuration)
        {
            int cardCount = savedCards.Count;
            var boardConfiguration = config.Configurations.First(boardConfig => boardConfig.CardsCount == cardCount);
            var rowsInstances = CreateRowCols(savedCards, null, config, boardConfiguration);
            await UniTask.NextFrame();
            ResizableContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
            foreach (var rowInstance in rowsInstances)
            {
                rowInstance.enabled = false;
            }
            LoadCardStates(savedCards);
            ShowCards(cardsShowDuration);
            return _cardsInstances;
        }

        public async UniTask<List<CardView>> SetupBoardWithResizableCards(List<CardView> cardsPrefabs, BoardConfig config, float cardsShowDuration)
        {
            int cardCount = cardsPrefabs.Count;
            var boardConfiguration = config.Configurations.First(boardConfig => boardConfig.CardsCount == cardCount);
            var rowsInstances = CreateRowCols(null, cardsPrefabs, config, boardConfiguration);
            await UniTask.NextFrame();
            ResizableContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
            foreach (var rowInstance in rowsInstances)
            {
                rowInstance.enabled = false;
            }
            ShowCards(cardsShowDuration);
            return _cardsInstances;
        }

        List<HorizontalLayoutGroup> CreateRowCols(List<CardState> savedCards, List<CardView> cardPrefabs, BoardConfig config,
            BoardConfiguration boardConfiguration)
        {
            int rowCount = boardConfiguration.RowsCount;
            int colCount = boardConfiguration.ColsCount;
            var rowsInstances = new List<HorizontalLayoutGroup>();
            int i = 0;
            for (int row = 0; row < rowCount; row++)
            {
                var rowInstance = Instantiate(RowPrefab, ResizableContainer);
                for (int col = 0; col < colCount; col++)
                {
                    bool isInvisible = boardConfiguration.InvisibleCards
                        .Any(cardPosition => row == cardPosition.Row && col == cardPosition.Col);

                    if (!isInvisible)
                    {
                        if (savedCards != null)
                        {
                            InstantiateCardFromCardState(config.Cards, rowInstance.transform, savedCards[i]);
                        }
                        else
                        {
                            InstantiateCardFromPrefab(cardPrefabs[i], rowInstance.transform, i);
                        }
                        ++i;
                    }
                    else
                    {
                        Instantiate(InvisibleCardPrefab, rowInstance.transform);
                    }
                }
                rowsInstances.Add(rowInstance.GetComponent<HorizontalLayoutGroup>());
            }
            return rowsInstances;
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
                InstantiateCardFromCardState(cardsPrefabs, Container.transform, cardState);
            }
        }

        void PlaceCards(IEnumerable<CardView> cardsPrefabs)
        {
            int cardId = 0;
            foreach (var cardPrefab in cardsPrefabs)
            {
                InstantiateCardFromPrefab(cardPrefab, Container.transform, cardId);
                cardId++;
            }
        }

        void InstantiateCardFromCardState(List<CardView> cardsPrefabs, Transform containerTransform, CardState cardState)
        {
            var cardView = Instantiate(cardsPrefabs[cardState.CardIndex], containerTransform);
            cardView.CardId = cardState.CardId;
            cardView.CardIndex = cardState.CardIndex;
            cardView.OnCardShow += CardShowHandler;
            _cardsInstances.Add(cardView);
        }

        void InstantiateCardFromPrefab(CardView cardPrefab, Transform containerTransform, int cardId)
        {
            var cardView = Instantiate(cardPrefab, containerTransform);
            cardView.CardIndex = cardPrefab.CardIndex;
            cardView.CardId = cardId;
            cardView.OnCardShow += CardShowHandler;
            _cardsInstances.Add(cardView);
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